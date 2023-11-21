using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace CoroutinesAsyncAwait
{

    public delegate void CloseHandler(bool accepted);

    
    public class PopupExample : MonoBehaviour
    {

        [SerializeField] private Button buttonAccept;
        [SerializeField] private Button buttonCancel;

        public CloseHandler OnClose;


        private async Task<bool> PressButtonAsync(CancellationToken ct, Button button)
        {
            bool isPressed = false;

            button.onClick.AddListener(() => isPressed = true);

            while (isPressed == false)
            {
                if (ct.IsCancellationRequested) return false;

                await Task.Yield();
            }

            return true;
        }

        
        public async void ActivatePopup(CancellationToken ct)
        {
            using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {
                CancellationToken linkedCt = linkedCts.Token;
                Task<bool> task1 = PressButtonAsync(linkedCt, buttonAccept);
                Task<bool> task2 = PressButtonAsync(linkedCt, buttonCancel);
                Task<bool> finishedTask = await Task.WhenAny(task1, task2);
                bool result = (finishedTask == task1 && finishedTask.Result == true);
            
                linkedCts.Cancel();
                OnClose?.Invoke(result);
            }
        }
        

    }
}