namespace MVCDemo.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelDB : DbContext
    {
        public ModelDB()
            : base("name=ModelDB")
        {
        }

        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        //public virtual DbSet<Store> Stores { get; set; }
        
        //public virtual DbSet<Webpage> Webpages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Store>()
            //    .Property(e => e.StoreName)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.Webpages)
            //    .WithMany(e => e.Stores)
            //    .Map(m => m.ToTable("WebpageStore").MapLeftKey("StoreId").MapRightKey("WebpageId"));

            //modelBuilder.Entity<Webpage>()
            //    .Property(e => e.WebpageName)
            //    .IsUnicode(false);
        }
    }
}
