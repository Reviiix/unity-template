using TMPro;
using UnityEngine;

namespace UnityTemplate.Utilities
{
    /// <summary>Shows the version from Player Settings in this text ("v1.0." or "V1.0."), then removes itself.</summary>
    [RequireComponent(typeof(TMP_Text))]
    public class SetVersionNumberText : MonoBehaviour
    {
        [SerializeField] private bool capitalise;

        /// <summary>Writes the version number into the text component once.</summary>
        private void Awake()
        {
            GetComponent<TMP_Text>().text = (capitalise ? "V" : "v") + Application.version + ".";
            Destroy(this);
        }
    }
}
