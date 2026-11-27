using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityTemplate.Accessibility;
using UnityTemplate.Core;

namespace UnityTemplate.UserInterface.ConditionalMenus
{
    /// <summary>The daily bonus prize wheel: picks a weighted random segment and spins to it with DOTween.</summary>
    public class SpinningWheel : MonoBehaviour
    {
        [SerializeField] private WheelWeightings wheelSegments;
        [SerializeField] private TMP_Text[] segmentLabels;
        [Header("Controls")]
        [SerializeField] private int rotations;
        [SerializeField] [Range(0.1f, 15)] private float duration;
        [SerializeField] private Ease wheelEasing;
        [SerializeField] private float easingOvershoot;
        private Transform wheel;
        private int currentSegment;

        public event Action<int> SpinEnded;

        private int AmountOfSegments => wheelSegments.AmountOfSegments;

        /// <summary>Caches the wheel, resets it to segment 0 and writes each segment's credit value on its label.</summary>
        private void Awake()
        {
            wheel = transform;
            currentSegment = 0;
            wheel.eulerAngles = Vector3.zero;
            for (var i = 0; i < segmentLabels.Length && i < AmountOfSegments; i++)
            {
                segmentLabels[i].SetText("{0:0}", wheelSegments.GetSegmentValue(i));
            }
        }

        /// <summary>Picks a segment using the designer-set chance ranges.</summary>
        public int PickRandomSegment()
        {
            return wheelSegments.PickRandomSegment();
        }

        /// <summary>Returns the credits a segment is worth.</summary>
        public int GetSegmentValue(int segment)
        {
            return wheelSegments.GetSegmentValue(segment);
        }

        /// <summary>Spins to the given segment (or turns straight to it with reduced motion) and raises SpinEnded with its value when the wheel stops.</summary>
        public void SpinTo(int targetSegment)
        {
            var segmentsToTurn = (AmountOfSegments + (targetSegment - currentSegment)) % AmountOfSegments;
            var rotation = new Vector3(0, 0, GetRotationAngle(segmentsToTurn));
            currentSegment = targetSegment;

            var seconds = ServiceLocator.TryGet(out AccessibilityService accessibility) ? accessibility.MotionSeconds(duration) : duration;
            wheel.DOLocalRotate(rotation, seconds).SetEase(wheelEasing, easingOvershoot).SetLink(gameObject).OnComplete(() =>
            {
                SpinEnded?.Invoke(wheelSegments.GetSegmentValue(targetSegment));
            });
        }

        /// <summary>Returns the final angle for turning a number of segments plus the configured full rotations.</summary>
        private float GetRotationAngle(int segment)
        {
            return (-(float)(AmountOfSegments - segment % AmountOfSegments) / AmountOfSegments - rotations) * 360;
        }

        /// <summary>The wheel's segments and the chance range of each.</summary>
        [Serializable]
        private class WheelWeightings
        {
            [SerializeField] private WheelSegmentChance[] wheelSegments;

            public int AmountOfSegments => wheelSegments.Length;

            /// <summary>Returns the credits of a segment.</summary>
            public int GetSegmentValue(int index)
            {
                return wheelSegments[index].value;
            }

            /// <summary>Returns a random segment whose chance range contains the roll.</summary>
            public int PickRandomSegment()
            {
                var chance = UnityEngine.Random.Range(0, wheelSegments[^1].maximumChance);
                for (var i = 0; i < wheelSegments.Length; i++)
                {
                    if (chance >= wheelSegments[i].minimumChance && chance <= wheelSegments[i].maximumChance) return i;
                }
                Debug.LogError("The wheel's chance ranges have a gap; segment 0 was chosen.");
                return 0;
            }
        }

        /// <summary>One segment: its chance range and credit value.</summary>
        [Serializable]
        private struct WheelSegmentChance
        {
            public int minimumChance;
            public int maximumChance;
            public int value;
        }
    }
}
