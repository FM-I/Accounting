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

foreach (var item in products)
{
    docProduct.Add(new PurchaceInvoiceProduct() { Nomenclature = item, Price = 10, Quantity = 10, Summa = 20 , Unit = baseUnit });
}

var doc = new PurchaceInvoice()
{
    Client = client,
    Organization = organization,
    Date = DateTime.Now,
    Warehouse = warehouse,
    Products = docProduct
};

var ctr = new AccumulationRegisterController(db);

await ctr.AddOrUpdateAsync(new Leftover() { Date = DateTime.Now, Nomenclature = products.First(), Warehouse = warehouse, Value = 10, TypeMove = Domain.Enum.TypeAccumulationRegisterMove.INCOMING, DocumentId = doc.Id});

var data = ctr.GetListData<Leftover>(s => products.Contains(s.Nomenclature) && s.Warehouse == warehouse);
var leftovers = ctr.GetLeftoverList(data, g => new { g.Nomenclature, g.Warehouse }, s => new { Nomenclature = s.Key.Nomenclature, Value = s.Sum(selector => selector.Value) });

await controllerDoc.AddOrUpdateAsync(doc);

return 0;

