using TMPro;
using UnityEngine;

namespace UnityTemplate.Utilities
{
    /// <summary>
    /// This class gets the bundle version number from Application (set in Player Settings) and displays it in the required component, TMP_Text
    /// Can choose to capitalise.
    /// The class will then remove itself as its no longer needed
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class SetVersionNumberText : MonoBehaviour
    {
        [SerializeField] private bool capitalise;
        
        private void Awake()
        {
            GetComponent<TMP_Text>().text = capitalise ? "V" + Application.version + "." : "v" + Application.version + ".";
            Destroy(this);
        }
    }
}

