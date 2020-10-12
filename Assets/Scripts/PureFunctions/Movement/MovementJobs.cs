using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PureFunctions.Movement
{
    //The Job system requires a lot of boiler plate code but is the most optimised development strategy in Unity 2020.
    public static class MovementJobs
    {
        public static void MoveObjects(Transform[] objectsToMove, Vector3[] positions, float speed)
        {
            //Create primitive/native variables to pass to job.
            var amountOfObjects = objectsToMove.Length;
            var objectPositions = new NativeArray<float3>(amountOfObjects, Allocator.TempJob);
            var endPositions = new NativeArray<float3>(amountOfObjects, Allocator.TempJob);
            //Compose variables.
            for (var i = 0; i < amountOfObjects; i++)
            {
                objectPositions[i] = new float3(objectsToMove[i].localPosition);
                endPositions[i]= new float3(positions[i]);
            }
            
            //Create job handle and pass in values.
            var jobHandle = new MoveMultipleObjectsTowardsPosition
            {
                PositionsOfObjectsToMove = objectPositions,
                PositionsToMoveTo = endPositions,
                Speed = speed,
                DeltaTime = Time.deltaTime,
            };

            //Run the job with the handle.
            var handle = jobHandle.Schedule(amountOfObjects, 10);
            handle.Complete();
    
            //Reassign the position values (Having had the job applied).
            for (var i = 0; i < amountOfObjects; i++)
            {
                objectsToMove[i].transform.localPosition = objectPositions[i];
            }
    
            //Dispose of native arrays (Not handled by garbage collector).
            objectPositions.Dispose();
            endPositions.Dispose();
        }

        [BurstCompile]
        private struct MoveMultipleObjectsTowardsPosition : IJobParallelFor
        {
            public NativeArray<float3> PositionsOfObjectsToMove;
            public NativeArray<float3> PositionsToMoveTo;
            public float Speed;
            public float DeltaTime;

            public void Execute(int index)
            {
                var directionOfTravel = PositionsToMoveTo[index] - PositionsOfObjectsToMove[index];
                PositionsOfObjectsToMove[index] += new float3(directionOfTravel.x * Speed * DeltaTime, directionOfTravel.y * Speed * DeltaTime, directionOfTravel.z * Speed * DeltaTime);
            }
        }
  
    }
}
