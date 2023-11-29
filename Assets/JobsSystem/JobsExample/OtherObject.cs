using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace JobsSystem.JobsExample
{
    
    public class OtherObject : MonoBehaviour
    {

        private NativeArray<Vector3> array;
        private JobHandle handle;

        
        private void Start()
        {
            this.array = new NativeArray<Vector3>(100, Allocator.TempJob);
            AdvancedJob job = new AdvancedJob();
            job.Array = array;

            handle = job.Schedule(100, 5);
            handle.Complete();

            StartCoroutine(JobCoroutine());
        }

        
        private IEnumerator JobCoroutine()
        {
            while (handle.IsCompleted == false)
            {
                yield return new WaitForEndOfFrame();
            }

            foreach (var vector in array)
            {
                Debug.Log(vector);
            }

            array.Dispose();
        }
        
        
        public struct AdvancedJob : IJobParallelFor
        {
            public NativeArray<Vector3> Array;

            public void Execute(int index)
            {
                Array[index] = Array[index].normalized;
            }
        }
        
        
    }
}