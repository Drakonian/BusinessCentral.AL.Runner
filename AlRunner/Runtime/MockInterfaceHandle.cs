namespace AlRunner.Runtime;

using Microsoft.Dynamics.Nav.Runtime;

/// <summary>
/// Lightweight replacement for NavInterfaceHandle.
/// In the BC runtime, NavInterfaceHandle wraps an ITreeObject to represent
/// AL interface references. For standalone execution, we just store the object.
///
/// Implements <see cref="ITreeObject"/> and
/// <see cref="IALAssignable{T}"/> so MockInterfaceHandle can satisfy the
/// type constraints on <c>NavObjectList&lt;T&gt;</c> — BC generates
/// <c>NavObjectList&lt;MockInterfaceHandle&gt;</c> for AL's
/// <c>List of [Interface X]</c>.
/// </summary>
public class MockInterfaceHandle : ITreeObject, IALAssignable<MockInterfaceHandle>
{
    private object? _implementation;

    public MockInterfaceHandle()
    {
    }

    /// <summary>
    /// Constructor accepting a parent scope — used when an interface is returned
    /// from a function. The BC compiler passes the parent NavScope as an argument.
    /// In standalone mode, we ignore the parent.
    /// </summary>
    public MockInterfaceHandle(object? parent)
    {
    }

    // ITreeObject — stub properties, never inspected in standalone mode.
    TreeHandler ITreeObject.Tree => null!;
    TreeObjectType ITreeObject.Type => default;
    bool ITreeObject.SingleThreaded => false;

    /// <summary>
    /// Factory used by the rewriter when translating
    /// <c>ALCompiler.ToInterface(this, codeunit)</c>. Wraps an implementation
    /// (usually a <see cref="MockCodeunitHandle"/>) so it can be stored in a
    /// <c>NavObjectList&lt;MockInterfaceHandle&gt;</c>.
    /// </summary>
    public static MockInterfaceHandle Wrap(object? implementation)
    {
        var h = new MockInterfaceHandle();
        h.ALAssign(implementation);
        return h;
    }

    /// <summary>
    /// IALAssignable&lt;MockInterfaceHandle&gt;.ALAssign — copy another
    /// handle's implementation reference into this one. Used when AL
    /// reassigns interface variables inside a NavObjectList.
    /// </summary>
    public void ALAssign(MockInterfaceHandle other)
    {
        _implementation = other?._implementation;
    }

    /// <summary>
    /// Assigns an interface implementation (codeunit) to this handle.
    /// In BC, ALAssign wraps the codeunit as an interface implementation.
    /// </summary>
    public void ALAssign(object? implementation)
    {
        // Unwrap: if the caller hands us another MockInterfaceHandle,
        // adopt its implementation instead of nesting.
        if (implementation is MockInterfaceHandle inner)
        {
            _implementation = inner._implementation;
            return;
        }
        _implementation = implementation;
    }

    public void Clear()
    {
        _implementation = null;
    }

    /// <summary>
    /// Invoke a method on the interface implementation by member ID.
    /// Similar to MockCodeunitHandle.Invoke but via the interface dispatch pattern.
    /// In BC, InvokeInterfaceMethod dispatches through the codeunit's IsInterfaceMethod table.
    /// </summary>
    public object? InvokeInterfaceMethod(int memberId, object[] args)
    {
        if (_implementation == null)
            throw new InvalidOperationException("Interface not assigned");

        // If the implementation is a MockCodeunitHandle, delegate to it
        if (_implementation is MockCodeunitHandle handle)
            return handle.Invoke(memberId, args);

        // If the implementation is another MockInterfaceHandle (e.g., returned from a factory),
        // delegate through it
        if (_implementation is MockInterfaceHandle innerHandle)
            return innerHandle.InvokeInterfaceMethod(memberId, args);

        throw new NotSupportedException(
            $"Interface dispatch not supported for implementation type {_implementation.GetType().Name}");
    }

    /// <summary>
    /// 3-arg overload: InvokeInterfaceMethod(interfaceId, memberId, args)
    /// The interfaceId identifies which interface is being called (ignored in standalone mode).
    /// </summary>
    public object? InvokeInterfaceMethod(int interfaceId, int memberId, object[] args)
    {
        return InvokeInterfaceMethod(memberId, args);
    }
}
