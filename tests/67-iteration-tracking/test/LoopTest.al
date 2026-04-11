codeunit 50921 "Loop Tests"
{
    Subtype = Test;

    var
        Helper: Codeunit "Loop Helper";
        Assert: Codeunit Assert;

    [Test]
    procedure TestSimpleLoop()
    var
        Result: Integer;
    begin
        Result := Helper.SumRange(1, 5);
        Assert.AreEqual(15, Result, 'Sum 1..5 should be 15');
    end;

    [Test]
    procedure TestLoopWithBranch()
    begin
        Helper.CollectEvenOdd(4);
        // Messages: odd: 1, even: 2, odd: 3, even: 4
    end;
}
