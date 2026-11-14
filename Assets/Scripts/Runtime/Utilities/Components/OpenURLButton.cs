using UnityEngine;
using UnityEngine.UI;

namespace UnityTemplate.Utilities
{
    /// <summary>Opens a web page when the button on the same GameObject is tapped.</summary>
    [RequireComponent(typeof(Button))]
    public class OpenURLButton : MonoBehaviour
    {
        [SerializeField] private string url;

        /// <summary>Wires the button to open the configured URL.</summary>
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OpenUrl);
        }

        /// <summary>Opens the URL in the device's browser.</summary>
        private void OpenUrl()
        {
            Application.OpenURL(url);
        }
    }
}
