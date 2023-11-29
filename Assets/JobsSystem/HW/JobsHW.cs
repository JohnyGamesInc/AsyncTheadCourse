using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace JobsSystem.HW
{
    
    public class JobsHW : MonoBehaviour
    {

        private NativeArray<int> intsArray;

        private JobHandle moreTenHandle;
        private JobHandle sumVectorsHandle;

        private NativeArray<Vector3> positions;
        private NativeArray<Vector3> velocities;
        private NativeArray<Vector3> finalPositions;



        private void Start()
        {
            intsArray = new NativeArray<int>(new[] {10, 15, 5, 7, 20, 3, 4, 11, 234, 45}, Allocator.TempJob);

            positions = new NativeArray<Vector3>(new[] {Vector3.up, Vector3.back, Vector3.down, Vector3.forward}, Allocator.TempJob);
            velocities = new NativeArray<Vector3>(new[] {Vector3.forward, Vector3.down, Vector3.back, Vector3.up}, Allocator.TempJob);
            finalPositions = new NativeArray<Vector3>(new Vector3[positions.Length], Allocator.TempJob);

            VectorsSumJob vectorsSumJob = new VectorsSumJob()
            {
                Positions = positions,
                Velocities = velocities,
                FinalPositions = finalPositions
            };

            MoreTenJob moreTenJob = new MoreTenJob()
            {
                IntsArray = intsArray
            };

            moreTenHandle = moreTenJob.Schedule();
            moreTenHandle.Complete();

            sumVectorsHandle = vectorsSumJob.Schedule(positions.Length, 0);
            sumVectorsHandle.Complete();
            
            StartCoroutine(JobCoroutine());
        }
        
        
        [BurstCompile]
        private struct MoreTenJob : IJob
        {
            public NativeArray<int> IntsArray;
            
            public void Execute()
            {
                for (int i = 0; i < IntsArray.Length; i++)
                {
                    if (IntsArray[i] > 10)
                        IntsArray[i] = 0;
                }
            }
        }
        
        
        [BurstCompile]
        private struct VectorsSumJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Vector3> Positions;
            [ReadOnly] public NativeArray<Vector3> Velocities;
            [WriteOnly] public NativeArray<Vector3> FinalPositions;
            
            public void Execute(int index)
            {
                FinalPositions[index] = Positions[index] + Velocities[index];
            }
        }
        
        
        private IEnumerator JobCoroutine()
        {
            while (sumVectorsHandle.IsCompleted == false)
            {
                yield return new WaitForEndOfFrame();
            }

            foreach (var el in finalPositions)
            {
                Debug.Log(el);
            }

            intsArray.Dispose();
            positions.Dispose();
            velocities.Dispose();
            finalPositions.Dispose();
        }


        private void OnDestroy()
        {
            // intsArray.Dispose();
        }
        
        
    }
}