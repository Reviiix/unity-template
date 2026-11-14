using TMPro;
using UnityEngine;

namespace UnityTemplate.Utilities
{
    /// <summary>Shows the product name from Player Settings in this text, optionally in capitals, then removes itself.</summary>
    [RequireComponent(typeof(TMP_Text))]
    public class SetGameTitleText : MonoBehaviour
    {
        [SerializeField] private bool capitalise;

        /// <summary>Writes the product name into the text component once.</summary>
        private void Awake()
        {
            GetComponent<TMP_Text>().text = capitalise ? Application.productName.ToUpper() : Application.productName;
            Destroy(this);
        }
    }
}
