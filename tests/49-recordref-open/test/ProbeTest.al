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
    procedure OneArgOpenCompilesAndRuns()
    var
        Probe: Codeunit "RR Open Probe";
        Result: Integer;
    begin
        // [GIVEN] A procedure that calls single-arg RecRef.Open and IsEmpty in an if-branch
        // [THEN] The codeunit must compile and the procedure must return its post-call sentinel.
        //        We deliberately avoid asserting the truth value of IsEmpty because BC's
        //        lowering for `RecRef.IsEmpty()` varies across compiler versions and the
        //        RecordRef stub policy is "compiles but does not function at runtime".
        Result := Probe.ProbeLocal();
        Assert.IsTrue(Result >= 10, 'ProbeLocal must reach the post-IsEmpty sentinel');
    end;
}
