using System.Collections;
using UnityEngine;


namespace CoroutinesAsyncAwait.HW
{
    
    public class Unit : MonoBehaviour
    {

        [SerializeField] private int health;

        private int healPoints = 5;
        private float healTimout = 0.5f;
        private float healTime = 3.0f;

        private bool isHealingInProcess;

        
        [ContextMenu(nameof(ReceiveHealing))]
        public void ReceiveHealing()
        {
            Debug.Log("Receive Healing Called");
            if (!isHealingInProcess) 
                StartCoroutine(ProcessHealing());
        }


        private IEnumerator ProcessHealing()
        {
            float timer = 0.0f;
            isHealingInProcess = true;

            while (timer <= healTime)
            {
                timer += healTimout;
                health += healPoints;
                if (health >= 100)
                {
                    health = 100;
                    break;
                }
                yield return new WaitForSeconds(healTimout);
            }
            isHealingInProcess = false;
            Debug.Log($"HEALTH [{health}]");
        }

        
    }
}