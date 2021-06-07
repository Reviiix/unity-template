namespace Abstract.Interfaces
{
    public interface IListItem
    {
        //IListItems are MonoBehaviours on prefabs that store references to components. Remove the MonoBehaviour once the list is initialised.
        //The intent of this is to reduce unnecessary MonoBehaviours in the Scene but not have to use GetComponent<T> on prefabs to find references.
        void RemoveClassFromObject();
    }
}
