using Application.Controllers;
using Application.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Informations;
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
var baseUnit = new Unit() { Name = "KG." };
await controllerHd.AddOrUpdateAsync<Unit>(baseUnit, false);

var nm = new Nomenclature() { Name = "Apple", Arcticle = "AP",  BaseUnit = baseUnit };
await controllerHd.AddOrUpdateAsync<Nomenclature>(nm);
var typePrice = new TypePrice() { Name = "vvv" };
await controllerHd.AddOrUpdateAsync<TypePrice>(typePrice);


var cr = new InformationRegisterController(db);
await cr.AddOrUpdateAsync<Price>(new Price() { Nomenclature = nm, TypePrice = typePrice, Value = 100, Date = DateTime.Now });

var res = cr.GetLastDateData<Price>(DateTime.Now, p => p.NomenclatureId == nm.Id);

await cr.AddOrUpdateAsync<Price>(new Price() { Nomenclature = nm, TypePrice = typePrice, Value = 120, Date = DateTime.Now });

res = cr.GetLastDateData<Price>(DateTime.Now);

var list = cr.GetListData<Price>();

await cr.DeleteAsync<Price>(p => p.NomenclatureId == list.First().NomenclatureId);

return 0;

var products = controllerHd.GetHandbooks<Nomenclature>();

var order = new ClientOrder() { Date = DateTime.Now };

var unit = new Unit() { Name = "шт.", Coefficient = 1 };
var org = new Organization() { Name = "osn" };
var cur = new Currency() { Name = "osn" };
var client = new Client() { Name = "lva" };

await controllerHd.AddOrUpdateAsync<Unit>(unit, false);
await controllerHd.AddOrUpdateAsync<Client>(client, false);
await controllerHd.AddOrUpdateAsync<Organization>(org, false);
await controllerHd.AddOrUpdateAsync<Currency>(cur);

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

await controllerDoc.AddOrUpdateAsync<ClientOrder>(order);

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

