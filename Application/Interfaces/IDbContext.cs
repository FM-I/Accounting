using Domain.Entity.Handbooks;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IDbContext
    {
        DbSet<Nomenclature> Nomenclatures { get; set; }
        DbSet<BankAccount> BankAccounts { get; set; }

    }
}
