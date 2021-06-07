namespace MostlyPureFunctions.DateTime
{
    public static class DateTimeOrdinals
    {
        public enum Days { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }
        
        public enum Months { January, February, March, April, May, June, July, August, September, October, November, December }

        public static string ConvertToOrdinalString(int number)
        {
            if (number <= 0) return number.ToString();

            return (number % 100) switch
            {
                11 => number + "th",
                12 => number + "th",
                13 => number + "th",
                _ => (number % 10) switch
                {
                    1 => number + "st",
                    2 => number + "nd",
                    3 => number + "rd",
                    _ => number + "th"
                }
            };
        }
    }
}
