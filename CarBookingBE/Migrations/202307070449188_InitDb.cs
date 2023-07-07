namespace CarBookingBE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Department",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ContactInfo = c.String(),
                        Code = c.String(),
                        UnderDepartment = c.Int(),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DepartmentMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UserId = c.Int(),
                        DepartmentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Department", t => t.DepartmentId)
                .ForeignKey("dbo.Account", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Account",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 50),
                        Password = c.String(),
                        Email = c.String(),
                        EmployeeNumber = c.String(maxLength: 30),
                        AvatarPath = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Sex = c.Boolean(),
                        Created = c.DateTime(),
                        Birthday = c.DateTime(),
                        JobTitle = c.String(),
                        Company = c.String(),
                        Unit = c.String(),
                        Function = c.String(),
                        SectionsOrTeam = c.String(),
                        Groups = c.String(),
                        OfficeLocation = c.String(),
                        LineManager = c.String(),
                        BelongToDepartments = c.String(),
                        Rank = c.String(),
                        EmployeeType = c.String(),
                        Rights = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        Nation = c.String(),
                        Phone = c.String(),
                        IdCardNumber = c.String(),
                        DateOfIdCard = c.DateTime(),
                        PlaceOfIdCard = c.String(),
                        HealthInsurance = c.String(),
                        StartingDate = c.DateTime(),
                        StartingDateOfficial = c.DateTime(),
                        LeavingDate = c.DateTime(),
                        StartDateMaternityLeave = c.DateTime(),
                        Note = c.String(),
                        AcademicLevel = c.String(),
                        Qualification = c.String(),
                        BusinessPhone = c.String(),
                        HomePhone = c.String(),
                        PersonalEmail = c.String(),
                        BankName = c.String(),
                        BankBranchNumber = c.String(),
                        BankBranchName = c.String(),
                        BankAccountNumber = c.String(),
                        BankAccountName = c.String(),
                        Street = c.String(),
                        FlatNumber = c.String(),
                        City = c.String(),
                        Province = c.String(),
                        PostalCode = c.String(),
                        Country = c.String(),
                        MartialStatus = c.String(),
                        ContactName = c.String(),
                        Relationship = c.String(),
                        PhoneR = c.String(),
                        StreetR = c.String(),
                        FlatNumberR = c.String(),
                        CityR = c.String(),
                        ProvinceR = c.String(),
                        PostalCodeR = c.String(),
                        CountryR = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Username, unique: true)
                .Index(t => t.EmployeeNumber, unique: true);
            
            CreateTable(
                "dbo.Request",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestCode = c.String(),
                        SenderId = c.Int(),
                        DepartmentId = c.Int(),
                        ReceiverId = c.Int(),
                        Created = c.DateTime(),
                        Mobile = c.String(),
                        CostCenter = c.String(),
                        TotalPassengers = c.Int(nullable: false),
                        UsageFrom = c.DateTime(nullable: false),
                        UsageTo = c.DateTime(nullable: false),
                        PickTime = c.DateTime(nullable: false),
                        PickLocation = c.String(),
                        Destination = c.String(),
                        Reason = c.String(),
                        Share = c.Int(),
                        Note = c.String(),
                        ApplyNote = c.Boolean(),
                        IsDeleted = c.Boolean(nullable: false),
                        Account_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Department", t => t.DepartmentId)
                .ForeignKey("dbo.Account", t => t.ReceiverId)
                .ForeignKey("dbo.Account", t => t.SenderId)
                .ForeignKey("dbo.Account", t => t.Account_Id)
                .Index(t => t.SenderId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ReceiverId)
                .Index(t => t.Account_Id);
            
            CreateTable(
                "dbo.RequestAttachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        RequestId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Request", t => t.RequestId)
                .Index(t => t.RequestId);
            
            CreateTable(
                "dbo.RequestComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Created = c.DateTime(nullable: false),
                        RequestId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Request", t => t.RequestId)
                .Index(t => t.RequestId);
            
            CreateTable(
                "dbo.RequestWorkflow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        Level = c.Int(nullable: false),
                        Status = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RequestId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Request", t => t.RequestId)
                .ForeignKey("dbo.Account", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RequestId);
            
            CreateTable(
                "dbo.RequestShare",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        UserId = c.Int(),
                        RequestId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Request", t => t.RequestId)
                .ForeignKey("dbo.Account", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RequestId);
            
            CreateTable(
                "dbo.AccountRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsDeleted = c.Boolean(nullable: false),
                        UserId = c.Int(),
                        RoleId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Role", t => t.RoleId)
                .ForeignKey("dbo.Account", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountRole", "UserId", "dbo.Account");
            DropForeignKey("dbo.AccountRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.RequestShare", "UserId", "dbo.Account");
            DropForeignKey("dbo.RequestShare", "RequestId", "dbo.Request");
            DropForeignKey("dbo.Request", "Account_Id", "dbo.Account");
            DropForeignKey("dbo.Request", "SenderId", "dbo.Account");
            DropForeignKey("dbo.RequestWorkflow", "UserId", "dbo.Account");
            DropForeignKey("dbo.RequestWorkflow", "RequestId", "dbo.Request");
            DropForeignKey("dbo.RequestComment", "RequestId", "dbo.Request");
            DropForeignKey("dbo.RequestAttachment", "RequestId", "dbo.Request");
            DropForeignKey("dbo.Request", "ReceiverId", "dbo.Account");
            DropForeignKey("dbo.Request", "DepartmentId", "dbo.Department");
            DropForeignKey("dbo.DepartmentMembers", "UserId", "dbo.Account");
            DropForeignKey("dbo.DepartmentMembers", "DepartmentId", "dbo.Department");
            DropIndex("dbo.AccountRole", new[] { "RoleId" });
            DropIndex("dbo.AccountRole", new[] { "UserId" });
            DropIndex("dbo.RequestShare", new[] { "RequestId" });
            DropIndex("dbo.RequestShare", new[] { "UserId" });
            DropIndex("dbo.RequestWorkflow", new[] { "RequestId" });
            DropIndex("dbo.RequestWorkflow", new[] { "UserId" });
            DropIndex("dbo.RequestComment", new[] { "RequestId" });
            DropIndex("dbo.RequestAttachment", new[] { "RequestId" });
            DropIndex("dbo.Request", new[] { "Account_Id" });
            DropIndex("dbo.Request", new[] { "ReceiverId" });
            DropIndex("dbo.Request", new[] { "DepartmentId" });
            DropIndex("dbo.Request", new[] { "SenderId" });
            DropIndex("dbo.Account", new[] { "EmployeeNumber" });
            DropIndex("dbo.Account", new[] { "Username" });
            DropIndex("dbo.DepartmentMembers", new[] { "DepartmentId" });
            DropIndex("dbo.DepartmentMembers", new[] { "UserId" });
            DropTable("dbo.Role");
            DropTable("dbo.AccountRole");
            DropTable("dbo.RequestShare");
            DropTable("dbo.RequestWorkflow");
            DropTable("dbo.RequestComment");
            DropTable("dbo.RequestAttachment");
            DropTable("dbo.Request");
            DropTable("dbo.Account");
            DropTable("dbo.DepartmentMembers");
            DropTable("dbo.Department");
        }
    }
}
