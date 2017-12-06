namespace PTSMSDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payment", "Currency1_Name", "dbo.Currency");
            DropForeignKey("dbo.Student", "StartEnds_StartEndId", "dbo.StartEnd");
            DropForeignKey("dbo.SPoint", "Id", "dbo.Student");
            DropForeignKey("dbo.Student", "RoomNumber", "dbo.Room");
            DropForeignKey("dbo.ReserveRoom", "Number", "dbo.Room");
            DropForeignKey("dbo.Material", "RoomNumber", "dbo.Room");
            DropForeignKey("dbo.Room", "Block1_Name", "dbo.Block");
            DropForeignKey("dbo.Attendance", "Id", "dbo.Student");
            DropIndex("dbo.REL_TRAINEECOURSE", new[] { "TraineeId" });
            DropIndex("dbo.REL_TRAINEECOURSE", "UK_TraineeCourse");
            DropIndex("dbo.Payment", new[] { "Currency1_Name" });
            DropIndex("dbo.StartEnd", new[] { "StartEndId" });
            DropIndex("dbo.SPoint", new[] { "Id" });
            DropIndex("dbo.ReserveRoom", new[] { "Number" });
            DropIndex("dbo.Material", new[] { "RoomNumber" });
            DropIndex("dbo.Room", new[] { "Block1_Name" });
            DropIndex("dbo.Student", new[] { "StartEnds_StartEndId" });
            DropIndex("dbo.Student", new[] { "RoomNumber" });
            DropIndex("dbo.Attendance", new[] { "Id" });
            DropTable("dbo.User");
            DropTable("dbo.Sop");
            DropTable("dbo.Nation");
            DropTable("dbo.Logger");
            DropTable("dbo.Payment");
            DropTable("dbo.Currency");
            DropTable("dbo.StartEnd");
            DropTable("dbo.SPoint");
            DropTable("dbo.ReserveRoom");
            DropTable("dbo.Material");
            DropTable("dbo.Block");
            DropTable("dbo.Room");
            DropTable("dbo.Student");
            DropTable("dbo.Attendance");
            CreateIndex("dbo.REL_TRAINEECOURSE", new[] { "BatchCategoryId", "TraineeId", "CourseId" }, unique: true, name: "UK_TraineeCourse");
        }
    }
}
