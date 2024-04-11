using Domain.Entity.Handbooks;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

using (var db = new AppDbContext())
{
    db.Database.Migrate();
}
