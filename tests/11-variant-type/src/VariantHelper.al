codeunit 50112 "Variant Helper"
{
    procedure VariantToText(V: Variant): Text
    begin
        if V.IsInteger then
            exit('Integer');
        if V.IsDecimal then
            exit('Decimal');
        if V.IsBoolean then
            exit('Boolean');
        if V.IsText then
            exit('Text');
        if V.IsCode then
            exit('Code');
        exit('Unknown');
    end;

    procedure IsNumeric(V: Variant): Boolean
    begin
        exit(V.IsInteger or V.IsDecimal);
    end;

    procedure PassThroughInteger(Value: Integer; var Result: Variant)
    begin
        Result := Value;
    end;

    procedure PassThroughText(Value: Text; var Result: Variant)
    begin
        Result := Value;
    end;

    procedure PassThroughBoolean(Value: Boolean; var Result: Variant)
    begin
        Result := Value;
    end;

    procedure PassThroughDecimal(Value: Decimal; var Result: Variant)
    begin
        Result := Value;
    end;
}
