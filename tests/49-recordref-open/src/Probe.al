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

    procedure ProbeLocal(): Integer
    var
        RecRef: RecordRef;
        Sentinel: Integer;
    begin
        // Single-arg form still compiles and runs.
        // IsEmpty() is called in an if-branch so BC lowers it through
        // whichever runtime path is current for that compiler version,
        // but we don't assert on the actual truth value — the RecordRef
        // runner policy is "stub compiles, does not function".
        Sentinel := 0;
        RecRef.Open(18);
        if RecRef.IsEmpty() then
            Sentinel += 1;
        Sentinel += 10;
        exit(Sentinel);
    end;
}
