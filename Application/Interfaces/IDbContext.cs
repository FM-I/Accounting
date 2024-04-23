using Domain.Entity.Documents;
using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Informations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;

namespace Application.Interfaces
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
        public DbSet<ClientContact> ClientsContacts { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<ExchangesRate> ExchangesRates { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        EntityEntry Update(object entity);
        public DbSet<T> GetPropertyData<T>() where T : class;
    }
}
