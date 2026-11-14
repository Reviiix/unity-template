using System.Text;

namespace UnityTemplate.Utilities
{
    /// <summary>Small string helpers shared by the user interface.</summary>
    public static class StringUtilities
    {
        /// <summary>Turns "PascalCaseText" into "Pascal Case Text", e.g. to show enum names as titles.</summary>
        public static string AddSpacesBeforeCapitals(string text)
        {
            var sentence = new StringBuilder(text.Length * 2);
            for (var i = 0; i < text.Length; i++)
            {
                if (i > 0 && char.IsUpper(text[i])) sentence.Append(' ');
                sentence.Append(text[i]);
            }
            return sentence.ToString();
        }
    }
}
