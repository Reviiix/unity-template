using UnityEngine;

namespace PureFunctions.Movement
{
    public static class MoveInAndOutOfView
    {
        public static void MoveToCentre(Transform objectToMove, int speed)
        {
            MovementJobs.MoveObjects(new []{objectToMove}, new []{Vector3.zero}, speed);
        }
        
        public static void MoveToLeft(Transform objectToMove, int speed, int leftDistance)
        {
            MovementJobs.MoveObjects(new []{objectToMove}, new []{new Vector3(-leftDistance,0,0)}, speed);
        }
    }
}
