using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class SpinningWheel : MonoBehaviour
{
    public Action<int> OnWheelSpinEnded;
    private Transform _wheel;
    private int _currentSegment;
    [SerializeField] private WheelWeightings wheelSegments;
    [Header("Controls")]
    [SerializeField] private int rotations;
    [SerializeField] [Range(0.1f, 15)] private float duration;
    [SerializeField] private Ease wheelEasing;
    [SerializeField] private float easingOvershoot;
    private int AmountOfSegments => wheelSegments.ReturnAmountOfSegments;
    private float ReturnRotationAngle(int segment) => (-(float)(AmountOfSegments - segment%AmountOfSegments) / AmountOfSegments - rotations) * 360;

    private void Awake()
    {
        _wheel = GetComponent<Transform>();
        ResetWheel();
    }

    [ContextMenu(nameof(StartSpin))]
    public void StartSpin()
    {
        Spin(wheelSegments.ReturnFinalSegmentIndex());
    }

    private void Spin(int targetSegment)
    {
        var rotationAngle = new Vector3(0, 0, ReturnRotationAngle((AmountOfSegments + (targetSegment - _currentSegment)) % AmountOfSegments));
        _currentSegment = targetSegment;

        _wheel.DOLocalRotate(rotationAngle, duration).SetEase(wheelEasing, easingOvershoot).OnComplete(() =>
        {
            OnWheelSpinEnded?.Invoke(wheelSegments.ReturnSegmentValue(targetSegment));
        });
    }

    private void ResetWheel()
    {
        _currentSegment = 0;
        _wheel.eulerAngles = Vector3.zero;
    }
    
    [Serializable]
    private class WheelWeightings
    {
        [SerializeField] private WheelSegmentChance[] wheelSegments;
        
        public int ReturnAmountOfSegments => wheelSegments.Length;
        
        public int ReturnSegmentValue(int index) => wheelSegments[index].value;

        public int ReturnFinalSegmentIndex()
        {
            var chance = UnityEngine.Random.Range(0, wheelSegments.Last().maximumChance);
            var amountOfSegments = wheelSegments.Length;
            for (var i = 0; i < amountOfSegments; i++)
            {
                var segment = wheelSegments[i];
                if (chance >= segment.minimumChance && chance <= segment.maximumChance) return i;
            }
            Debug.LogError("Not a valid segment value, index returned as 0.");
            return 0;
        }
    }
    
    [Serializable]
    private struct WheelSegmentChance
    {
        public int minimumChance;
        public int maximumChance;
        public int value;
    }
}
