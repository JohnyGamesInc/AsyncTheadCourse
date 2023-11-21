using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace CoroutinesAsyncAwait
{
    
    public class AsyncExample : MonoBehaviour
    {
        public GameObject Cube;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private void Start()
        {
            CancellationToken cancelToken = _cancellationTokenSource.Token;
            
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                _cancellationTokenSource.Cancel();
            });
            
            // Task.Run(() => PrintForever(cancelToken, 10));

            // PrintAsync();
            // await Task.Run(() => SomeAction());
            // PrintAsync("TEST MESSAGE", 50);
            // UnitTasksAsync();
            // Task<int> task1 = WaitRandomTime(2000);
            // Task<int> task2 = WaitRandomTime(1000);
            // var taskResult = await Task.WhenAny(task1, task2);
            // Debug.Log($"FASTEST [{taskResult.Result}]");

            // Task task = new Task(() => FactorialAsync(cancelToken, 5));
            // task.Start();
            
            // _cancellationTokenSource.
            Task.Run(async () => await Rotate(cancelToken));
            // Rotate(cancelToken);
        }


        private async Task Rotate(CancellationToken ct)
        {
            while (true)
            {
                if (ct.IsCancellationRequested) return;
                
                Cube.transform.position += Vector3.one;
                await Task.Delay(500);
            }
        }
        


        private async Task<long> FactorialAsync(CancellationToken cancelToken, int x)
        {
            int result = 1;
            
            for (int i = 1; i < x; i++)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    Debug.Log("Factorial operation cancelled");
                    return result;
                }

                result *= i;
                await Task.Yield();
            }

            return result;
        }


        private async Task PrintForever(CancellationToken token, int foreverTime)
        {
            int i = 0;
            
            while (i <= foreverTime)
            {
                if (token.IsCancellationRequested)
                {
                    Debug.Log("OPERATION CANCELLED");
                    return;
                }
                
                i++;
                Debug.Log("Print Forever");
                await Task.Delay(1000);
            }
        }


        private async void PrintAsync()
        {
            Debug.Log("Message BEFORE AWAIT");
            await Task.Delay(1000);
            Debug.Log("Message AFTER AWAIT delay in 1 second");
        }
        
        
        private async void PrintAsync(string message, int times)
        {
            while (times > 0)
            {
                times--;
                Debug.Log(message);
                await Task.Yield();
            }
        }


        private async Task SomeAction()
        {
            gameObject.SetActive(false);
        }


        private async Task Unit1Async()
        {
            Debug.Log("Unit 1 starts chopping the wood");
            await Task.Delay(3000);
            Debug.Log("Unit 1 finishes chopping wood");
        }
        
        
        private async Task Unit2Async()
        {
            Debug.Log("Unit 2 starts patrolling");
            await Task.Delay(5000);
            Debug.Log("Unit 2 finishes patrolling");
        }


        private async void UnitTasksAsync()
        {
            Task task1 = Task.Run(() => Unit1Async());
            Task task2 = Task.Run(() => Unit2Async());

            await Task.WhenAll(task1, task2);
            Debug.Log("All units finished their tasks");
        }


        private async Task<int> WaitRandomTime(int time)
        {
            int rnd = time;
            await Task.Delay(rnd);
            return rnd;
        }


        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        
        
    }
}