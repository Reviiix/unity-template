// Reads iOS accessibility settings that Unity's AccessibilitySettings doesn't expose; called from SystemAccessibility.cs.
#import <UIKit/UIKit.h>

extern "C" bool UnityTemplateIsReduceMotionEnabled()
{
    return UIAccessibilityIsReduceMotionEnabled();
}

extern "C" bool UnityTemplateIsDarkerSystemColorsEnabled()
{
    return UIAccessibilityDarkerSystemColorsEnabled();
}
