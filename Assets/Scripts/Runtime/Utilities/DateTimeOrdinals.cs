namespace UnityTemplate.Utilities
{
    /// <summary>Formats numbers as English ordinals for dates and anniversaries.</summary>
    public static class DateTimeOrdinals
    {
        /// <summary>Returns 1st, 2nd, 3rd, 4th, 11th, 21st... for positive numbers and the plain number otherwise.</summary>
        public static string ConvertToOrdinalString(int number)
        {
            if (number <= 0) return number.ToString();

            var suffix = (number % 100) switch
            {
                11 or 12 or 13 => "th",
                _ => (number % 10) switch
                {
                    1 => "st",
                    2 => "nd",
                    3 => "rd",
                    _ => "th",
                },
            };
            return number + suffix;
        }
    }
}
