﻿using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class SetVersionNumberText : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TMP_Text>().text = "V" + PlayerSettings.bundleVersion + ".";
        Destroy(GetComponent<SetVersionNumberText>());
    }
}

