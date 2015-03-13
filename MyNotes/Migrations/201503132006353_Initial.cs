namespace MyNotes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditCards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StripeId = c.String(),
                        Name = c.String(nullable: false),
                        Last4 = c.String(),
                        Type = c.String(),
                        Fingerprint = c.String(),
                        AddressCity = c.String(nullable: false),
                        AddressCountry = c.String(nullable: false),
                        AddressLine1 = c.String(nullable: false),
                        AddressLine2 = c.String(),
                        AddressState = c.String(),
                        AddressZip = c.String(nullable: false),
                        Cvc = c.String(nullable: false, maxLength: 4),
                        ExpirationMonth = c.String(nullable: false),
                        ExpirationYear = c.String(nullable: false),
                        CardCountry = c.String(),
                        SaasEcomUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StripeId = c.String(maxLength: 50),
                        StripeCustomerId = c.String(maxLength: 50),
                        Date = c.DateTime(),
                        PeriodStart = c.DateTime(),
                        PeriodEnd = c.DateTime(),
                        Subtotal = c.Int(),
                        Total = c.Int(),
                        Attempted = c.Boolean(),
                        Closed = c.Boolean(),
                        Paid = c.Boolean(),
                        AttemptCount = c.Int(),
                        AmountDue = c.Int(),
                        StartingBalance = c.Int(),
                        EndingBalance = c.Int(),
                        NextPaymentAttempt = c.DateTime(),
                        ApplicationFee = c.Int(),
                        Tax = c.Int(),
                        TaxPercent = c.Decimal(precision: 18, scale: 2),
                        Currency = c.String(),
                        BillingAddress_Name = c.String(),
                        BillingAddress_AddressLine1 = c.String(),
                        BillingAddress_AddressLine2 = c.String(),
                        BillingAddress_City = c.String(),
                        BillingAddress_State = c.String(),
                        BillingAddress_ZipCode = c.String(),
                        BillingAddress_Country = c.String(),
                        BillingAddress_Vat = c.String(),
                        Description = c.String(),
                        StatementDescriptor = c.String(),
                        ReceiptNumber = c.String(),
                        Forgiven = c.Boolean(nullable: false),
                        Customer_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.StripeId, unique: true)
                .Index(t => t.StripeCustomerId)
                .Index(t => t.Paid);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        SaasEcomUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        SaasEcomUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId });
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        SaasEcomUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Start = c.DateTime(),
                        End = c.DateTime(),
                        TrialStart = c.DateTime(),
                        TrialEnd = c.DateTime(),
                        SubscriptionPlanId = c.String(maxLength: 128),
                        UserId = c.String(maxLength: 128),
                        StripeId = c.String(maxLength: 50),
                        Status = c.String(),
                        TaxPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReasonToCancel = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SubscriptionPlans", t => t.SubscriptionPlanId)
                .Index(t => t.SubscriptionPlanId)
                .Index(t => t.StripeId);
            
            CreateTable(
                "dbo.SubscriptionPlans",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Price = c.Double(nullable: false),
                        Currency = c.String(),
                        Interval = c.Int(nullable: false),
                        TrialPeriodInDays = c.Int(nullable: false),
                        Disabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubscriptionPlanProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                        SubscriptionPlan_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SubscriptionPlans", t => t.SubscriptionPlan_Id, cascadeDelete: true)
                .Index(t => t.SubscriptionPlan_Id);
            
            CreateTable(
                "dbo.LineItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StripeLineItemId = c.String(),
                        Type = c.String(),
                        Amount = c.Int(),
                        Currency = c.String(),
                        Proration = c.Boolean(nullable: false),
                        Period_Start = c.DateTime(),
                        Period_End = c.DateTime(),
                        Quantity = c.Int(),
                        Plan_StripePlanId = c.String(),
                        Plan_Interval = c.String(),
                        Plan_Name = c.String(),
                        Plan_Created = c.DateTime(),
                        Plan_AmountInCents = c.Int(),
                        Plan_Currency = c.String(),
                        Plan_IntervalCount = c.Int(nullable: false),
                        Plan_TrialPeriodDays = c.Int(),
                        Plan_StatementDescriptor = c.String(),
                        Invoice_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.Invoice_Id)
                .Index(t => t.Invoice_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        RegistrationDate = c.DateTime(nullable: false),
                        LastLoginTime = c.DateTime(nullable: false),
                        StripeCustomerId = c.String(),
                        IPAddress = c.String(),
                        IPAddressCountry = c.String(),
                        Delinquent = c.Boolean(nullable: false),
                        LifetimeValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.LineItems", "Invoice_Id", "dbo.Invoices");
            DropForeignKey("dbo.Subscriptions", "SubscriptionPlanId", "dbo.SubscriptionPlans");
            DropForeignKey("dbo.SubscriptionPlanProperties", "SubscriptionPlan_Id", "dbo.SubscriptionPlans");
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.LineItems", new[] { "Invoice_Id" });
            DropIndex("dbo.SubscriptionPlanProperties", new[] { "SubscriptionPlan_Id" });
            DropIndex("dbo.Subscriptions", new[] { "StripeId" });
            DropIndex("dbo.Subscriptions", new[] { "SubscriptionPlanId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.Invoices", new[] { "Paid" });
            DropIndex("dbo.Invoices", new[] { "StripeCustomerId" });
            DropIndex("dbo.Invoices", new[] { "StripeId" });
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.LineItems");
            DropTable("dbo.SubscriptionPlanProperties");
            DropTable("dbo.SubscriptionPlans");
            DropTable("dbo.Subscriptions");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Invoices");
            DropTable("dbo.CreditCards");
        }
    }
}
