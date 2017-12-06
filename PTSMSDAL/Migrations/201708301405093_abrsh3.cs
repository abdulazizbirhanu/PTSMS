namespace PTSMSDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class abrsh3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EVALUATIONITEM", "sequenceNo", c => c.Int(nullable: false));
            AddColumn("dbo.REL_TRAINEEEVALUATIONCATEGORY", "sequenceNo", c => c.Int(nullable: false));
            AddColumn("dbo.REL_TRAINEEEVALUATIONITEM", "sequenceNo", c => c.Int(nullable: false));
            DropColumn("dbo.EVALUATIONTEMPLATE", "sequenceNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EVALUATIONTEMPLATE", "sequenceNo", c => c.Int(nullable: false));
            DropColumn("dbo.REL_TRAINEEEVALUATIONITEM", "sequenceNo");
            DropColumn("dbo.REL_TRAINEEEVALUATIONCATEGORY", "sequenceNo");
            DropColumn("dbo.EVALUATIONITEM", "sequenceNo");
        }
    }
}
