namespace DynamicTypeDemo
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using DynamicTypeDemo.Template;

    public partial class Model2 : DbContext
    {
        public Model2()
            : base("name=Model1")
        {
            Database.Log = (log) => { System.Diagnostics.Debug.WriteLine(log); };
        }

        //public virtual DbSet<T_CM_PRODUCT> T_CM_PRODUCT { get; set; }
        public virtual DbSet<TableTemplate> TableTemplates { get; set; }
        public virtual DbSet<TableTemplateField> TableTemplateFields { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableTemplate>().HasMany(t => t.Fields)
                .WithOptional()                
                .HasForeignKey(t=>t.TableTemplateId);



        }
    }
}
