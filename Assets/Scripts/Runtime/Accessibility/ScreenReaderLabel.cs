using System.Text;
using TMPro;
using UnityEngine;

namespace UnityTemplate.Accessibility
{
    /// <summary>What the screen reader says for a control or graphic with no readable text, such as an icon button; on a non-control it stands in for the texts beneath it, and left empty it reads those texts together as one item.</summary>
    public class ScreenReaderLabel : MonoBehaviour, IScreenReaderLabelSource
    {
        [Tooltip("Leave empty to read the texts beneath this object together, e.g. \"1000\" and \"CREDITS\" as one item.")]
        [SerializeField] private string label;
        private readonly StringBuilder joinedTexts = new();

        public string SpokenLabel => string.IsNullOrEmpty(label) ? JoinChildTexts() : label;

        /// <summary>Joins the active texts beneath this object in hierarchy order.</summary>
        private string JoinChildTexts()
        {
            joinedTexts.Clear();
            foreach (var text in GetComponentsInChildren<TMP_Text>())
            {
                if (joinedTexts.Length > 0) joinedTexts.Append(' ');
                joinedTexts.Append(text.text);
            }
            return joinedTexts.ToString();
        }
    }
}
