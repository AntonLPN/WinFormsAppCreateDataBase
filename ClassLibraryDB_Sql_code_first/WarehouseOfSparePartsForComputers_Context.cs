using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ClassLibraryDB_Sql_code_first
{
    public partial class WarehouseOfSparePartsForComputers_Context: DbContext
    {

        public DbSet<PartsPC> Spare_parts_warehouse { get; set; }
      
        public WarehouseOfSparePartsForComputers_Context(DbContextOptions<WarehouseOfSparePartsForComputers_Context> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB; Database = Spare_parts_warehouse; Trusted_Connection = true;");
            }
        }


        public WarehouseOfSparePartsForComputers_Context()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);



    }
   
}
