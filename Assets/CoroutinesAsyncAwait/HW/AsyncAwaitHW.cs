using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace CoroutinesAsyncAwait.HW
{
    public class AsyncAwaitHW : MonoBehaviour
    {
        private CancellationTokenSource ctsSource = new();


        private async void Start()
        {
            var cts = ctsSource.Token;
            // var t = new Task(() => Wait1AndPrint(cts));
            // t.Start();
            // Task.Run(() => Wait60Frames(cts));

            // Task.Run(async () =>
            // {
            //     await Task.Delay(5000);
            //     ctsSource.Cancel();
            //     Debug.Log("CANCELLATION REQUESTED FROM START");
            // }, cts);

            Debug.Log("START ENDS");

            var t1 = Wait1AndPrint(cts);
            var t2 = Wait60Frames(cts);
            
            var taskResult = await WhatTaskFasterAsync(cts, t1, t2);
            
            if (taskResult)
                Debug.Log($"FASTEST [Wait1Sec] {taskResult}");
            else
                Debug.Log($"FASTEST [Wait60Frames] {taskResult}");
        }


        private async Task Wait1AndPrint(CancellationToken cts)
        {
            Debug.Log("TASK 1 AND PRINT STARTED");

            if (cts.IsCancellationRequested)
            {
                Debug.Log("TOKEN CANCELLED");
                return;
            }

            await Task.Delay(2000);
            Debug.Log("WAIT 1 SEC ENDS");
        }


        private async Task Wait60Frames(CancellationToken cts)
        {
            Debug.Log("TASk 60 FRAMES WAIT AND PRINT STARTED");

            int frames = 0;
            while (frames <= 60)
            {
                frames++;
                if (cts.IsCancellationRequested)
                {
                    Debug.Log("TOKEN CANCELLED");
                    return;
                }

                await Task.Yield();
            }

            Debug.Log("WAITED 60 FRAMES ENDS");
        }


        private async Task<bool> WhatTaskFasterAsync(CancellationToken ct, Task task1, Task task2)
        {
            using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {
                Task resultTask = Task.WhenAny(task1, task2);
                linkedCts.Cancel();
                return resultTask == task1;
            }
        }


        private void OnDestroy()
        {
            Debug.Log("DISPOSE CANCELLATION TOKEN");
            ctsSource.Dispose();
        }
    }
}