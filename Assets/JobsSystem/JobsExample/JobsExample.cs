using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace JobsSystem.JobsExample
{
    
    public class JobsExample : MonoBehaviour
    {
        
        private void Start()
        {
            NativeArray<int> array = new NativeArray<int>(10, Allocator.Temp);
            array.Dispose();
        }



        private struct AdvancedJob : IJobParallelFor
        {
            public NativeArray<Vector3> Array;

            public void Execute(int index)
            {
                
            }
        }


        private struct SomeJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<Vector3> Positions;

            [WriteOnly]
            public NativeArray<Vector3> NewPositions;

            
            public void Execute(int index)
            {
                NewPositions[index] = Positions[0];
            }
        }
        
        
        private struct OtherJob : IJobParallelFor
        {
            [WriteOnly]
            public NativeArray<Vector3> Positions;

            [ReadOnly]
            public NativeArray<Vector3> NewPositions;

            
            public void Execute(int index)
            {
                Positions[index] = NewPositions[index];
            }
        }
        
        
    }
}