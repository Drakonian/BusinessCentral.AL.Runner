codeunit 50907 "Sales Line Tests"
{
    Subtype = Test;

    var
        SalesLineMgmt: Codeunit "Sales Line Management";
        Assert: Codeunit Assert;

    [Test]
    procedure TestInsertAndGetWithCompositePK()
    var
        SalesLine: Record "Test Sales Line";
    begin
        // [GIVEN] A sales line with composite PK (DocType, DocNo, LineNo)
        SalesLine.Init();
        SalesLine."Document Type" := 'ORDER';
        SalesLine."Document No." := 'ORD-001';
        SalesLine."Line No." := 10000;
        SalesLine."Item No." := 'ITEM-A';
        SalesLine."Quantity" := 5;
        SalesLine."Unit Price" := 10.00;
        SalesLine."Amount" := 50.00;
        SalesLine.Insert(true);

        // [WHEN] Getting the record by composite key
        SalesLine.Init();
        SalesLine.Get('ORDER', 'ORD-001', 10000);

        // [THEN] All fields should match
        Assert.AreEqual('ITEM-A', SalesLine."Item No.", 'Item No. should match after Get');
        Assert.AreEqual(5, SalesLine."Quantity", 'Quantity should match after Get');
    end;

    [Test]
    procedure TestModifyWithCompositePK()
    var
        SalesLine: Record "Test Sales Line";
    begin
        // [GIVEN] An existing sales line
        CreateSalesLine('ORDER', 'ORD-002', 10000, 'ITEM-B', 3, 20.00, 60.00);

        // [WHEN] Modifying the quantity and recalculating amount
        SalesLine.Get('ORDER', 'ORD-002', 10000);
        SalesLine."Quantity" := 7;
        SalesLine.Modify();
        SalesLineMgmt.UpdateAmount('ORDER', 'ORD-002', 10000);

        // [THEN] Amount should be recalculated
        SalesLine.Get('ORDER', 'ORD-002', 10000);
        Assert.AreEqual(140, SalesLine."Amount", 'Amount should be 7 * 20 = 140');
    end;

    [Test]
    procedure TestDeleteWithCompositePK()
    var
        SalesLine: Record "Test Sales Line";
        Deleted: Boolean;
    begin
        // [GIVEN] Two sales lines for the same document
        CreateSalesLine('INVOICE', 'INV-001', 10000, 'ITEM-C', 1, 100.00, 100.00);
        CreateSalesLine('INVOICE', 'INV-001', 20000, 'ITEM-D', 2, 50.00, 100.00);

        // [WHEN] Deleting the first line
        Deleted := SalesLineMgmt.DeleteLine('INVOICE', 'INV-001', 10000);

        // [THEN] First line deleted, second still exists
        Assert.IsTrue(Deleted, 'Delete should return true');
        Assert.IsFalse(SalesLine.Get('INVOICE', 'INV-001', 10000), 'First line should be deleted');
        Assert.IsTrue(SalesLine.Get('INVOICE', 'INV-001', 20000), 'Second line should still exist');
    end;

    [Test]
    procedure TestGetTotalForDocument()
    var
        Total: Decimal;
    begin
        // [GIVEN] Multiple lines across different document types
        CreateSalesLine('ORDER', 'ORD-010', 10000, 'ITEM-E', 2, 25.00, 50.00);
        CreateSalesLine('ORDER', 'ORD-010', 20000, 'ITEM-F', 1, 75.00, 75.00);
        CreateSalesLine('INVOICE', 'ORD-010', 10000, 'ITEM-G', 1, 999.00, 999.00);

        // [WHEN] Getting total for the Order document
        Total := SalesLineMgmt.GetTotalForDocument('ORDER', 'ORD-010');

        // [THEN] Should only sum Order lines, not Invoice lines
        Assert.AreEqual(125, Total, 'Total should be 50 + 75 = 125 (excluding invoice line)');
    end;

    local procedure CreateSalesLine(DocType: Code[10]; DocNo: Code[20]; LineNo: Integer; ItemNo: Code[20]; Qty: Integer; Price: Decimal; Amount: Decimal)
    var
        SalesLine: Record "Test Sales Line";
    begin
        SalesLine.Init();
        SalesLine."Document Type" := DocType;
        SalesLine."Document No." := DocNo;
        SalesLine."Line No." := LineNo;
        SalesLine."Item No." := ItemNo;
        SalesLine."Quantity" := Qty;
        SalesLine."Unit Price" := Price;
        SalesLine."Amount" := Amount;
        SalesLine.Insert(true);
    end;
}
