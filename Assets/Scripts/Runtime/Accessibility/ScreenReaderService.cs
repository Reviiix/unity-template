using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UI;

namespace UnityTemplate.Accessibility
{
    /// <summary>Exposes what is on screen to VoiceOver and TalkBack while one is on: every visible control and text becomes a node the player can swipe to and double-tap. UserInterfaceManager reports each screen (a menu, or the top popup on its own so focus stays inside it).</summary>
    public sealed class ScreenReaderService : IDisposable
    {
        private const float SliderStep = 0.1f;
        private readonly AccessibilityHierarchy hierarchy = new();
        private readonly List<Transform> screenRoots = new();
        private readonly Dictionary<Selectable, AccessibilityNode> nodesByControl = new();
        private readonly Dictionary<AccessibilityNode, Selectable> controlsByNode = new();
        private readonly List<ScreenEntry> entries = new();
        private readonly List<ScreenEntry> row = new();
        private readonly Vector3[] corners = new Vector3[4];
        private bool isRebuildPending;
        private bool isPendingNewScreen;
        private Selectable pendingFocus;
        private bool isDisposed;

        public bool IsScreenReaderOn => AssistiveSupport.isScreenReaderEnabled;
        public AccessibilityHierarchy Hierarchy => hierarchy;

        /// <summary>Creates the service and follows the screen reader being turned on or off.</summary>
        public ScreenReaderService()
        {
            AssistiveSupport.screenReaderStatusChanged += OnScreenReaderStatusChanged;
        }

        /// <summary>Replaces what is on screen with these roots (read in order) and rebuilds the nodes next frame, once layout has settled.</summary>
        public void ShowScreen(IEnumerable<Transform> roots)
        {
            screenRoots.Clear();
            screenRoots.AddRange(roots);
            RequestRebuild(true, null);
        }

        /// <summary>Has the screen reader read a short message, such as a notice.</summary>
        public void Announce(string message)
        {
            if (IsScreenReaderOn) AssistiveSupport.notificationDispatcher.SendAnnouncement(message);
        }

        /// <summary>Does what a double tap on a node does: presses its button, flips its switch or selects it; returns false if it can't be used.</summary>
        public bool Activate(AccessibilityNode node)
        {
            var control = ControlFor(node);
            if (control == null || !control.IsActive() || !control.IsInteractable()) return false;
            switch (control)
            {
                case Button button:
                    button.onClick.Invoke();
                    break;
                case Toggle toggle:
                    toggle.isOn = !toggle.isOn;
                    break;
                default:
                    control.Select();
                    break;
            }
            RequestRebuild(false, control);
            return true;
        }

        /// <summary>Stops following the screen reader and takes this service's nodes away from it.</summary>
        public void Dispose()
        {
            isDisposed = true;
            AssistiveSupport.screenReaderStatusChanged -= OnScreenReaderStatusChanged;
            if (AssistiveSupport.activeHierarchy == hierarchy) AssistiveSupport.activeHierarchy = null;
        }

        /// <summary>Builds the nodes when the screen reader comes on and removes them when it goes off.</summary>
        private void OnScreenReaderStatusChanged(bool isOn)
        {
            RequestRebuild(true, null);
        }

        /// <summary>Rebuilds once next frame however many changes ask for it; a new screen moves focus to its first node, otherwise focus stays on the given control.</summary>
        private async void RequestRebuild(bool isNewScreen, Selectable focus)
        {
            isPendingNewScreen |= isNewScreen;
            if (focus != null) pendingFocus = focus;
            if (isRebuildPending) return;
            isRebuildPending = true;
            await Awaitable.NextFrameAsync();
            isRebuildPending = false;
            if (isDisposed) return;
            Rebuild(isPendingNewScreen, pendingFocus);
            isPendingNewScreen = false;
            pendingFocus = null;
        }

