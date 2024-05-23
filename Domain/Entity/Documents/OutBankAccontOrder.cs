using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Domain.Interfaces;
using Domain.Models;

namespace Domain.Entity.Documents
{
    public class OutBankAccontOrder : Document
    {
        public virtual Currency Currency { get; set; }
        public double CurrencyRate { get; set; } = 1;
        public virtual BankAccount Account { get; set; }
        public TypePayment Operation { get; set; }
        public decimal Summa { get; set; }

        public override Dictionary<Type, List<IAccumulationRegister>> GetAccumulationMove()
        {
            Dictionary<Type, List<IAccumulationRegister>> moves = new();

            var debts = new List<IAccumulationRegister>();
            Type typeDebt = typeof(ProvidersDebt);

            if (Operation == TypePayment.Provider)
            {
                debts.Add(
                    new ProvidersDebt()
                    {
                        Provider = Client,
                        Organization = Organization,
                        Date = DateTime.Now,
                        TypeMove = TypeAccumulationRegisterMove.OUTCOMING,
                        Value = Summa * (decimal)CurrencyRate
                    }
                );
            }
            else
            {
                debts.Add(
                    new ClientsDebt()
                    {
                        Client = Client,
                        Organization = Organization,
                        Date = DateTime.Now,
                        TypeMove = TypeAccumulationRegisterMove.INCOMING,
                        Value = Summa * (decimal)CurrencyRate
                    }
                );
                
                typeDebt = typeof(ClientsDebt);
            }
            
            moves.Add(typeDebt, debts);

            return moves;
        }

        public override void FillWith(Document document)
        {
            if (document is ClientOrder)
            {
                Summa = ((ClientOrder)document).Summa;
                Operation = TypePayment.Client;
            }
            else if (document is ProviderOrder)
            {
                Summa = ((ProviderOrder)document).Summa;
                Operation = TypePayment.Provider;
            }

            Client = document.Client;
            Organization = document.Organization;
        }

        public override Document DeepCopy()
        {
            OutBankAccontOrder document = (OutBankAccontOrder)MemberwiseClone();

            document.Id = Guid.Empty;
            document.Number = "";
            document.Date = DateTime.Now;
            document.DeleteMark = false;

            return document;
        }

        public override DataComplectionResult CheckDataComplection()
        {
            throw new NotImplementedException();
        }
    }
}
