using System;
using System.Runtime.InteropServices;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace UnityTemplate.Accessibility
{
    /// <summary>The device's own accessibility settings, read once at start-up; a new save starts from these.</summary>
    public readonly struct SystemAccessibility
    {
        public float FontScale { get; }
        public bool IsBoldText { get; }
        public bool IsHighContrast { get; }
        public bool IsReducedMotion { get; }

        /// <summary>Creates a set of device settings; tests use this to stand in for a device.</summary>
        public SystemAccessibility(float fontScale, bool isBoldText, bool isHighContrast, bool isReducedMotion)
        {
            FontScale = fontScale;
            IsBoldText = isBoldText;
            IsHighContrast = isHighContrast;
            IsReducedMotion = isReducedMotion;
        }

        /// <summary>Reads the device's text size and bold text (Unity's AccessibilitySettings) and its contrast and motion settings (iOS and Android only); anything unreadable falls back to the default.</summary>
        public static SystemAccessibility Read()
        {
            var fontScale = 1f;
            var isBoldText = false;
            var isHighContrast = false;
            var isReducedMotion = false;
            try
            {
                fontScale = UnityEngine.Accessibility.AccessibilitySettings.fontScale;
                isBoldText = UnityEngine.Accessibility.AccessibilitySettings.isBoldTextEnabled;
#if UNITY_IOS && !UNITY_EDITOR
                isHighContrast = UnityTemplateIsDarkerSystemColorsEnabled();
                isReducedMotion = UnityTemplateIsReduceMotionEnabled();
#elif UNITY_ANDROID && !UNITY_EDITOR
                using var resolver = AndroidApplication.currentContext.Call<AndroidJavaObject>("getContentResolver");
                using var globalSettings = new AndroidJavaClass("android.provider.Settings$Global");
                using var secureSettings = new AndroidJavaClass("android.provider.Settings$Secure");
                isReducedMotion = globalSettings.CallStatic<float>("getFloat", resolver, "animator_duration_scale", 1f) == 0;
                isHighContrast = secureSettings.CallStatic<int>("getInt", resolver, "high_text_contrast_enabled", 0) == 1;
#endif
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not read the device's accessibility settings: " + exception.Message);
            }
            return new SystemAccessibility(fontScale > 0 ? fontScale : 1f, isBoldText, isHighContrast, isReducedMotion);
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern bool UnityTemplateIsReduceMotionEnabled();
        [DllImport("__Internal")] private static extern bool UnityTemplateIsDarkerSystemColorsEnabled();
#endif
    }
}
