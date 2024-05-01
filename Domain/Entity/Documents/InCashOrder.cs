﻿using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Interfaces;

namespace Domain.Entity.Documents
{
    public class InCashOrder : Document
    {
        public Currency Currency { get; set; }
        public double CurrencyRate { get; set; }
        public CashBox CashBox { get; set; }
        public decimal Summa { get; set; }

        public override Dictionary<Type, List<IAccumulationRegister>> GetAccumulationMove()
        {
            Dictionary<Type, List<IAccumulationRegister>> moves = new();

            var debts = new List<IAccumulationRegister>()
            {
                new ClientsDebt()
                {
                    Client = Client,
                    Organization = Organization,
                    Date = DateTime.Now,
                    TypeMove = Enum.TypeAccumulationRegisterMove.OUTCOMING,
                    Value = Summa * (decimal)CurrencyRate
                }
            };

            moves.Add(typeof(ClientsDebt), debts);

            return moves;
        }

    }
}
