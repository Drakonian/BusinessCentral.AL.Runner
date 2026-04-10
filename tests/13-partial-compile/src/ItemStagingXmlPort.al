xmlport 50300 "Item Staging Import"
{
    Direction = Import;
    Format = VariableText;

    schema
    {
        textelement(Root)
        {
            tableelement(StagingLine; "Item Staging")
            {
                fieldelement(EntryNo; StagingLine."Entry No.") { }
                fieldelement(ItemNo; StagingLine."Item No.") { }
                fieldelement(Description; StagingLine.Description) { }
                fieldelement(Quantity; StagingLine.Quantity) { }
            }
        }
    }
}
