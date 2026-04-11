namespace AlRunner.Runtime;

/// <summary>
/// Stub for NavRecordRef / AL's <c>RecordRef</c>. Declaration-site type
/// and the row-presence surface (Open, IsEmpty, FindSet, Find/First/Last,
/// Next, Count, Close) are backed by the shared in-memory table store
/// exposed through <see cref="MockRecordHandle.TableHasAnyRow"/> — so
/// opening a table via RecRef and asking IsEmpty sees rows that typed
/// Record variables have inserted into the same table. Field-level
/// access (RecRef.Field(n).Value) is still out of scope and will return
/// defaults.
/// </summary>
public class MockRecordRef
{
    public int Number { get; private set; }
    private int _cursor = -1;

    public MockRecordRef() { }

    public void Clear()
    {
        Number = 0;
        _cursor = -1;
    }

    public void Open(int tableId) => Open(tableId, false, null);
    public void Open(int tableId, bool temporary) => Open(tableId, temporary, null);
    public void Open(int tableId, bool temporary, string? companyName)
    {
        Number = tableId;
        _cursor = -1;
    }

    public void Close()
    {
        Number = 0;
        _cursor = -1;
    }

    public bool IsEmpty() => Number == 0 || !MockRecordHandle.TableHasAnyRow(Number);

    public bool Find(string which)
    {
        if (Number == 0) return false;
        var count = MockRecordHandle.TableRowCount(Number);
        if (count == 0) return false;
        _cursor = 0;
        return true;
    }

    public bool FindFirst() => Find("-");

    public bool FindLast()
    {
        if (Number == 0) return false;
        var count = MockRecordHandle.TableRowCount(Number);
        if (count == 0) return false;
        _cursor = count - 1;
        return true;
    }

    public bool FindSet() => Find("-");

    public bool Next()
    {
        if (Number == 0) return false;
        var count = MockRecordHandle.TableRowCount(Number);
        if (_cursor < 0 || _cursor + 1 >= count) return false;
        _cursor++;
        return true;
    }

    public int Count() => MockRecordHandle.TableRowCount(Number);

    // AL-lowered surface the BC compiler sometimes emits for RecordRef calls.
    // BC prefixes most of the ALOpen overloads with a CompilationTarget enum,
    // which we accept as an untyped first arg (rewriter needs zero changes
    // to pass it through). ALIsEmpty is a property in the BC runtime — not
    // a method — so we expose it the same way or AL lowering produces
    // `method group` errors on `!recRef.ALIsEmpty` expressions.
    public void ALOpen(int tableId) => Open(tableId);
    public void ALOpen(int tableId, bool temporary) => Open(tableId, temporary);
    public void ALOpen(int tableId, bool temporary, string companyName) => Open(tableId, temporary, companyName);
    public void ALOpen(object compilationTarget, int tableId) => Open(tableId);
    public void ALOpen(object compilationTarget, int tableId, bool temporary) => Open(tableId, temporary);
    public void ALOpen(object compilationTarget, int tableId, bool temporary, string companyName) => Open(tableId, temporary, companyName);
    public void ALClose() => Close();
    public bool ALIsEmpty => IsEmpty();
    public bool ALFindFirst() => FindFirst();
    public bool ALFindSet() => FindSet();
    public int ALCount() => Count();
    public int ALGetNumber() => Number;
}
