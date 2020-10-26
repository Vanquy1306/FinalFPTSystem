namespace Training_FPT0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpCodex : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.CourseCategories");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CourseCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseCategoryName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
