namespace PTSMSDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aa1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.REL_TRAINEELESSON", "FlightDate", c => c.String());
            DropColumn("dbo.REL_TRAINEELESSON", "EvaluationRemark");
        }
        
        public override void Down()
        {
            AddColumn("dbo.REL_TRAINEELESSON", "EvaluationRemark", c => c.String());
            DropColumn("dbo.REL_TRAINEELESSON", "FlightDate");
        }
    }
}
