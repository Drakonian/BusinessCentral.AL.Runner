codeunit 50107 "Sales Line Management"
{
    procedure UpdateAmount(DocType: Code[10]; DocNo: Code[20]; LineNo: Integer)
    var
        SalesLine: Record "Test Sales Line";
    begin
        SalesLine.Get(DocType, DocNo, LineNo);
        SalesLine."Amount" := SalesLine."Quantity" * SalesLine."Unit Price";
        SalesLine.Modify();
    end;

    procedure GetTotalForDocument(DocType: Code[10]; DocNo: Code[20]): Decimal
    var
        SalesLine: Record "Test Sales Line";
        Total: Decimal;
    begin
        SalesLine.SetRange("Document Type", DocType);
        SalesLine.SetRange("Document No.", DocNo);
        if SalesLine.FindSet() then
            repeat
                Total += SalesLine."Amount";
            until SalesLine.Next() = 0;
        exit(Total);
    end;

    procedure DeleteLine(DocType: Code[10]; DocNo: Code[20]; LineNo: Integer): Boolean
    var
        SalesLine: Record "Test Sales Line";
    begin
        if SalesLine.Get(DocType, DocNo, LineNo) then begin
            SalesLine.Delete(false);
            exit(true);
        end;
        exit(false);
    end;
}
