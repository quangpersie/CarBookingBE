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
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-001",
                    Note = "Trường hợp Phòng Hành Chính không đủ xe để đáp ứng yêu cầu điều xe của bộ phận, Phòng Hành Chính đề nghị sắp xếp phương tiện khác thay thế (thuê xe ngoài, hoặc dùng thẻ taxi, Grab,...) và chi phí sẽ hạch toán theo bộ phận yêu cầu.",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    SenderId = 1,
                    ReceiverId = 2,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    IsDeleted = false
                },
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-002",
                    Note = "Trường hợp Phòng Hành Chính không đủ xe để đáp ứng yêu cầu điều xe của bộ phận, Phòng Hành Chính đề nghị sắp xếp phương tiện khác thay thế (thuê xe ngoài, hoặc dùng thẻ taxi, Grab,...) và chi phí sẽ hạch toán theo bộ phận yêu cầu.",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    SenderId = 1,
                    ReceiverId = 2,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    IsDeleted = false
                },
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-003",
                    Note = "Trường hợp Phòng Hành Chính không đủ xe để đáp ứng yêu cầu điều xe của bộ phận, Phòng Hành Chính đề nghị sắp xếp phương tiện khác thay thế (thuê xe ngoài, hoặc dùng thẻ taxi, Grab,...) và chi phí sẽ hạch toán theo bộ phận yêu cầu.",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    SenderId = 1,
                    ReceiverId = 2,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    IsDeleted = false
                },
                new CarBookingTest.Models.Request
                {
                    RequestCode = "2023OPS-CAR-0706-004",
                    Note = "Trường hợp Phòng Hành Chính không đủ xe để đáp ứng yêu cầu điều xe của bộ phận, Phòng Hành Chính đề nghị sắp xếp phương tiện khác thay thế (thuê xe ngoài, hoặc dùng thẻ taxi, Grab,...) và chi phí sẽ hạch toán theo bộ phận yêu cầu.",
                    ApplyNote = false,
                    TotalPassengers = 1,
                    SenderId = 1,
                    ReceiverId = 2,
                    UsageFrom = DateTime.Now,
                    UsageTo = DateTime.Now,
                    PickTime = DateTime.Now,
                    IsDeleted = false
                }
                );

            context.RequestComments.AddOrUpdate(
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now, RequestId = 1 },
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now, RequestId = 1 },
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now, RequestId = 1 },
                new CarBookingTest.Models.RequestComment { Content = "hello", IsDeleted = false, Created = DateTime.Now, RequestId = 1 }
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
