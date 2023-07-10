namespace CarBookingBE.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CarBookingTest.Models.MyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CarBookingTest.Models.MyDbContext context)
        {
        //  This method will be called after migrating to the latest version.
        Seed:
            context.Users.AddOrUpdate(
                            new CarBookingTest.Models.Account { Username = "admin0001", Password = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=", EmployeeNumber = "AD0001", IsDeleted = false, Sex = true, Created = DateTime.Now },
                            new CarBookingTest.Models.Account { Username = "admin0002", Password = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=", EmployeeNumber = "AD0002", IsDeleted = false, Sex = true, Created = DateTime.Now }
                        );

            context.Requests.AddOrUpdate(
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-001",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    Status = "Waiting for approval",
                    IsDeleted = false
                },
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-001",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    Status = "Waiting for approval",
                    IsDeleted = false
                },
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-001",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    Status = "Waiting for approval",
                    IsDeleted = false
                },
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-002",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    Status = "Approved",
                    IsDeleted = false
                },
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-002",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    Status = "Approved",
                    IsDeleted = false
                },
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-003",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    Status = "Rejected",
                    IsDeleted = false
                },
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-003",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    Status = "Rejected",
                    IsDeleted = false
                },
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-004",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    Status = "Done",
                    IsDeleted = false
                }
                );

            /*context.RequestComments.AddOrUpdate(
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now },
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now },
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now },
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now }
                );*/
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
