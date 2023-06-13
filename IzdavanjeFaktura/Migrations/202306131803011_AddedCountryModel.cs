namespace IzdavanjeFaktura.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCountryModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        VATPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CountryID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Countries");
        }
    }
}
