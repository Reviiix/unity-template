using Abstract.Interfaces;

namespace Achievements.Display.ListDisplay
{
    public class AchievementListItem : AchievementItemBase, IListItem
    {
        public void RemoveClassFromObject()
        {
            Destroy(GetComponent<AchievementListItem>());
        }
    }
}