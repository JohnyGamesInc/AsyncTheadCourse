using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace CoroutinesAsyncAwait.HW
{
    
    public class AsyncAwaitHW : MonoBehaviour
    {

        private CancellationTokenSource ctsSource = new();


        private void Start()
        {
            var cts = ctsSource.Token;
            var t = new Task(() => Wait1AndPrint(cts));
            t.Start();
            Task.Run(() => Wait60Frames(cts));

            Task.Run(async () =>
            {
                await Task.Delay(5000);
                ctsSource.Cancel();
                Debug.Log("CANCELLATION REQUESTED");
            }, cts);

            Debug.Log("START ENDS");
        }


        private async Task Wait1AndPrint(CancellationToken cts)
        {
            Debug.Log("TASK 1 AND PRINT STARTED");
            
            if (cts.IsCancellationRequested)
            {
                Debug.Log("TOKEN CANCELLED");
                return;
            }
            
            await Task.Delay(1000);
            Debug.Log("WAITED 1 SEC AND ENDS");
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
            
            Debug.Log("WAITED 60 FRAMES AND ENDS");
        }

        
        private void OnDestroy()
        {
            ctsSource.Dispose();
        }
        
        
    }
}