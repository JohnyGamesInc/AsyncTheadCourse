using System;
using System.Threading;
using UnityEngine;


namespace CoroutinesAsyncAwait
{
    
    public class PurchaseExample : MonoBehaviour
    {

        [SerializeField] private GameObject popupPrefab;

        
        private void Start()
        {
            TryBuyItem();
        }


        private void TryBuyItem()
        {
            GameObject newPopup = Instantiate(popupPrefab, transform);
            PopupExample popup = newPopup.GetComponent<PopupExample>();

            popup.OnClose += CompletePurchase;

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            popup.ActivatePopup(ct);
        }


        private void CompletePurchase(bool completed)
        {
            if (completed) Debug.Log("Purchase Completed");
            else Debug.Log("Purchase Cancelled");
        }
        

    }
}