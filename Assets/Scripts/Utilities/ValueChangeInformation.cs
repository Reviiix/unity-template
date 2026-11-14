namespace Utilities
{
    /// <summary>
    /// This ValueChangeInformation struct is just an alternative to using key value pairs or tuples or something that give greater control/descriptions of the data
    /// </summary>
    public readonly struct ValueChangeInformation
    {
        public long OldValue { get; }
        public  long NewValue { get; }

        public ValueChangeInformation(long oldValue, long newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}