using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTemplate.Accessibility
{
    /// <summary>Works out what the screen reader says for a control or text: a label source if there is one, else the control's own text, else the text just before it in its row (as in "DARK MODE" before its switch), cleaned of markup and capitals.</summary>
    public static class ScreenReaderLabels
    {
        private static readonly Regex RichTextTag = new("<[^>]+>", RegexOptions.Compiled);
        private static readonly Regex Whitespace = new(@"\s+", RegexOptions.Compiled);

        /// <summary>Returns the spoken label of a control, or an empty string if it has none.</summary>
        public static string Describe(Selectable control)
        {
            var source = control.GetComponent<IScreenReaderLabelSource>();
            if (source != null) return Clean(source.SpokenLabel);
            var ownText = control.GetComponentInChildren<TMP_Text>(true);
            if (ownText != null && Clean(ownText.text).Length > 0) return Clean(ownText.text);
            var rowLabel = RowLabelFor(control.transform);
            return rowLabel != null ? Clean(rowLabel.text) : string.Empty;
        }

        /// <summary>True when a text only names the control after it, so the control speaks it and the text is skipped.</summary>
        public static bool IsRowLabel(TMP_Text text)
        {
            var transform = text.transform;
            var parent = transform.parent;
            if (parent == null || transform.GetSiblingIndex() + 1 >= parent.childCount) return false;
            var next = parent.GetChild(transform.GetSiblingIndex() + 1);
            var control = next.GetComponent<Selectable>();
            return control != null && control.GetComponent<IScreenReaderLabelSource>() == null && next.GetComponentInChildren<TMP_Text>(true) == null;
        }

        /// <summary>Removes rich-text tags, leading and trailing symbols such as ‹ › ×, and extra spaces, and turns ALL-CAPS words into title case so they are read as words.</summary>
        public static string Clean(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return string.Empty;
            var text = Whitespace.Replace(RichTextTag.Replace(raw, " "), " ");
            var start = 0;
            var end = text.Length - 1;
            while (start <= end && IsTrimmed(text[start])) start++;
            while (end >= start && IsTrimmed(text[end])) end--;
            text = start > end ? string.Empty : text.Substring(start, end - start + 1);
            return IsAllCapitals(text) ? CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLowerInvariant()) : text;
        }

        /// <summary>Returns the text just before a control in its row, if that is a text.</summary>
        private static TMP_Text RowLabelFor(Transform control)
        {
            var index = control.GetSiblingIndex();
            return index > 0 && control.parent != null ? control.parent.GetChild(index - 1).GetComponent<TMP_Text>() : null;
        }

        /// <summary>True for spaces and decorative symbols that should not be read out.</summary>
        private static bool IsTrimmed(char character)
        {
            if (char.IsWhiteSpace(character)) return true;
            var category = char.GetUnicodeCategory(character);
            return category is UnicodeCategory.MathSymbol or UnicodeCategory.OtherSymbol or UnicodeCategory.InitialQuotePunctuation or UnicodeCategory.FinalQuotePunctuation;
        }

        /// <summary>True when a text has letters and none of them are lower case.</summary>
        private static bool IsAllCapitals(string text)
        {
            var hasLetter = false;
            foreach (var character in text)
            {
                if (!char.IsLetter(character)) continue;
                if (char.IsLower(character)) return false;
                hasLetter = true;
            }
            return hasLetter;
        }
    }
}
