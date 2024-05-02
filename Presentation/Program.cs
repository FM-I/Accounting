using Application.Controllers;
using Application.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Entity.Registers.Informations;
using Domain.Interfaces;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

var services = new ServiceCollection()
    .AddSingleton<IDbContext, AppDbContext>()
    .AddSingleton<IHandbookController, HandbookController>()
    .AddSingleton<IDocumentController, DocumentController>()
    .AddSingleton<IAccumulationRegisterController, AccumulationRegisterController>()
    .BuildServiceProvider();

var db = services.GetRequiredService<IDbContext>();
var controllerDoc = services.GetRequiredService<IDocumentController>();
var controllerHd = services.GetRequiredService<IHandbookController>();

var baseUnit = new Unit() { Name = "KG." };
await controllerHd.AddOrUpdateAsync<Unit>(baseUnit, false);

List<Nomenclature> products = new List<Nomenclature>();
products.Add(new Nomenclature() { Name = "Apple", Arcticle = "AP", BaseUnit = baseUnit });
products.Add(new Nomenclature() { Name = "Table", Arcticle = "TB", BaseUnit = baseUnit });
products.Add(new Nomenclature() { Name = "Door", Arcticle = "DR", BaseUnit = baseUnit });
await controllerHd.AddOrUpdateRangeAsync<Nomenclature>(products);

var warehouse = new Warehouse() {  Name = "warehouse 1" };
await controllerHd.AddOrUpdateAsync(warehouse);

var typePrice = new TypePrice() { Name = "vvv" };
await controllerHd.AddOrUpdateAsync(typePrice);

var organization = new Organization() { Name = "Organization 1" };
await controllerHd.AddOrUpdateAsync(organization);

var client = new Client() { Name = "Client 1", TypeClient = Domain.Enum.TypesClient.Client };
await controllerHd.AddOrUpdateAsync(client);


List<PurchaceInvoiceProduct> docProduct = new List<PurchaceInvoiceProduct>();
List<SalesInvoiceProduct> saleProducts = new List<SalesInvoiceProduct>();

foreach (var item in products)
{
    docProduct.Add(new PurchaceInvoiceProduct() { Nomenclature = item, Price = 10, Quantity = 10, Summa = 20 , Unit = baseUnit });
    saleProducts.Add(new() { Nomenclature = item, Price = 10, Quantity = 5, Summa = 20 , Unit = baseUnit });
}

var PurchaceInvoice = new PurchaceInvoice()
{
    Client = client,
    Organization = organization,
    Date = DateTime.Now,
    Warehouse = warehouse,
    Products = docProduct
};

var res = await controllerDoc.ConductedDoumentAsync(PurchaceInvoice);

var SaleInvoice = new SaleInvoice()
{
    Client = client,
    Organization = organization,
    Date = DateTime.Now,
    Warehouse = warehouse,
    Products = saleProducts
};

var ctr = new AccumulationRegisterController(db);
res = await controllerDoc.ConductedDoumentAsync(SaleInvoice);
var leftovers = ctr.GetListData<Leftover>(s => products.Contains(s.Nomenclature) && s.Warehouse == warehouse);
await controllerDoc.UnConductedDoumentAsync(SaleInvoice);
leftovers = ctr.GetListData<Leftover>(s => products.Contains(s.Nomenclature) && s.Warehouse == warehouse);

var debts = ctr.GetListData<ClientsDebt>(s => s.Client == SaleInvoice.Client);
var sales = ctr.GetListData<Sale>(s => products.Contains(s.Nomenclature) && s.Client == SaleInvoice.Client);
//await ctr.AddOrUpdateAsync(new Leftover() { Date = DateTime.Now, Nomenclature = products.First(), Warehouse = warehouse, Value = 10, TypeMove = Domain.Enum.TypeAccumulationRegisterMove.INCOMING});
//await ctr.AddOrUpdateAsync(new Leftover() { Date = DateTime.Now, Nomenclature = products.First(), Warehouse = warehouse, Value = 1, TypeMove = Domain.Enum.TypeAccumulationRegisterMove.OUTCOMING});




//await controllerDoc.AddOrUpdateAsync(doc);
    

return 0;

