using Domain.Entity;
using Domain.Entity.Documents;
using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Entity.Registers.Informations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.Metadata;

namespace BL.Interfaces
{
    public interface IDbContext
    {
        DbSet<Bank> Banks { get; set; }
        DbSet<BankAccount> BankAccounts { get; set; }
        DbSet<CashBox> CashBoxes { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<Contact> Contacts { get; set; }
        DbSet<Nomenclature> Nomenclatures { get; set; }
        DbSet<Organization> Organizations { get; set; }
        DbSet<TypePrice> TypePrices { get; set; }
        DbSet<Unit> Units { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Warehouse> Warehouses { get; set; }
        DbSet<Currency> Currencies { get; set; }
        DbSet<ClientOrderProduct> ClientOrderProducts { get; set; }
        DbSet<ProviderOrderProduct> ProviderOrderProducts { get; set; }
        DbSet<PurchaceInvoiceProduct> PurchaceInvoiceProducts { get; set; }
        DbSet<SalesInvoiceProduct> SalesInvoiceProducts { get; set; }
        DbSet<ClientOrder> ClientOrders { get; set; }
        DbSet<ProviderOrder> ProviderOrders { get; set; }
        DbSet<SaleInvoice> SalesInvoices { get; set; }
        DbSet<InCashOrder> InCashOrders { get; set; }
        DbSet<InBankAccontOrder> InBankAccontOrders { get; set; }
        DbSet<OutCashOrder> OutCashOrders { get; set; }
        DbSet<OutBankAccontOrder> OutBankAccontOrders { get; set; }
        DbSet<PurchaceInvoice> PurchaceInvoices { get; set; }
        DbSet<Barcode> Barcodes { get; set; }
        DbSet<ClientContact> ClientsContacts { get; set; }
        DbSet<Price> Prices { get; set; }
        DbSet<ExchangesRate> ExchangesRates { get; set; }
        DbSet<Leftover> Leftovers { get; set; }
        DbSet<ClientsDebt> ClientsDebts { get; set; }
        DbSet<ProvidersDebt> ProvidersDebts { get; set; }
        DbSet<Sale> Sales { get; set; }
        public DatabaseFacade Database { get; }

        public ChangeTracker ChangeTracker { get; }

        public event EventHandler<SavedChangesEventArgs>? SavedChanges;

        public EntityEntry Entry(object entity);
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        public EntityEntry Update(object entity);
        public EntityEntry Add(object entity);
        public EntityEntry Remove(object entity);
        public void UpdateRange(IEnumerable<object> entities);
        public void RemoveRange(IEnumerable<object> entities);
        public DbSet<T> GetPropertyData<T>() where T : class;
        public IQueryable<T> IncludeVirtualProperty<T>(IQueryable<T> data) where T : class;
    }
}
