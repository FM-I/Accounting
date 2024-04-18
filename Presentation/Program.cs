using Application.Controllers;
using Domain.Entity.Handbooks;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

using (var db = new AppDbContext())
{
    db.Database.Migrate();
    
    var controller = new HandbookController(db);

    Nomenclature handbook = new Nomenclature() { Name = "test", Code = "41" };
    handbook.BaseUnit = new UnitClasificator() { Code= "test" , Name = "mdfs"};

    var cop = (Nomenclature)handbook.DeepCopy();

    Console.WriteLine(handbook.BaseUnit.GetHashCode());
    Console.WriteLine(cop.BaseUnit.GetHashCode());

    Console.WriteLine(handbook.Code);
    Console.WriteLine(cop.Code);

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
}


