using Application.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure
{
    public class AppDbContext : DbContext, IDbContext
    {
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<CashBox> CashBoxes { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<TypePrice> TypePrices { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UnitClasificator> UnitsClasificators { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ClientOrderProduct> ClientOrderProducts { get; set; }
        public DbSet<ProviderOrderProduct> ProviderOrderProducts { get; set; }
        public DbSet<PurchaceInvoiceProduct> PurchaceInvoiceProducts { get; set; }
        public DbSet<SalesInvoiceProduct> SalesInvoiceProducts { get; set; }
        public DbSet<ClientOrder> ClientOrders { get; set; }
        public DbSet<ProviderOrder> ProviderOrders { get; set; }
        public DbSet<SaleInvoice> SalesInvoices { get; set; }
        public DbSet<InCashOrder> InCashOrders { get; set; }
        public DbSet<InBankAccontOrder> InBankAccontOrders { get; set; }
        public DbSet<OutCashOrder> OutCashOrders { get; set; }
        public DbSet<OutBankAccontOrder> OutBankAccontOrders { get; set; }
        public DbSet<PurchaceInvoice> PurchaceInvoices { get; set; }

        public AppDbContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Mobile.db");
        }
    }
}
