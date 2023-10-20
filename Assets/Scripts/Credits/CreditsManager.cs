using System;
using DebuggingAid;
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
        private static readonly CreditTracker Credits  = new ();
        private static readonly CreditTracker PremiumCredits = new ();
        public static Action<ValueChangeInformation> OnCreditsChanged;
        public static Action<ValueChangeInformation> OnPremiumCreditsChanged;

        public static void Initialise()
        {
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
        }

        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            Credits.Initialise(saveData.credits);
            PremiumCredits.Initialise(saveData.premiumCredits);
        }
        
        public static void AddCredits(Currency currency, long amount)
        {
            var creditsTracker = GetCreditsTracker(currency);
            var creditsBeforeAddition = creditsTracker.CurrentCredits;
            var creditsAfterAddition = creditsTracker.CurrentCredits + amount;
            creditsTracker.AddCredits(amount);
            GetIncrementCreditsAction(currency).Invoke(new ValueChangeInformation(creditsBeforeAddition, creditsAfterAddition));
            SaveSystem.Save();
            Debugging.DisplayDebugMessage($"{currency} updated by {amount}.\nFrom {creditsBeforeAddition} to {creditsAfterAddition}.");
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
                    Debug.LogError(currency + Debugging.IsNotAccountedForInSwitchStatement);
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
                    Debug.LogError(currency + Debugging.IsNotAccountedForInSwitchStatement);
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
                    Debug.LogError(currency + Debugging.IsNotAccountedForInSwitchStatement);
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

            public void AddCredits(long amount)
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
