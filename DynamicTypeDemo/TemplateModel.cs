namespace DynamicTypeDemo
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using DynamicTypeDemo.Entities;

    public partial class TemplateModel : DbContext
    {
        public TemplateModel()
            : base("name=Model1")
        {
            //Database.Log = (log) => { System.Diagnostics.Debug.WriteLine(log); };
            Database.SetInitializer<TemplateModel>(null);
        }

        //public virtual DbSet<T_CM_PRODUCT> T_CM_PRODUCT { get; set; }
        public virtual DbSet<TableTemplate> TableTemplates { get; set; }
        public virtual DbSet<TableTemplateField> TableTemplateFields { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableTemplate>().HasMany(t => t.Fields)
                .WithRequired()
                .HasForeignKey(t=>t.TableTemplateId);



        }
    }
}
