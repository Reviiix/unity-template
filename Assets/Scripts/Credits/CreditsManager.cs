using System;
using PureFunctions;
using UnityEngine;

namespace Credits
{
    /// <summary>
    /// This class handles tracking the credit values.
    /// It is not for displaying values. That job belongs to CreditsDisplay.
    /// </summary>
    public static class CreditsManager
    {
        private static readonly CreditTracker Credits  = new CreditTracker();
        private static readonly CreditTracker PremiumCredits = new CreditTracker();
        public static Action<ValueChangeInformation> OnCreditsChanged;
        public static Action<ValueChangeInformation> OnPremiumCreditsChanged;

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
        
        public static void IncrementCredits(Currency currency, long amount)
        {
            var creditsTracker = GetCreditsTracker(currency);
            creditsTracker.IncrementCredits(amount);
            GetIncrementCreditsAction(currency).Invoke(new ValueChangeInformation(creditsTracker.CurrentCredits, creditsTracker.CurrentCredits + amount));
            SaveSystem.Save();
        }
        
        public static long GetCredits(Currency currency)
        {
            switch (currency)
            {
                case Currency.Credits:
                    return Credits.CurrentCredits;
                case Currency.PremiumCredits:
                    return PremiumCredits.CurrentCredits;
                default:
                    Debug.LogError(currency + DebuggingAid.Debugging.IsNotAccountedForInSwitchStatement);
                    return 0;
            }
        }

        private static CreditTracker GetCreditsTracker(Currency currency)
        {
            switch (currency)
            {
                case Currency.Credits:
                    return Credits;
                case Currency.PremiumCredits:
                    return PremiumCredits;
                default:
                    Debug.LogError(currency + DebuggingAid.Debugging.IsNotAccountedForInSwitchStatement);
                    return PremiumCredits;
            }
        }

        private static Action<ValueChangeInformation> GetIncrementCreditsAction(Currency currency)
        {
            switch (currency)
            {
                case Currency.Credits:
                    return OnCreditsChanged;
                case Currency.PremiumCredits:
                    return OnPremiumCreditsChanged;
                default:
                    Debug.LogError(currency + DebuggingAid.Debugging.IsNotAccountedForInSwitchStatement);
                    return OnPremiumCreditsChanged;
            }
        }

        private class CreditTracker
        {
            private const int StartingCredits = 0;
            public long CurrentCredits { get; private set; } = StartingCredits;
            
            public void Initialise(long credits)
            { 
                CurrentCredits = credits;
            }

            public void IncrementCredits(long amount)
            {
                CurrentCredits += amount;
            }
        }
        
        public enum Currency
        {
            Credits,
            PremiumCredits
        }
    }
    
}
