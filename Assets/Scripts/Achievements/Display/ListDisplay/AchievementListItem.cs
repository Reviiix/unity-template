using Abstract;
using Abstract.Interfaces;
using UserInterface.MainMenus.StageSelection;

namespace Achievements.Display.ListDisplay
{
    public class AchievementListItem : AchievementItemBase, IListItem
    {
        public void RemoveClassFromObject()
        {
            Destroy(GetComponent<StageListItem>());
        }
    }
}