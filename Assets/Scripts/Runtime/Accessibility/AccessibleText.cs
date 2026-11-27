using TMPro;
using UnityEngine;
using UnityTemplate.Core;

namespace UnityTemplate.Accessibility
{
    /// <summary>Applies the player's text size and bold text to one text: it grows up to the chosen scale where its box has room and never shrinks below its designed size. Every user interface text carries one.</summary>
    [RequireComponent(typeof(TMP_Text))]
    public class AccessibleText : MonoBehaviour
    {
        private TMP_Text text;
        private AccessibilityService accessibility;
        private float designedSize;
        private float designedMinimumSize;
        private float designedMaximumSize;
        private bool isDesignedAutoSized;
        private FontStyles designedStyle;
        private bool hasDesign;

        /// <summary>Remembers the text's designed size and style, the first time only.</summary>
        private void Awake()
        {
            RememberDesign();
        }

        /// <summary>Applies the current settings and follows changes.</summary>
        private void OnEnable()
        {
            if (!ServiceLocator.TryGet(out accessibility)) return;
            RememberDesign();
            accessibility.Changed += Apply;
            Apply();
        }

        /// <summary>Stops following changes.</summary>
        private void OnDisable()
        {
            if (accessibility != null) accessibility.Changed -= Apply;
        }

        /// <summary>Caches the designed size and style once, before any setting changes them.</summary>
        private void RememberDesign()
        {
            if (hasDesign) return;
            text = GetComponent<TMP_Text>();
            designedSize = text.fontSize;
            designedMinimumSize = text.fontSizeMin;
            designedMaximumSize = text.fontSizeMax;
            isDesignedAutoSized = text.enableAutoSizing;
            designedStyle = text.fontStyle;
            hasDesign = true;
        }

        /// <summary>Sizes and weights the text for the current settings.</summary>
        private void Apply()
        {
            var scale = accessibility.TextScale;
            if (isDesignedAutoSized)
            {
                text.fontSizeMin = designedMinimumSize;
                text.fontSizeMax = designedMaximumSize * scale;
            }
            else if (scale > 1)
            {
                text.enableAutoSizing = true;
                text.fontSizeMin = designedSize;
                text.fontSizeMax = designedSize * scale;
            }
            else
            {
                text.enableAutoSizing = false;
                text.fontSize = designedSize;
            }
            text.fontStyle = accessibility.IsBoldText ? designedStyle | FontStyles.Bold : designedStyle;
        }
    }
}
