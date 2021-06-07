using System;
using System.Collections;
using UnityEngine;

namespace MostlyPureFunctions.Movement
{
    public static class MoveInAndOutOfView
    {
        public static IEnumerator MoveTransformToCentre(Transform transformToMove, int speed, int distanceTolerance = 1, Action callBack = null)
        {
            while (Vector2.Distance(transformToMove.localPosition, Vector2.zero) >= distanceTolerance)
            {
                MoveToCentre(transformToMove, speed);
                yield return null;
            }
            callBack?.Invoke();
        }
        
        public static IEnumerator MoveTransformToLeft(Transform transformToMove, int speed, Vector3 leftPosition, int distanceTolerance = 1, Action callBack = null)
        {
            while (Vector2.Distance(transformToMove.localPosition, -leftPosition) >= distanceTolerance)
            {
                MoveToLeft(transformToMove, speed, (int)leftPosition.x);
                yield return null;
            }
            callBack?.Invoke();
        }
        
        private static void MoveToCentre(Transform objectToMove, int speed)
        {
            MovementJobs.MoveObjects(new []{objectToMove}, new []{Vector3.zero}, speed);
        }
        
        private static void MoveToLeft(Transform objectToMove, int speed, int leftDistance)
        {
            MovementJobs.MoveObjects(new []{objectToMove}, new []{new Vector3(-leftDistance,0,0)}, speed);
        }
    }
}
