using SaasEcom.Core.Models;

namespace MyNotes.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MyNotes.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MyNotes.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            var basicMonthly = new SubscriptionPlan
            {
                Id = "basic_monthly",
                Name = "Basic",
                Interval = SubscriptionPlan.SubscriptionInterval.Monthly,
                TrialPeriodInDays = 30,
                Price = 10.00,
                Currency = "USD"
            };
            basicMonthly.Properties.Add(new SubscriptionPlanProperty { Key = "MaxNotes", Value = "100" });

            var professionalMonthly = new SubscriptionPlan
            {
                Id = "professional_monthly",
                Name = "Professional",
                Interval = SubscriptionPlan.SubscriptionInterval.Monthly,
                TrialPeriodInDays = 30,
                Price = 20.00,
                Currency = "USD"
            };
            professionalMonthly.Properties.Add(new SubscriptionPlanProperty
            {
                Key = "MaxNotes",
                Value = "10000"
            });

            var businessMonthly = new SubscriptionPlan
            {
                Id = "business_monthly",
                Name = "Business",
                Interval = SubscriptionPlan.SubscriptionInterval.Monthly,
                TrialPeriodInDays = 30,
                Price = 30.00,
                Currency = "USD"
            };
            businessMonthly.Properties.Add(new SubscriptionPlanProperty
            {
                Key = "MaxNotes",
                Value = "1000000"
            });

            context.SubscriptionPlans.AddOrUpdate(
                sp => sp.Id,
                basicMonthly,
                professionalMonthly,
                businessMonthly);
        }
    }
}
