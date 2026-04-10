codeunit 50911 "Variant Type Tests"
{
    Subtype = Test;

    var
        VariantHelper: Codeunit "Variant Helper";
        Assert: Codeunit Assert;

    [Test]
    procedure TestVariantDetectsInteger()
    var
        V: Variant;
    begin
        // [GIVEN] An integer value assigned to a Variant
        V := 42;

        // [WHEN] Checking the type
        // [THEN] Should detect as Integer
        Assert.AreEqual('Integer', VariantHelper.VariantToText(V), 'Should detect integer variant');
    end;

    [Test]
    procedure TestVariantDetectsText()
    var
        V: Variant;
    begin
        V := 'Hello World';
        Assert.AreEqual('Text', VariantHelper.VariantToText(V), 'Should detect text variant');
    end;

    [Test]
    procedure TestVariantDetectsBoolean()
    var
        V: Variant;
    begin
        V := true;
        Assert.AreEqual('Boolean', VariantHelper.VariantToText(V), 'Should detect boolean variant');
    end;

    [Test]
    procedure TestVariantDetectsDecimal()
    var
        V: Variant;
    begin
        V := 3.14;
        Assert.AreEqual('Decimal', VariantHelper.VariantToText(V), 'Should detect decimal variant');
    end;

    [Test]
    procedure TestIsNumericWithInteger()
    var
        V: Variant;
    begin
        V := 100;
        Assert.IsTrue(VariantHelper.IsNumeric(V), 'Integer should be numeric');
    end;

    [Test]
    procedure TestIsNumericWithText()
    var
        V: Variant;
    begin
        V := 'not a number';
        Assert.IsFalse(VariantHelper.IsNumeric(V), 'Text should not be numeric');
    end;

    [Test]
    procedure TestPassThroughInteger()
    var
        V: Variant;
    begin
        // [GIVEN/WHEN] Passing an integer through variant
        VariantHelper.PassThroughInteger(99, V);

        // [THEN] Variant should hold integer
        Assert.IsTrue(V.IsInteger, 'Variant should be integer after pass-through');
    end;

    [Test]
    procedure TestPassThroughBoolean()
    var
        V: Variant;
    begin
        VariantHelper.PassThroughBoolean(true, V);
        Assert.IsTrue(V.IsBoolean, 'Variant should be boolean after pass-through');
    end;
}
