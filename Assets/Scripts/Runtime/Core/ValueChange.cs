namespace UnityTemplate.Core
{
    /// <summary>A value before and after a change, passed with change events so listeners can animate between the two.</summary>
    public readonly struct ValueChange
    {
        public long OldValue { get; }
        public long NewValue { get; }

        /// <summary>Creates a change from one value to another.</summary>
        public ValueChange(long oldValue, long newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