        /// <summary>Recreates every node for the current screen and tells the screen reader what changed.</summary>
        private void Rebuild(bool isNewScreen, Selectable focus)
        {
            if (!IsScreenReaderOn)
            {
                if (AssistiveSupport.activeHierarchy == hierarchy) AssistiveSupport.activeHierarchy = null;
                return;
            }
            hierarchy.Clear();
            nodesByControl.Clear();
            controlsByNode.Clear();
            entries.Clear();
            foreach (var root in screenRoots)
            {
                if (root != null && IsOnShownCanvas(root)) CollectEntries(root);
            }
            AddNodesInReadingOrder();
            if (AssistiveSupport.activeHierarchy != hierarchy) AssistiveSupport.activeHierarchy = hierarchy;

            var focusNode = focus != null && nodesByControl.TryGetValue(focus, out var node) ? node : null;
            if (isNewScreen && focusNode == null) AssistiveSupport.notificationDispatcher.SendScreenChanged(null);
            else AssistiveSupport.notificationDispatcher.SendLayoutChanged(focusNode);
        }

        /// <summary>Collects what an object and its children show: a control or a label source is one entry that speaks for its children; a text is an entry unless it only names the control after it.</summary>
        private void CollectEntries(Transform target)
        {
            if (!target.gameObject.activeInHierarchy) return;
            var control = target.GetComponent<Selectable>();
            if (control != null && control.enabled)
            {
                entries.Add(new ScreenEntry((RectTransform)target, control, null, false));
                return;
            }
            var source = target.GetComponent<IScreenReaderLabelSource>();
            if (source is Behaviour { enabled: true })
            {
                entries.Add(new ScreenEntry((RectTransform)target, null, () => ScreenReaderLabels.Clean(source.SpokenLabel), false));
                return;
            }
            var text = target.GetComponent<TMP_Text>();
            if (text != null && text.enabled && !ScreenReaderLabels.IsRowLabel(text))
            {
                entries.Add(new ScreenEntry(text.rectTransform, null, () => ScreenReaderLabels.Clean(text.text), target.name.Contains("Title")));
            }
            foreach (Transform child in target) CollectEntries(child);
        }

        /// <summary>Adds the collected entries as nodes in reading order: rows from the top of the screen down, and left to right within a row (entries whose centres sit within half the row's first entry's height).</summary>
        private void AddNodesInReadingOrder()
        {
            foreach (var entry in entries) entry.Measure(corners);
            entries.Sort((first, second) => second.CentreY.CompareTo(first.CentreY));
            var index = 0;
            while (index < entries.Count)
            {
                row.Clear();
                var rowStart = entries[index];
                while (index < entries.Count && rowStart.CentreY - entries[index].CentreY < rowStart.Height * 0.5f) row.Add(entries[index++]);
                row.Sort((first, second) => first.Left.CompareTo(second.Left));
                foreach (var entry in row)
                {
                    if (entry.Control != null) AddControlNode(entry.Control);
                    else AddTextNode(entry.Rect, entry.ReadLabel, entry.IsHeading);
                }
            }
        }

        /// <summary>Adds a node read as plain text (or a heading), skipping texts that are empty.</summary>
        private void AddTextNode(RectTransform rect, Func<string> readLabel, bool isHeading)
        {
            var label = readLabel();
            if (label.Length == 0) return;
            var node = hierarchy.AddNode(label);
            node.role = isHeading ? AccessibilityRole.Header : AccessibilityRole.StaticText;
            node.frameGetter = FrameGetter(rect);
            node.focusChanged += (focused, isFocused) =>
            {
                if (isFocused) focused.label = readLabel();
            };
        }

