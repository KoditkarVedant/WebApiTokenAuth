namespace WebApiTokenAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Clients", "Active", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Clients", "Active", c => c.String());
        }
    }
}
