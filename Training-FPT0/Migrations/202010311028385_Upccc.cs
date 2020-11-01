namespace Training_FPT0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upccc : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TrainerTopics", "TrainerId", "dbo.AspNetUsers");
            DropIndex("dbo.TrainerTopics", new[] { "TrainerId" });
            AlterColumn("dbo.TrainerTopics", "TrainerId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.TrainerTopics", "TrainerId");
            AddForeignKey("dbo.TrainerTopics", "TrainerId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainerTopics", "TrainerId", "dbo.AspNetUsers");
            DropIndex("dbo.TrainerTopics", new[] { "TrainerId" });
            AlterColumn("dbo.TrainerTopics", "TrainerId", c => c.String(maxLength: 128));
            CreateIndex("dbo.TrainerTopics", "TrainerId");
            AddForeignKey("dbo.TrainerTopics", "TrainerId", "dbo.AspNetUsers", "Id");
        }
    }
}
