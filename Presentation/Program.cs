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

List <IHandbook> handbooks = new List<IHandbook>();
handbooks.Add(new Nomenclature() { Name = "Apple", Arcticle = "AP", BaseUnit = baseUnit });
handbooks.Add(new Nomenclature() { Name = "Table", Arcticle = "TB", BaseUnit = baseUnit });
handbooks.Add(new Nomenclature() { Name = "Door", Arcticle = "DR", BaseUnit = baseUnit });
await controllerHd.AddOrUpdateRangeAsync<Nomenclature>(handbooks);

var warehouse = new Warehouse() {  Name = "warehouse 1" };
await controllerHd.AddOrUpdateAsync<Warehouse>(warehouse);
var warehouseId = warehouse.Id;

List<IAccumulationRegister> leftovers = new List<IAccumulationRegister>();
foreach (Nomenclature item in handbooks)
{
    for(var i = 0; i < 3; ++i)
    {
        leftovers.Add(new Leftover() { Date = DateTime.Now.AddSeconds(i), Nomenclature = item, Warehouse = warehouse, Value = i + 3});
    }
}




var typePrice = new TypePrice() { Name = "vvv" };
await controllerHd.AddOrUpdateAsync<TypePrice>(typePrice);

var ctr = new AccumulationRegisterController(db);
await ctr.AddOrUpdateRangeAsync(leftovers);

foreach(Leftover leftover in leftovers)
{
    leftover.Value -= 3;
}

await ctr.AddOrUpdateRangeAsync(leftovers);

var cr = new InformationRegisterController(db);

var prices = new List<Price>()
{
    new Price() { Nomenclature = (Nomenclature)handbooks.First(), TypePrice = typePrice, Value = 100, Date = DateTime.Now },
    new Price() { Nomenclature = (Nomenclature)handbooks.First(), TypePrice = typePrice, Value = 150, Date = DateTime.Now.AddSeconds(5) },
    new Price() { Nomenclature = (Nomenclature)handbooks.First(), TypePrice = typePrice, Value = 160, Date = DateTime.Now.AddSeconds(15) },
};

await cr.AddOrUpdateRangeAsync(prices);

return 0;

