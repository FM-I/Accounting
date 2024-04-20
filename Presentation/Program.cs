using Application.Controllers;
using Application.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddSingleton<IDbContext, AppDbContext>()
    .AddSingleton<IHandbookController, HandbookController>()
    .AddSingleton<IDocumentController, DocumentController>()
    .BuildServiceProvider();

var db = services.GetRequiredService<IDbContext>();

var controllerDoc = services.GetRequiredService<IDocumentController>();
var controllerHd = services.GetRequiredService<IHandbookController>();

var docs = controllerDoc.GetDocuments<ClientOrder>();
var baseUnit = new UnitClasificator() { Name = "KG." };
await controllerHd.AddOrUpdateHandbookAsync<UnitClasificator>(baseUnit, false);

var nm = new Nomenclature() { Name = "Apple", Arcticle = "AP",  BaseUnit = baseUnit };
await controllerHd.AddOrUpdateHandbookAsync<Nomenclature>(nm);

var products = controllerHd.GetHandbooks<Nomenclature>();

var order = new ClientOrder() { Date = DateTime.Now };

var unit = new Unit() { Name = "шт.", Coefficient = 1 };
var org = new Organization() { Name = "osn" };
var cur = new Currency() { Name = "osn" };
var client = new Client() { Name = "lva" };
var typePrice = new TypePrice() { Name = "vvv" };

await controllerHd.AddOrUpdateHandbookAsync<Unit>(unit, false);
await controllerHd.AddOrUpdateHandbookAsync<Client>(client, false);
await controllerHd.AddOrUpdateHandbookAsync<Organization>(org, false);
await controllerHd.AddOrUpdateHandbookAsync<TypePrice>(typePrice, false);
await controllerHd.AddOrUpdateHandbookAsync<Currency>(cur);

order.Organization = org;
order.Currency = cur;
order.Client = client;
order.Products = new List<ClientOrderProduct>();
order.TypePrice = typePrice;

foreach (var product in products)
{
    order.Products.Add(new ClientOrderProduct()
    {
        Nomenclature = product,
        Price = 10,
        Quantity = 3,
        Summa = 30,
        Unit = unit
    });

}

await controllerDoc.AddOrUpdateDocumentAsync<ClientOrder>(order);

//var h1 = controller.GetHandbooks<Bank>(0, 0);
//Guid h = await controller.AddOrUpdateHandbookAsync(handbook);
//var h2 = controller.GetHandbook<Bank>(h);
//
//handbook.Name = "кваа";
//handbook.Code = "кваа";
//handbook.Id = new Guid("B6982772-31E6-4B42-AA14-B9EC0917A770");
//var а = db.Update(handbook);
//db.SaveChanges();
//var res = db.Banks.First();

//handbook.Code = null;
//db.Update(handbook);
//db.SaveChanges();
//object[] arr = { new Guid("B6982772-31E6-4B42-AA14-B9EC0917A770")}; 
//var res2 = db.FindAsync(handbook.GetType(), arr);
//var b = db.GetType().GetProperty("Banks");



//object j = b.GetValue(db);
//var t = j.GetType();
//var res1 = j.GetType().GetMethod("Find");
//var kva = res1.Invoke(j, [new object[] { new Guid("B6982772-31E6-4B42-AA14-B9EC0917A770") }]);

