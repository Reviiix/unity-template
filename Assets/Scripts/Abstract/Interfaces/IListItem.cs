namespace Abstract.Interfaces
{
    /// <summary>
    /// This interface is for items that appear in a list display.
    /// IListItems are MonoBehaviours on prefabs that store references to components. Remove the MonoBehaviour once the list is initialised.
    /// The intent of this is to reduce unnecessary MonoBehaviours in the Scene but not have to use GetComponent<> on prefabs to find references.
    /// </summary>
    public interface IListItem
    {
        void RemoveClassFromObject();
    }
}
