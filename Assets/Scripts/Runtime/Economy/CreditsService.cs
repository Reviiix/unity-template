using System;
using UnityTemplate.Core;

namespace UnityTemplate.Economy
{
    /// <summary>Owns the player's credit and premium credit balances and announces every change.</summary>
    public class CreditsService : ISaveable
    {
        private readonly SaveService saveService;
        private long credits;
        private long premiumCredits;

        public event Action<Currency, ValueChange> BalanceChanged;

        /// <summary>Creates the service; balances are filled in when the save file loads.</summary>
        public CreditsService(SaveService saveService)
        {
            this.saveService = saveService;
        }

        /// <summary>Returns the current balance of a currency.</summary>
        public long GetBalance(Currency currency)
        {
            return currency == Currency.Credits ? credits : premiumCredits;
        }

        /// <summary>Adds (or, with a negative amount, removes) currency, notifies listeners and saves.</summary>
        public void Add(Currency currency, long amount)
        {
            var oldBalance = GetBalance(currency);
            var newBalance = Math.Max(0, oldBalance + amount);
            if (currency == Currency.Credits) credits = newBalance;
            else premiumCredits = newBalance;

            BalanceChanged?.Invoke(currency, new ValueChange(oldBalance, newBalance));
            saveService.Save();
            GameLog.Info($"{currency} changed by {amount}: {oldBalance} -> {newBalance}.");
        }

        /// <summary>Restores both balances from the save file.</summary>
        public void Load(SaveData data)
        {
            credits = data.credits;
            premiumCredits = data.premiumCredits;
        }

        /// <summary>Writes both balances to the save file.</summary>
        public void Save(SaveData data)
        {
            data.credits = credits;
            data.premiumCredits = premiumCredits;
        }
    }
}
