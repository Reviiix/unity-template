using TMPro;
using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(TMP_Text))]
public class SetGameTitleText : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TMP_Text>().text = PlayerSettings.productName.ToUpper();
        Destroy(GetComponent<SetGameTitleText>());
    }
}
