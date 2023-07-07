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
            context.Users.AddOrUpdate(
                new CarBookingTest.Models.Account { Username = "admin0001", Password = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=", EmployeeNumber = "AD0001", IsDeleted = false, Sex = true, Created = DateTime.Now },
                new CarBookingTest.Models.Account { Username = "admin0002", Password = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=", EmployeeNumber = "AD0002", IsDeleted = false, Sex = true, Created = DateTime.Now }
            );

            context.Requests.AddOrUpdate(
                new CarBookingTest.Models.Request { RequestCode = "2023OPS-CAR-0706-001", TotalPassengers = 1, SenderId = 1, ReceiverId = 2, UsageFrom = DateTime.Now, UsageTo = DateTime.Now, PickTime = DateTime.Now, isDeleted = false },
                new CarBookingTest.Models.Request { RequestCode = "2023OPS-CAR-0706-002", TotalPassengers = 1, SenderId = 1, ReceiverId = 2, UsageFrom = DateTime.Now, UsageTo = DateTime.Now, PickTime = DateTime.Now, isDeleted = false },
                new CarBookingTest.Models.Request { RequestCode = "2023OPS-CAR-0706-003", TotalPassengers = 1, SenderId = 1, ReceiverId = 2, UsageFrom = DateTime.Now, UsageTo = DateTime.Now, PickTime = DateTime.Now, isDeleted = false },
                new CarBookingTest.Models.Request { RequestCode = "2023OPS-CAR-0706-004", TotalPassengers = 1, SenderId = 1, ReceiverId = 2, UsageFrom = DateTime.Now, UsageTo = DateTime.Now, PickTime = DateTime.Now, isDeleted = false }
                );

            context.RequestComments.AddOrUpdate(
                new CarBookingTest.Models.RequestComment { Content = "hello", isDeleted = false, Created = DateTime.Now, RequestId = 1 },
                new CarBookingTest.Models.RequestComment { Content = "hello", isDeleted = false, Created = DateTime.Now, RequestId = 1 },
                new CarBookingTest.Models.RequestComment { Content = "hello", isDeleted = false, Created = DateTime.Now, RequestId = 1 },
                new CarBookingTest.Models.RequestComment { Content = "hello", isDeleted = false, Created = DateTime.Now, RequestId = 1 }
                );
            //  This method will be called after migrating to the latest version.

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
