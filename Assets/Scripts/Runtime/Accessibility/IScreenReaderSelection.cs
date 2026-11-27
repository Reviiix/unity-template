using UnityEngine.UI;

namespace UnityTemplate.Accessibility
{
    /// <summary>Tells the screen reader which of a group's controls is selected, such as the current tab or segment.</summary>
    public interface IScreenReaderSelection
    {
        bool IsSelected(Selectable control);
    }
}
