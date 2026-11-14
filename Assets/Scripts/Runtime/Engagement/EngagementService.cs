using System;
using UnityTemplate.Core;

namespace UnityTemplate.Engagement
{
    /// <summary>Tracks app opens, play time, the daily bonus (eligibility, streak, pending wheel result) and first-open anniversaries.</summary>
    public class EngagementService : ISaveable
    {
        public const int DailyBonusCreditsPerStreakDay = 10;
        public const int AnniversaryCreditsPerYear = 100;
        public const int NoPendingWheelSegment = -1;
        private readonly SaveService saveService;
        private readonly Func<double> realtimeClock;
        private TimeSpan savedPlayTime;
        private double sessionSecondsBeforePause;
        private double sessionSegmentStart;
        private bool sessionRunning;
        private DateTime firstOpenDate;
        private int anniversariesClaimed;
        private int lastDailyBonusDay;
        private int dailyBonusStreak;

        public int TimesOpened { get; private set; }
        public bool IsFirstOpen => TimesOpened == 1;
        public TimeSpan TotalPlayTime => savedPlayTime + SessionPlayTime;
        public TimeSpan SessionPlayTime => TimeSpan.FromSeconds(sessionSecondsBeforePause + (sessionRunning ? realtimeClock() - sessionSegmentStart : 0));
        public int PendingWheelSegment { get; private set; } = NoPendingWheelSegment;

        /// <summary>Creates the service with a clock in seconds (Time.realtimeSinceStartupAsDouble in the game); state comes from the save file.</summary>
        public EngagementService(SaveService saveService, Func<double> realtimeClock)
        {
            this.saveService = saveService;
            this.realtimeClock = realtimeClock;
        }

        /// <summary>Counts this launch; the very first launch records the first-open date and starts the daily bonus from tomorrow.</summary>
        public void RecordAppOpen(DateTime today)
        {
            TimesOpened++;
            if (TimesOpened == 1)
            {
                firstOpenDate = today.Date;
                lastDailyBonusDay = ToDayNumber(today);
            }
            saveService.Save();
            GameLog.Info($"App has been opened {TimesOpened} times.");
        }

        /// <summary>Starts counting play time, at launch and whenever the app returns to the foreground.</summary>
        public void ResumeSession()
        {
            if (sessionRunning) return;

            sessionSegmentStart = realtimeClock();
            sessionRunning = true;
        }

        /// <summary>Stops counting play time while the app is in the background.</summary>
        public void PauseSession()
        {
            if (!sessionRunning) return;

            sessionSecondsBeforePause += realtimeClock() - sessionSegmentStart;
            sessionRunning = false;
        }

        /// <summary>True on the first launch of each calendar day until the bonus is claimed; false if the clock was moved back.</summary>
        public bool IsDailyBonusAvailable(DateTime today)
        {
            return ToDayNumber(today) > lastDailyBonusDay;
        }

        /// <summary>Returns the streak the next claim will have: one more than the last claim if that was yesterday, otherwise one.</summary>
        public int GetStreakForClaim(DateTime today)
        {
            return ToDayNumber(today) == lastDailyBonusDay + 1 ? dailyBonusStreak + 1 : 1;
        }

        /// <summary>Returns today's base daily bonus, which grows with the consecutive-day streak.</summary>
        public int GetDailyBonusCredits(DateTime today)
        {
            return DailyBonusCreditsPerStreakDay * GetStreakForClaim(today);
        }

        /// <summary>Stores the wheel result before the spin animation starts, so relaunching mid-spin cannot re-roll it.</summary>
        public void SetPendingWheelSegment(int segment)
        {
            PendingWheelSegment = segment;
            saveService.Save();
        }

        /// <summary>Marks today's bonus as claimed, advances the streak and clears the pending wheel result; returns false if already claimed.</summary>
        public bool ClaimDailyBonus(DateTime today)
        {
            if (!IsDailyBonusAvailable(today)) return false;

            dailyBonusStreak = GetStreakForClaim(today);
            lastDailyBonusDay = ToDayNumber(today);
            PendingWheelSegment = NoPendingWheelSegment;
            saveService.Save();
            return true;
        }

        /// <summary>Returns how many full years have passed since the first open (a 29 February first open counts on 28 February in other years).</summary>
        public int GetYearsSinceFirstOpen(DateTime today)
        {
            if (firstOpenDate == default) return 0;

            var years = today.Year - firstOpenDate.Year;
            return today.Date < GetAnniversaryDate(today.Year) ? years - 1 : years;
        }

        /// <summary>True when an anniversary of the first open has passed and its reward has not been claimed yet.</summary>
        public bool IsAnniversaryAvailable(DateTime today)
        {
            return GetYearsSinceFirstOpen(today) > anniversariesClaimed;
        }

        /// <summary>Returns the anniversary reward, which grows with the number of years.</summary>
        public int GetAnniversaryCredits(DateTime today)
        {
            return AnniversaryCreditsPerYear * Math.Max(1, GetYearsSinceFirstOpen(today));
        }

        /// <summary>Marks every anniversary up to today as claimed; returns false if there was nothing to claim.</summary>
        public bool ClaimAnniversary(DateTime today)
        {
            if (!IsAnniversaryAvailable(today)) return false;

            anniversariesClaimed = GetYearsSinceFirstOpen(today);
            saveService.Save();
            return true;
        }

        /// <summary>Restores engagement state from the save file.</summary>
        public void Load(SaveData data)
        {
            TimesOpened = data.timesOpened;
            savedPlayTime = TimeSpan.FromTicks(Math.Max(0, data.totalPlayTimeTicks));
            sessionSecondsBeforePause = 0;
            sessionSegmentStart = realtimeClock();
            firstOpenDate = data.firstOpenDateTicks > 0 ? new DateTime(data.firstOpenDateTicks) : default;
            anniversariesClaimed = data.anniversariesClaimed;
            lastDailyBonusDay = data.lastDailyBonusDay;
            dailyBonusStreak = data.dailyBonusStreak;
            PendingWheelSegment = data.pendingWheelSegment;
        }

        /// <summary>Writes engagement state, including this session's play time, to the save file.</summary>
        public void Save(SaveData data)
        {
            data.timesOpened = TimesOpened;
            data.totalPlayTimeTicks = TotalPlayTime.Ticks;
            data.firstOpenDateTicks = firstOpenDate.Ticks;
            data.anniversariesClaimed = anniversariesClaimed;
            data.lastDailyBonusDay = lastDailyBonusDay;
            data.dailyBonusStreak = dailyBonusStreak;
            data.pendingWheelSegment = PendingWheelSegment;
        }

        /// <summary>Returns a year's anniversary date, using 28 February for a 29 February first open in non-leap years.</summary>
        private DateTime GetAnniversaryDate(int year)
        {
            var day = Math.Min(firstOpenDate.Day, DateTime.DaysInMonth(year, firstOpenDate.Month));
            return new DateTime(year, firstOpenDate.Month, day);
        }

        /// <summary>Converts a date to a whole day number for comparing calendar days.</summary>
        private static int ToDayNumber(DateTime date)
        {
            return (int)(date.Date.Ticks / TimeSpan.TicksPerDay);
        }
    }
}
