namespace PTSMSDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class abrsh2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EVALUATIONCATEGORY", "sequenceNo", c => c.Int(nullable: false));
            AddColumn("dbo.EVALUATIONTEMPLATE", "sequenceNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EVALUATIONTEMPLATE", "sequenceNo");
            DropColumn("dbo.EVALUATIONCATEGORY", "sequenceNo");
        }
    }
}
