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
                new CarBookingTest.Models.Account { Username = "admin0002", Password = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=", EmployeeNumber = "AD0002", IsDeleted = false, Sex = true, Created = DateTime.Now },
                new CarBookingTest.Models.Account { Username = "admin0003", Password = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=", EmployeeNumber = "AD0003", IsDeleted = false, Sex = true, Created = DateTime.Now }
            );

            context.Roles.AddOrUpdate(
                new CarBookingTest.Models.Role { Title = "EMPLOYEE" },
                new CarBookingTest.Models.Role { Title = "APPROVER" },
                new CarBookingTest.Models.Role { Title = "ADMINISTRATIVE" },
                new CarBookingTest.Models.Role { Title = "SECURITY" },
                new CarBookingTest.Models.Role { Title = "ADMIN" }
            );

            /*context.Requests.AddOrUpdate(
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-001",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    SenderId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517"),
                    ReceiverId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA"),
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
                    SenderId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517"),
                    ReceiverId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA"),
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
                    SenderId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517"),
                    ReceiverId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA"),
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
                    SenderId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517"),
                    ReceiverId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA"),
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
                    SenderId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA"),
                    ReceiverId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517"),
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
                    SenderId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA"),
                    ReceiverId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517"),
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
                    SenderId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA"),
                    ReceiverId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517"),
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
                    SenderId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA"),
                    ReceiverId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517"),
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    Status = "Done",
                    IsDeleted = false
                }
                );*/

            /*context.RequestWorkflows.AddOrUpdate(
                new CarBookingTest.Models.RequestWorkflow { IsDeleted = false, Level = 1, RequestId = Guid.Parse("52DC3456-B99B-4424-8EF1-06C92B6D370C"), UserId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA") },
                new CarBookingTest.Models.RequestWorkflow { IsDeleted = false, Level = 2, RequestId = Guid.Parse("52DC3456-B99B-4424-8EF1-06C92B6D370C"), UserId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517") },
                new CarBookingTest.Models.RequestWorkflow { IsDeleted = false, Level = 1, RequestId = Guid.Parse("D7DA5AB9-FF10-48E7-952C-92935A8C27E3"), UserId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517") },
                new CarBookingTest.Models.RequestWorkflow { IsDeleted = false, Level = 2, RequestId = Guid.Parse("D7DA5AB9-FF10-48E7-952C-92935A8C27E3"), UserId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA") }
                );
*/
            /*context.RequestComments.AddOrUpdate(
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now, RequestId = Guid.Parse("52DC3456-B99B-4424-8EF1-06C92B6D370C"), UserId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA") },
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now, RequestId = Guid.Parse("52DC3456-B99B-4424-8EF1-06C92B6D370C"), UserId = Guid.Parse("E80C2698-A2C6-457C-B72C-C82ED2A136CA") },
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now, RequestId = Guid.Parse("52DC3456-B99B-4424-8EF1-06C92B6D370C"), UserId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517") },
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now, RequestId = Guid.Parse("52DC3456-B99B-4424-8EF1-06C92B6D370C"), UserId = Guid.Parse("724CBAF1-97D0-4CDF-A82A-5C68FEE87517") }
                );*/
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
