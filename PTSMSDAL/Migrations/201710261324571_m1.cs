namespace PTSMSDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1 : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.REL_TRAINEEBATCHCLASS", "TraineeBatchClassId", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
        }
    }
}
