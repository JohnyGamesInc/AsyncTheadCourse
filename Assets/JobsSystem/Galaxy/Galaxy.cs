using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;


namespace JobsSystem.Galaxy
{
    
    public class Galaxy : MonoBehaviour
    {
        
        [SerializeField] private GameObject celestialBodyPrefab;

        [SerializeField] private int numberOfEntities;
        [SerializeField] private float startDistance;
        [SerializeField] private float startVelocity;
        [SerializeField] private float startMass;
        [SerializeField] private float gravitationModifier;

        private NativeArray<Vector3> positions;
        private NativeArray<Vector3> velocities;
        private NativeArray<Vector3> accelerations;
        private NativeArray<float> masses;
        private NativeArray<int> angles;

        private TransformAccessArray transformAccessArray;


        private void Start()
        {
            positions = new NativeArray<Vector3>(numberOfEntities, Allocator.Persistent);
            velocities = new NativeArray<Vector3>(numberOfEntities, Allocator.Persistent);
            accelerations = new NativeArray<Vector3>(numberOfEntities, Allocator.Persistent);
            masses = new NativeArray<float>(numberOfEntities, Allocator.Persistent);
            angles = new NativeArray<int>(numberOfEntities, Allocator.Persistent);
            
            Transform[] transforms = new Transform[numberOfEntities];
            
            for (int i = 0; i < numberOfEntities; i++)
            {
                positions[i] = Random.insideUnitSphere * Random.Range(0, startDistance);
                velocities[i] = Random.insideUnitSphere * Random.Range(0, startVelocity);
                accelerations[i] = Vector3.zero;
                masses[i] = Random.Range(1, startMass);
                
                transforms[i] = Instantiate(celestialBodyPrefab, positions[i], Quaternion.identity).transform;
            }

            transformAccessArray = new TransformAccessArray(transforms);
        }


        private void Update()
        {
            GravitationJob gravitationJob = new GravitationJob()
            {
                Positions = positions,
                Velocities = velocities,
                Accelerations = accelerations,
                Masses = masses,
                GravitationModifier = this.gravitationModifier,
                DeltaTime = Time.deltaTime
            };
            
            JobHandle gravitationJobHandle = gravitationJob.Schedule(numberOfEntities, 0);

            
            MoveJob moveJob = new MoveJob()
            {
                Positions = positions,
                Velocities = velocities,
                Accelerations = accelerations,
                DeltaTime = Time.deltaTime
            };

            JobHandle moveJobHandle = moveJob.Schedule(transformAccessArray, gravitationJobHandle);
            moveJobHandle.Complete();

            
            RotateJob rotateJob = new RotateJob()
            {
                Angles = angles
            };

            JobHandle rotateHandle = rotateJob.Schedule(transformAccessArray);
            rotateHandle.Complete();
        }


        [BurstCompile]
        private struct GravitationJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Vector3> Positions;
            [ReadOnly] public NativeArray<Vector3> Velocities;
            
            public NativeArray<Vector3> Accelerations;
            [ReadOnly] public NativeArray<float> Masses;
            [ReadOnly] public float GravitationModifier;
            [ReadOnly] public float DeltaTime;


            public void Execute(int index)
            {
                for (int i = 0; i < Positions.Length; i++)
                {
                    if (i == index) continue;

                    float distance = Vector3.Distance(Positions[i], Positions[index]);
                    Vector3 direction = (Positions[i] - Positions[index]).normalized;

                    // F = (G * m1 * m2) / (R * R)
                    // Vector3 gravitation = (direction * Masses[i] * GravitationModifier) / (Masses[index] * distance * distance);
                    
                    Vector3 gravitation = direction * (Masses[i] * Masses[index] * GravitationModifier) / (distance * distance);
                    
                    Accelerations[index] += gravitation * DeltaTime;
                }
            }
        }
        
        
        [BurstCompile]
        private struct MoveJob : IJobParallelForTransform
        {
            public NativeArray<Vector3> Positions;
            public NativeArray<Vector3> Velocities;
            public NativeArray<Vector3> Accelerations;
            
            [ReadOnly] public float DeltaTime;


            public void Execute(int index, TransformAccess transform)
            {
                Vector3 velocity = Velocities[index] + Accelerations[index];
                transform.position += velocity * DeltaTime;

                Positions[index] = transform.position;
                Velocities[index] = velocity;
                Accelerations[index] = Vector3.zero;
            }
        }


        [BurstCompile]
        private struct RotateJob : IJobParallelForTransform
        {
            public NativeArray<int> Angles;

            public void Execute(int index, TransformAccess transform)
            {
                transform.rotation = Quaternion.AngleAxis(Angles[index], Vector3.up);
                Angles[index] = Angles[index] == 180 ? 0 : Angles[index] + 1;
            }
        }


        private void OnDestroy()
        {
            positions.Dispose();
            velocities.Dispose();
            accelerations.Dispose();
            masses.Dispose();
            transformAccessArray.Dispose();
        }
        
        
    }
}