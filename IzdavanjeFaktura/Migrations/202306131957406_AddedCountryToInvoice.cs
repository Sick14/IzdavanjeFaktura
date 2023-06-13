namespace IzdavanjeFaktura.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCountryToInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "CountryID", c => c.Int(nullable: false));
            CreateIndex("dbo.Invoices", "CountryID");
            AddForeignKey("dbo.Invoices", "CountryID", "dbo.Countries", "CountryID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "CountryID", "dbo.Countries");
            DropIndex("dbo.Invoices", new[] { "CountryID" });
            DropColumn("dbo.Invoices", "CountryID");
        }
    }
}
