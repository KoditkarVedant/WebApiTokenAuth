namespace WebApiTokenAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ClientId = c.String(nullable: false, maxLength: 128),
                        ClientSecret = c.String(),
                        ApplicationName = c.String(),
                        ApplicationType = c.Int(nullable: false),
                        Active = c.String(),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(),
                        RefreshToken_Id = c.Int(),
                    })
                .PrimaryKey(t => t.ClientId)
                .ForeignKey("dbo.RefreshTokens", t => t.RefreshToken_Id)
                .Index(t => t.RefreshToken_Id);
            
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Token = c.String(),
                        ClientId = c.String(),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiryUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Clients", "RefreshToken_Id", "dbo.RefreshTokens");
            DropIndex("dbo.Clients", new[] { "RefreshToken_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.Clients");
        }
    }
}
