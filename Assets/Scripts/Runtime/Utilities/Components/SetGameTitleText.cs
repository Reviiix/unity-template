using TMPro;
using UnityEngine;

namespace UnityTemplate.Utilities
{
    /// <summary>
    /// This class gets the product name from Application (set in Player Settings) and displays it in the required component, TMP_Text.
    /// Can choose to capitalise.
    /// The class will then remove itself as its no longer needed.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class SetGameTitleText : MonoBehaviour
    {
        [SerializeField] private bool capitalise;
        
        private void Awake()
        {
            GetComponent<TMP_Text>().text = capitalise ? Application.productName.ToUpper() : Application.productName;
            Destroy(this);
        }
    }
}
