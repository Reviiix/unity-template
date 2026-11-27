using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Theming;

namespace UnityTemplate.UserInterface.Kit
{
    /// <summary>The kit's segmented control: a row of buttons on a muted track where exactly one segment is selected (raised, primary label).</summary>
    public class SegmentedControl : MonoBehaviour
    {
        [SerializeField] private Button[] segments;
        [SerializeField] private ThemeColour selectedSegment = ThemeColour.Raised;
        [SerializeField] private ThemeColour selectedLabel = ThemeColour.TextPrimary;
        [SerializeField] private ThemeColour unselectedLabel = ThemeColour.TextSecondary;
        private TMP_Text[] labels;
        private ThemeService theme;

        public int SelectedIndex { get; private set; }
        public event Action<int> SelectionChanged;

        /// <summary>Caches the labels and wires each segment's tap.</summary>
        private void Awake()
        {
            labels = new TMP_Text[segments.Length];
            for (var i = 0; i < segments.Length; i++)
            {
                var index = i;
                labels[i] = segments[i].GetComponentInChildren<TMP_Text>(true);
                segments[i].onClick.AddListener(() => Select(index));
            }
        }

        /// <summary>Shows the selection and recolours when the theme changes.</summary>
        private void OnEnable()
        {
            if (ServiceLocator.TryGet(out theme)) theme.Changed += Recolour;
            Recolour();
        }

        /// <summary>Stops following theme changes.</summary>
        private void OnDisable()
        {
            if (theme != null) theme.Changed -= Recolour;
        }

        /// <summary>Selects a segment without raising SelectionChanged, e.g. to mirror a saved setting.</summary>
        public void SetSelectedWithoutNotify(int index)
        {
            SelectedIndex = Mathf.Clamp(index, 0, segments.Length - 1);
            Recolour();
        }

        /// <summary>Selects a tapped segment and raises SelectionChanged if it changed.</summary>
        private void Select(int index)
        {
            if (index == SelectedIndex) return;
            SetSelectedWithoutNotify(index);
            SelectionChanged?.Invoke(SelectedIndex);
        }

        /// <summary>Colours every segment and label for the selection and the current theme.</summary>
        private void Recolour()
        {
            if (theme == null || labels == null) return;
            for (var i = 0; i < segments.Length; i++)
            {
                var isSelected = i == SelectedIndex;
                segments[i].image.color = isSelected ? theme.Get(selectedSegment) : Color.clear;
                labels[i].color = theme.Get(isSelected ? selectedLabel : unselectedLabel);
            }
        }
    }
}
