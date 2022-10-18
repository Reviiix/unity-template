using System;
using UnityEngine;

namespace Credits
{
    public static class CreditsManager
    {
        private static readonly CreditManager Credits  = new CreditManager();
        private static readonly CreditManager PremiumCredits = new CreditManager();
        public static Action<long, long> OnCreditsChanged;
        public static Action<long, long> OnPremiumCreditsChanged;

        public static void Initialise()
        {
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
        }

        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            Credits.Initialise(saveData.Credits);
            PremiumCredits.Initialise(saveData.PremiumCredits);
        }
        
        public static void ChangeCredits(Currency currency, long amount)
        {
            CreditManager credits;
            long originalAmount;
            long newAmountAmount;
            switch (currency)
            {
                case Currency.Credits:
                    credits = Credits;
                    originalAmount = credits.CurrentCredits;
                    credits.ChangeCredits(amount);
                    newAmountAmount = credits.CurrentCredits;
                    OnCreditsChanged?.Invoke(originalAmount, newAmountAmount);
                    break;
                case Currency.PremiumCredits:
                    credits = PremiumCredits;
                    originalAmount = credits.CurrentCredits;
                    credits.ChangeCredits(amount);
                    newAmountAmount = credits.CurrentCredits;
                    OnPremiumCreditsChanged?.Invoke(originalAmount, newAmountAmount);
                    break;
                default:
                    Debug.LogError(currency + " is not accounted for in this switch statement!");
                    break;
            }
            SaveSystem.Save();
        }

        public static long ReturnCredits(Currency currency)
        {
            switch (currency)
            {
                case Currency.Credits:
                    return Credits.CurrentCredits;
                case Currency.PremiumCredits:
                    return PremiumCredits.CurrentCredits;
                default:
                    Debug.LogError(currency + " is not accounted for in this switch statement!");
                    return 0;
            }
        }
        
        public enum Currency
        {
            Credits,
            PremiumCredits
        }
            
        private class CreditManager
        {
            private const int StartingCredits = 0;
            public long CurrentCredits { get; private set; } = StartingCredits;
            public void Initialise(long credits) => CurrentCredits = credits;
            public void ChangeCredits(long amount) => CurrentCredits += amount;
        }
    }
    
}
