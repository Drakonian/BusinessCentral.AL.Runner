codeunit 50490 "RR Open Probe"
{
    procedure ProbeCompany(CompanyName: Text): Boolean
    var
        RecRef: RecordRef;
    begin
        // Three-arg form: TableNo, Temporary, CompanyName.
        // In BC this is the standard way to probe a specific company's copy
        // of a table; in standalone mode the RecordRef stub returns
        // IsEmpty = true so the caller gets a sane negative result.
        RecRef.Open(18, false, CompanyName);
        if RecRef.IsEmpty() then
            exit(false);
        exit(true);
    end;

    procedure ProbeLocal(): Boolean
    var
        RecRef: RecordRef;
    begin
        // Single-arg form still works.
        RecRef.Open(18);
        exit(RecRef.IsEmpty());
    end;
}
