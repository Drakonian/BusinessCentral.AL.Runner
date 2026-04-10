table 50107 "Test Sales Line"
{
    DataClassification = ToBeClassified;

    fields
    {
        field(1; "Document Type"; Code[10])
        {
            DataClassification = ToBeClassified;
        }
        field(2; "Document No."; Code[20])
        {
            DataClassification = ToBeClassified;
        }
        field(3; "Line No."; Integer)
        {
            DataClassification = ToBeClassified;
        }
        field(4; "Item No."; Code[20])
        {
            DataClassification = ToBeClassified;
        }
        field(5; "Quantity"; Integer)
        {
            DataClassification = ToBeClassified;
        }
        field(6; "Unit Price"; Decimal)
        {
            DataClassification = ToBeClassified;
        }
        field(7; "Amount"; Decimal)
        {
            DataClassification = ToBeClassified;
        }
    }

    keys
    {
        key(PK; "Document Type", "Document No.", "Line No.")
        {
            Clustered = true;
        }
    }
}
