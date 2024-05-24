using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Domain.Interfaces;
using Domain.Models;

namespace Domain.Entity.Documents
{
    public class InBankAccontOrder : Document
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
            Type type = typeof(ClientsDebt);

            if(Operation == TypePayment.Client)
            {
                debts.Add(
            
                    new ClientsDebt()
                    {
                        Client = Client,
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

                    new ProvidersDebt()
                    {
                        Provider = Client,
                        Organization = Organization,
                        Date = DateTime.Now,
                        TypeMove = TypeAccumulationRegisterMove.INCOMING,
                        Value = Summa * (decimal)CurrencyRate
                    }
                );

                type = typeof(ProvidersDebt);
            }

            moves.Add(type, debts);

            return moves;
        }

        public override void FillWith(Document document)
        {
            if (document is ClientOrder)
            {
                var data = (ClientOrder)document;
                Summa = data.Summa;
                Currency = data.Currency;
                CurrencyRate = data.CurrencyRate;
                Operation = TypePayment.Client;
            }
            else if (document is ProviderOrder)
            {
                var data = (ProviderOrder)document;
                Summa = data.Summa;
                Currency = data.Currency;
                CurrencyRate = data.CurrencyRate;
                Operation = TypePayment.Provider;
            }

            Client = document.Client;
            Organization = document.Organization;
        }

        public override Document DeepCopy()
        {
            InBankAccontOrder document = (InBankAccontOrder)MemberwiseClone();

            document.Id = Guid.Empty;
            document.Number = "";
            document.Date = DateTime.Now;
            document.DeleteMark = false;

            return document;
        }

        public override DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (Client == null)
                result.Properties.Add("Контрагент");

            if (Organization == null)
                result.Properties.Add("Організація");

            if (Account == null)
                result.Properties.Add("Каса");

            if (Currency == null)
                result.Properties.Add("Валюта");

            if (Operation == default)
                result.Properties.Add("Тип операції");

            return result;
        }
    }
}
