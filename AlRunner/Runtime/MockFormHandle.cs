namespace AlRunner.Runtime;

/// <summary>
/// Compile-only stub for <c>NavFormHandle</c> / AL's
/// <c>Page "X"</c> variable. BC instantiates the field as
/// <c>new NavFormHandle(this, pageId)</c>, which demands ITreeObject
/// and a valid NavForm argument — unavailable standalone.
///
/// MockFormHandle has a parameterless constructor and no-op Run /
/// RunModal / SetRecord / GetRecord so AL code that declares a Page
/// variable and calls <c>p.Run()</c> compiles and executes without
/// touching the (unavailable) UI surface. Consistent with the existing
/// "pages stub but do not function" policy — any attempt to read page
/// state back returns defaults.
/// </summary>
public class MockFormHandle
{
    public int Id { get; private set; }

    public MockFormHandle() { }

    public MockFormHandle(int id)
    {
        Id = id;
    }

    public void Run() { }
    public void RunModal() { }
    public void SetRecord(object? record) { }
    public object? GetRecord() => null;
    public void Update(bool saveRecord = true) { }
    public void Close() { }
    public void Activate() { }
}
