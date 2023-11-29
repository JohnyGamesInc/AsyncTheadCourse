using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace JobsSystem.HW
{
    
    public class JobsHW : MonoBehaviour
    {

        private NativeArray<int> intsArray;

        
        private void Start()
        {
            intsArray = new NativeArray<int>(new[] {10, 15, 5, 7, 20, 3, 4, 11, 234, 45}, Allocator.TempJob);

            MoreTenJob moreTenJob = new MoreTenJob()
            {
                IntsArray = intsArray
            };

            JobHandle MoreTenJob = moreTenJob.Schedule();
            MoreTenJob.Complete();


            foreach (var el in intsArray)
            {
                Debug.Log(el);
            }
            
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


        private void OnDestroy()
        {
            intsArray.Dispose();
        }
        
        
    }
}