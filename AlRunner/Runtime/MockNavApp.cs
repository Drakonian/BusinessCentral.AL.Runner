using Microsoft.Dynamics.Nav.Runtime;
using Microsoft.Dynamics.Nav.Types;

namespace AlRunner.Runtime;

/// <summary>
/// Stub for <c>ALNavApp</c>. The real BC type's
/// <c>ALGetModuleInfo</c> / <c>ALGetCurrentModuleInfo</c> /
/// <c>ALGetCallerModuleInfo</c> reach into
/// <c>Microsoft.Dynamics.Nav.CodeAnalysis</c>, which isn't shipped
/// with al-runner — any AL call path that touches NavApp metadata
/// crashes with "Could not load file or assembly ..." under
/// standalone mode.
///
/// MockNavApp returns false for every lookup (no module metadata is
/// available without a service tier), leaving the ByRef'd
/// <see cref="NavModuleInfo"/> at its default. Callers are expected
/// to treat the false return as "not found" and fall back to a
/// placeholder, matching BC's own contract.
/// </summary>
public static class MockNavApp
{
    public static bool ALGetModuleInfo(DataError errorLevel, Guid moduleId, ByRef<NavModuleInfo> info)
    {
        // Intentionally don't touch the ByRef — the caller's local
        // ModuleInfo stays at its zero state, which BC handles as
        // "no module info". Setting a real value would require
        // metadata we don't have.
        return false;
    }

    public static bool ALGetCurrentModuleInfo(DataError errorLevel, ByRef<NavModuleInfo> info)
    {
        return false;
    }

    public static bool ALGetCallerModuleInfo(DataError errorLevel, ByRef<NavModuleInfo> info)
    {
        return false;
    }

    public static NavList<NavModuleInfo> ALGetCallerCallstackModuleInfos()
    {
        return NavList<NavModuleInfo>.Default;
    }
}
