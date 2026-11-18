using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;

namespace UnityTemplate.Theming
{
    /// <summary>Colours the Graphic on its object (Image, RawImage or text) with a ThemeColour and recolours it when the theme changes.</summary>
    [DisallowMultipleComponent]
    public class ThemedGraphic : MonoBehaviour
    {
        [SerializeField] private ThemeColour colour;
        private Graphic graphic;
        private ThemeService theme;

        public ThemeColour Colour => colour;

        /// <summary>Caches the Graphic this component colours.</summary>
        private void Awake()
        {
            graphic = GetComponent<Graphic>();
        }

        /// <summary>Applies the current theme and follows later changes; does nothing before GameInitialiser has registered the theme.</summary>
        private void OnEnable()
        {
            if (graphic == null || !ServiceLocator.TryGet(out theme)) return;
            theme.Changed += Apply;
            Apply();
        }

        /// <summary>Stops following theme changes.</summary>
        private void OnDisable()
        {
            if (theme != null) theme.Changed -= Apply;
        }

        /// <summary>Sets the Graphic to the current theme's colour.</summary>
        private void Apply()
        {
            graphic.color = theme.Get(colour);
        }
    }
}
