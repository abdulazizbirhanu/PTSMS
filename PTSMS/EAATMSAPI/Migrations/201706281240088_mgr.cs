namespace EAATMSAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mgr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TraineeInfo",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Salutation = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        Gender = c.String(),
                        Email = c.String(),
                        CellPhone = c.String(),
                        HomePhone = c.String(),
                        City = c.String(),
                        Country = c.String(),
                        EducationalLevel = c.String(),
                        ApplyingForProgram = c.String(),
                        CertificateType = c.String(),
                        Category = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TraineeInfo");
        }
    }
}
