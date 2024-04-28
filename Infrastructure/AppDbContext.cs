using Application.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Entity.Registers.Informations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
        public DbSet<Barcode> Barcodes { get; set; }
        public DbSet<ClientContact> ClientsContacts { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<ExchangesRate> ExchangesRates { get; set; }
        public DbSet<Leftover> Leftovers { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<Sale> Sales { get; set; }

        public AppDbContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Mobile.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Barcode>().HasKey(x => new { x.NomenclatureId, x.UnitId });
            modelBuilder.Entity<ClientContact>().HasKey(x => new { x.ClientId, x.ContactId });
            modelBuilder.Entity<Price>().HasKey(x => new { x.Date, x.NomenclatureId, x.TypePriceId });
            modelBuilder.Entity<ExchangesRate>().HasKey(x => new { x.Date, x.CurrencyId });
            modelBuilder.Entity<Leftover>().HasKey(x => new { x.Date, x.NomenclatureId, x.WarehouseId, x.DocumentId });
            modelBuilder.Entity<Debt>().HasKey(x => new { x.Date, x.ClientId, x.OrganizationId, x.DocumentId });
            modelBuilder.Entity<Sale>().HasKey(x => new { x.Date, x.NomenclatureId, x.ClientId, x.OrganizationId, x.DocumentId });
        }

        public DbSet<T> GetPropertyData<T>() where T : class
        {
            PropertyInfo? property = GetType().GetProperties().FirstOrDefault(x => x.PropertyType == typeof(DbSet<T>));

            if (property == null)
                throw new Exception("Property not finded");

            object? propValue = property.GetValue(this);

            if (propValue == null)
                throw new Exception("Property value is null");
            
            DbSet<T> data = (DbSet<T>)propValue;

            return data;
        }

        public void IncludeVirtualProperty<T>(IQueryable<T> data) where T : class
        {
            var virtualProperty = typeof(T).GetProperties().Where(p => p.PropertyType.IsClass && p.GetGetMethod().IsVirtual);

            foreach (var item in virtualProperty)
                data = data.Include(item.Name);
        }
    }
}
