namespace PTSMSDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.REL_TRAINEELESSON", "TimeIN", c => c.String());
            AddColumn("dbo.REL_TRAINEELESSON", "TimeOut", c => c.String());
            AddColumn("dbo.REL_TRAINEELESSON", "FlightTime", c => c.String());
            AddColumn("dbo.REL_TRAINEELESSON", "EvaluationRemark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.REL_TRAINEELESSON", "EvaluationRemark");
            DropColumn("dbo.REL_TRAINEELESSON", "FlightTime");
            DropColumn("dbo.REL_TRAINEELESSON", "TimeOut");
            DropColumn("dbo.REL_TRAINEELESSON", "TimeIN");
        }
    }
}
