namespace PTSMSDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class abrsh1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PERSON", "ShortName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PERSON", "ShortName");
        }
    }
}
