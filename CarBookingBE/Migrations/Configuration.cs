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
            /*context.Roles.AddOrUpdate(
                new CarBookingTest.Models.Role { Title = "ADMIN" },
                new CarBookingTest.Models.Role { Title = "ADMINISTRATIVE" },
                new CarBookingTest.Models.Role { Title = "APPROVER" },
                new CarBookingTest.Models.Role { Title = "EMPLOYEE" },
                new CarBookingTest.Models.Role { Title = "SECURITY" }
            );

            context.Departments.AddOrUpdate(
                new CarBookingTest.Models.Department { Code = "1", IsDeleted = false, Name = "IT" },
                new CarBookingTest.Models.Department { Code = "1", IsDeleted = false, Name = "Sales" },
                new CarBookingTest.Models.Department { Code = "1", IsDeleted = false, Name = "Marketing" },
                new CarBookingTest.Models.Department { Code = "1", IsDeleted = false, Name = "HR" }
            );
            //  This method will be called after migrating to the latest version.
            context.Users.AddOrUpdate(
                new CarBookingTest.Models.Account { Username = "admin0001", Password = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=", Email = "admin0001@gmail.com", EmployeeNumber = "AD0001", IsDeleted = false, Sex = true, Created = DateTime.Now },
                new CarBookingTest.Models.Account { Username = "admin0002", Password = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=", Email = "admin0002@gmail.com", EmployeeNumber = "AD0002", IsDeleted = false, Sex = true, Created = DateTime.Now },
                new CarBookingTest.Models.Account { Username = "admin0003", Password = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=", Email = "admin0003@gmail.com", EmployeeNumber = "AD0003", IsDeleted = false, Sex = true, Created = DateTime.Now }
            );
            context.Rotations.AddOrUpdate(
                new Models.Rotation { Type = "City" },
                new Models.Rotation { Type = "Province" },
                new Models.Rotation { Type = "Weekend" },
                new Models.Rotation { Type = "RnD" },
                new Models.Rotation { Type = "Letter" }
            );*/

            context.RequestShares.AddOrUpdate(
                new CarBookingTest.Models.RequestShare { RequestId = Guid.Parse("3e93a300-f948-4a1d-8012-174aaa6c664b"), UserId = Guid.Parse("7320218c-9894-4b6f-a8ff-754fc7de0445")},
                new CarBookingTest.Models.RequestShare { RequestId = Guid.Parse("badb7cdd-fa31-4330-906a-251e650918db"), UserId = Guid.Parse("7320218c-9894-4b6f-a8ff-754fc7de0445")}
                );

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
