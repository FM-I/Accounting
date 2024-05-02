﻿using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Domain.Interfaces;

namespace Domain.Entity.Documents
{
    public class InBankAccontOrder : Document
    {
        public Currency Currency { get; set; }
        public double CurrencyRate { get; set; } = 1;
        public BankAccount Account { get; set; }
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
    }
}
