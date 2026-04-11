codeunit 50491 "RR Open Tests"
{
    Subtype = Test;

    var
        Assert: Codeunit Assert;

    [Test]
    procedure ThreeArgOpenIsCallable()
    var
        Probe: Codeunit "RR Open Probe";
    begin
        // [WHEN] Calling RecRef.Open(int, bool, text) followed by IsEmpty
        // [THEN] The procedure compiles and runs; IsEmpty returns true for the stub
        Assert.IsFalse(Probe.ProbeCompany('CRONUS'), 'IsEmpty=true stub → not-present branch');
    end;

    [Test]
    procedure OneArgOpenStillWorks()
    var
        Probe: Codeunit "RR Open Probe";
    begin
        Assert.IsTrue(Probe.ProbeLocal(), 'Single-arg Open + IsEmpty must still resolve');
    end;
}
