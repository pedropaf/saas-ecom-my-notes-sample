namespace MyNotes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillingAddressAddedToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "BillingAddress_Name", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_AddressLine1", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_AddressLine2", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_City", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_State", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_ZipCode", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_Country", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_Vat", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "BillingAddress_Vat");
            DropColumn("dbo.AspNetUsers", "BillingAddress_Country");
            DropColumn("dbo.AspNetUsers", "BillingAddress_ZipCode");
            DropColumn("dbo.AspNetUsers", "BillingAddress_State");
            DropColumn("dbo.AspNetUsers", "BillingAddress_City");
            DropColumn("dbo.AspNetUsers", "BillingAddress_AddressLine2");
            DropColumn("dbo.AspNetUsers", "BillingAddress_AddressLine1");
            DropColumn("dbo.AspNetUsers", "BillingAddress_Name");
        }
    }
}
