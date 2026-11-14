namespace UnityTemplate.Core
{
    /// <summary>Implemented by services that persist state: SaveService asks each one to read from and write to the shared SaveData.</summary>
    public interface ISaveable
    {
        /// <summary>Restores this service's state from loaded (or new-game) data.</summary>
        void Load(SaveData data);

        /// <summary>Writes this service's current state into the data about to be saved.</summary>
        void Save(SaveData data);
    }
}
