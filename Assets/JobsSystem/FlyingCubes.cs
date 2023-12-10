using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Burst;
using Unity.VisualScripting;
using Random = UnityEngine.Random;


namespace JobsSystem
{
    
    public class FlyingCubes : MonoBehaviour
    {

        [SerializeField] private GameObject prefabCube;
        [SerializeField] private Vector3 direction;
        [SerializeField] private int count;
        [SerializeField] private float spawnRadius;

        private NativeArray<Color> colorsA;
        private NativeArray<Color> output;
        private NativeArray<int> angle;
        private TransformAccessArray accessArray;
        private Material[] cubesu;

        private MaterialPropertyBlock materialBlock;


        private void Awake()
        {
            materialBlock = new MaterialPropertyBlock();
        }


        private void Start()
        {
            cubesu = new Material[count];
            colorsA = new NativeArray<Color>(count, Allocator.Persistent);
            output = new NativeArray<Color>(count, Allocator.Persistent);
            angle = new NativeArray<int>(count, Allocator.Persistent);
            accessArray = new TransformAccessArray(SpawnObj(prefabCube, count, spawnRadius));

            for (int i = 0; i < count; i++)
            {
                colorsA[i] = Random.ColorHSV();
                output[i] = Random.ColorHSV();
                angle[i] = Random.Range(0, 180);
            }
            
            StartCoroutine(ChangeDirection());
        }


        private void Update()
        {
            MyJobParTransform myJobParTransform = new MyJobParTransform()
            {
                direction = this.direction,
                deltaTime = Time.deltaTime,
                random = Random.Range(-1.0f, 1.0f),
                A = colorsA,
                Output = this.output,
                angles = this.angle
            };

            JobHandle jobHandle = myJobParTransform.Schedule(accessArray);
            jobHandle.Complete();

            for (int i = 0; i < count; i++)
            {
                cubesu[i].color = output[i];
            }
        }


        private Transform[] SpawnObj(GameObject prefab, int count, float spawnRadius)
        {
            Transform[] objects = new Transform[count];

            for (int i = 0; i < count; i++)
            {
                objects[i] = Instantiate(prefab).transform;
                cubesu[i] = objects[i].gameObject.GetComponent<MeshRenderer>().material;
                objects[i].gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(materialBlock);
                objects[i].position = Random.insideUnitSphere * spawnRadius;
            }

            return objects;
        }


        private IEnumerator ChangeDirection()
        {
            while (true)
            {
                ShuffleStruct job = new ShuffleStruct()
                {
                    Colors = colorsA,
                    seed = (uint) (UnityEngine.Random.value * 10000)
                };
                JobHandle jobHandle = job.Schedule();
                jobHandle.Complete();
                yield return new WaitForSeconds(1.0f);
            }
        }


        public NativeArray<Color> Shuffle(NativeArray<Color> colors)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                Color temp;
                int rnd = Random.Range(0, colors.Length);
                temp = colors[rnd];
                colors[rnd] = colors[i];
                colors[i] = temp;
            }

            return colors;
        }


        [BurstCompile]
        private struct MyJobParTransform : IJobParallelForTransform
        {
            public Vector3 direction;
            public float deltaTime;
            public float random;
            public NativeArray<Color> A;
            public NativeArray<Color> Output;
            public NativeArray<int> angles;

            public void Execute(int index, TransformAccess transform)
            {
                transform.position += direction * deltaTime;
                transform.localRotation = Quaternion.AngleAxis(angles[index], Vector3.up);
                angles[index] = angles[index] == 180 ? 0 : angles[index] + 1;
                Output[index] = Color.Lerp(Output[index], A[index], deltaTime);
            }
        }


        [BurstCompile]
        private struct ShuffleStruct : IJob
        {
            public NativeArray<Color> Colors;
            public uint seed;
            
            public void Execute()
            {
                Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);
                for (int i = 0; i < Colors.Length; i++)
                {
                    Color temp;
                    int rnd = random.NextInt(0, Colors.Length);
                    temp = Colors[rnd];
                    Colors[rnd] = Colors[i];
                    Colors[i] = temp;
                }
            }
        }


        private void OnDestroy()
        {
            if (accessArray.isCreated)
            {
                accessArray.Dispose();
                colorsA.Dispose();
                output.Dispose();
                angle.Dispose();
            }
        }
        
        
    }
}