        /// <summary>Adds a node for a button, switch or slider with its label, role, state and actions.</summary>
        private void AddControlNode(Selectable control)
        {
            var node = hierarchy.AddNode(ScreenReaderLabels.Describe(control));
            nodesByControl[control] = node;
            controlsByNode[node] = control;
            node.frameGetter = FrameGetter((RectTransform)control.transform);
            node.role = control switch
            {
                Toggle => AccessibilityRole.Toggle,
                Slider => AccessibilityRole.Slider,
                TMP_InputField => AccessibilityRole.TextField,
                _ => AccessibilityRole.Button,
            };
            ShowState(node, control);
            node.invoked += () => Activate(node);
            node.focusChanged += (focused, isFocused) =>
            {
                if (!isFocused) return;
                focused.label = ScreenReaderLabels.Describe(control);
                ShowState(focused, control);
            };
            if (control is Slider slider)
            {
                node.incremented += () => StepSlider(node, slider, 1);
                node.decremented += () => StepSlider(node, slider, -1);
            }
        }

        /// <summary>Sets a node's state (disabled, selected) and, for sliders, its value.</summary>
        private static void ShowState(AccessibilityNode node, Selectable control)
        {
            var state = control.IsInteractable() ? AccessibilityState.None : AccessibilityState.Disabled;
            var group = control.GetComponentInParent<IScreenReaderSelection>();
            if ((control is Toggle { isOn: true }) || (group != null && group.IsSelected(control))) state |= AccessibilityState.Selected;
            node.state = state;
            if (control is Slider slider) node.value = Mathf.RoundToInt(slider.normalizedValue * 100) + "%";
        }

        /// <summary>Moves a slider a tenth of its range up or down, as a swipe up or down on it does.</summary>
        private static void StepSlider(AccessibilityNode node, Slider slider, int direction)
        {
            if (!slider.IsActive() || !slider.IsInteractable()) return;
            slider.value += direction * SliderStep * (slider.maxValue - slider.minValue);
            ShowState(node, slider);
        }

        /// <summary>Returns the control a node was made for, if it was made for one.</summary>
        private Selectable ControlFor(AccessibilityNode node)
        {
            return node != null && controlsByNode.TryGetValue(node, out var control) ? control : null;
        }

        /// <summary>True when the root's canvas is drawing, so hidden menus and closed popups are skipped.</summary>
        private static bool IsOnShownCanvas(Transform root)
        {
            var canvas = root.GetComponentInParent<Canvas>();
            return canvas != null && canvas.rootCanvas.isActiveAndEnabled;
        }

        /// <summary>Returns a frame getter for a rect, with its canvas camera looked up once.</summary>
        private Func<Rect> FrameGetter(RectTransform rect)
        {
            var canvas = rect.GetComponentInParent<Canvas>();
            var camera = canvas == null || canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.rootCanvas.worldCamera;
            return () => ScreenFrame(rect, camera);
        }

        /// <summary>Returns a rect's bounds on screen in pixels with the origin at the top left, as screen readers expect.</summary>
        private Rect ScreenFrame(RectTransform rect, Camera camera)
        {
            if (rect == null) return Rect.zero;
            rect.GetWorldCorners(corners);
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            foreach (var corner in corners)
            {
                var point = RectTransformUtility.WorldToScreenPoint(camera, corner);
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }
            return new Rect(min.x, Screen.height - max.y, max.x - min.x, max.y - min.y);
        }

        /// <summary>One thing to read on screen, with its position in world space for ordering.</summary>
        private sealed class ScreenEntry
        {
            public ScreenEntry(RectTransform rect, Selectable control, Func<string> readLabel, bool isHeading)
            {
                Rect = rect;
                Control = control;
                ReadLabel = readLabel;
                IsHeading = isHeading;
            }

            public RectTransform Rect { get; }
            public Selectable Control { get; }
            public Func<string> ReadLabel { get; }
            public bool IsHeading { get; }
            public float CentreY { get; private set; }
            public float Left { get; private set; }
            public float Height { get; private set; }

            /// <summary>Works out the entry's vertical centre, left edge and height from its world corners.</summary>
            public void Measure(Vector3[] corners)
            {
                Rect.GetWorldCorners(corners);
                CentreY = (corners[0].y + corners[1].y) * 0.5f;
                Left = corners[0].x;
                Height = Mathf.Abs(corners[1].y - corners[0].y);
            }
        }
    }
}
