namespace AlRunner.Runtime;

using Microsoft.Dynamics.Nav.Runtime;
using Microsoft.Dynamics.Nav.Types;

/// <summary>
/// In-memory replacement for ALIsolatedStorage.
/// IsolatedStorage in BC is a key-value store backed by the database.
/// We mock it the same way we mock record access — pure in-memory.
/// </summary>
public static class MockIsolatedStorage
{
    private static readonly Dictionary<string, string> _store = new();

    public static void ResetAll() => _store.Clear();

    // ALSet overloads
    public static void ALSet(DataError errorLevel, string key, string value, object dataScope)
    {
        _store[key] = value;
    }

    public static void ALSet(DataError errorLevel, string key, NavSecretText value, object dataScope)
    {
        _store[key] = value.ToString() ?? "";
    }

    public static void ALSet(DataError errorLevel, string key, string value, object dataScope, object encryption)
    {
        _store[key] = value;
    }

    public static void ALSet(DataError errorLevel, string key, NavSecretText value, object dataScope, object encryption)
    {
        _store[key] = value.ToString() ?? "";
    }

    // ALGet overloads
    public static bool ALGet(DataError errorLevel, string key, object dataScope, out NavText value)
    {
        if (_store.TryGetValue(key, out var v))
        {
            value = new NavText(v);
            return true;
        }
        value = new NavText("");
        return false;
    }

    public static bool ALGet(DataError errorLevel, string key, object dataScope, out NavSecretText value)
    {
        if (_store.TryGetValue(key, out var v))
        {
            value = NavSecretText.Create(v);
            return true;
        }
        value = NavSecretText.Create("");
        return false;
    }

    // ALContains
    public static bool ALContains(DataError errorLevel, string key, object dataScope)
    {
        return _store.ContainsKey(key);
    }

    // ALDelete
    public static bool ALDelete(DataError errorLevel, string key, object dataScope)
    {
        return _store.Remove(key);
    }
}
