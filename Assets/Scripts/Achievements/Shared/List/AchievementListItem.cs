using Abstract.Interfaces;

namespace Achievements.Display.ListDisplay
{
    /// <summary>
    /// This class is for achievement list items. it is shared by dynamic and permanent achievements.
    /// </summary>
    public class AchievementListItem : AchievementItemBase, IListItem
    {
        public void RemoveClassFromObject()
        {
            Destroy(GetComponent<AchievementListItem>());
        }
    }
}