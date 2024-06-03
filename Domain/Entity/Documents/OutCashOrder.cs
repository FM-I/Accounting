using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Documents
{
    public class OutCashOrder : Document
    {
        public virtual Currency Currency { get; set; }
        public double CurrencyRate { get; set; } = 1;
        public virtual CashBox CashBox { get; set; }
        public TypePayment Operation { get; set; }
        public decimal Summa { get; set; }

        [ForeignKey(nameof(SaleInvoice))]
        public Guid? SaleInvoiceId { get; set; }
        public SaleInvoice? SaleInvoice { get; set; }

        [ForeignKey(nameof(PurchaceInvoice))]
        public Guid? PurchaceInvoiceId { get; set; }
        public PurchaceInvoice? PurchaceInvoice { get; set; }


        public override Dictionary<Type, List<IAccumulationRegister>> GetAccumulationMove()
        {
            Dictionary<Type, List<IAccumulationRegister>> moves = new();

            var debts = new List<IAccumulationRegister>();
            var cashs = new List<IAccumulationRegister>();
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

            cashs.Add(new CashInCashBox()
            {
                CashBox = CashBox,
                Summa = Summa,
                TypeMove = TypeAccumulationRegisterMove.OUTCOMING,
                Date = DateTime.Now,
                Currency = Currency
            });

            moves.Add(typeDebt, debts);
            moves.Add(typeof(CashInCashBox), cashs);

            return moves;
        }

        public override void FillWith(Document document)
        {
            if (document is SaleInvoice)
            {
                var saleInvoice = (SaleInvoice)document;
                Summa = saleInvoice.Summa;
                Currency = saleInvoice.Currency;
                CurrencyRate = saleInvoice.CurrencyRate;
                Operation = TypePayment.Client;
                SaleInvoiceId = saleInvoice.Id;
                SaleInvoice = saleInvoice;
            }
            else if (document is PurchaceInvoice)
            {
                var purchaceInvoice = (PurchaceInvoice)document;
                Summa = purchaceInvoice.Summa;
                Currency = purchaceInvoice.Currency;
                CurrencyRate = purchaceInvoice.CurrencyRate;
                Operation = TypePayment.Provider;
                PurchaceInvoiceId = purchaceInvoice.Id;
                PurchaceInvoice = purchaceInvoice;
            }

            Client = document.Client;
            Organization = document.Organization;
        }

        public override Document DeepCopy()
        {
            OutCashOrder document = (OutCashOrder)MemberwiseClone();

            document.Id = Guid.Empty;
            document.Number = "";
            document.Date = DateTime.Now;
            document.DeleteMark = false;
            document.Conducted = false;

            return document;
        }

        public override DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (Client == null)
                result.Properties.Add("Контрагент");

            if (Organization == null)
                result.Properties.Add("Організація");

            if (CashBox == null)
                result.Properties.Add("Каса");

            if (Currency == null)
                result.Properties.Add("Валюта");

            if (Summa <= 0)
                result.Properties.Add("Сума");

            return result;
        }
    }
}
