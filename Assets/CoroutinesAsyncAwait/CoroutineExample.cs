using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


namespace CoroutinesAsyncAwait
{
    
    public class CoroutineExample : MonoBehaviour
    {
        
        private Animator _animator;
        // private SpriteRenderer _renderer;

        private Material _material;

        private float _timer;

        private Coroutine coroutine;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _material = gameObject.GetComponent<MeshRenderer>().material;
            // _renderer = GetComponent<SpriteRenderer>();
            // Debug.Log("Before Start coroutine");
            // StartCoroutine(PrintOverTime());
            // Debug.Log("After Start coroutine");

            // StartCoroutine(MoveUp(2, Vector3.up));
            
            // StartCoroutine(MoveAround());
            // StartCoroutine(PrintAndDestroy());
            // StartCoroutine(PlayAnimation("Dance", 2));
            // StartCoroutine(PlayAnimation("Sleep", 1));
            // StartCoroutine(StopAnimation(0.5f));
            // coroutine = StartCoroutine(PrintMessage());
            StartCoroutine(GenericAnimation(new Vector3(5.0f, 5.0f, 5.0f), Color.blue, 5.0f));

        }

        private void Update()
        {
            // if (_timer >= 5.0) 
            //     StopCoroutine(coroutine);
            //
            // if (_timer >= 2.0) 
            //     StopAllCoroutines();
        }


        private IEnumerator PrintOverTime()
        {
            Debug.Log("Message before yield");
            yield return new WaitForSeconds(1.0f);
            Debug.Log("Message after 1 second");
        }


        private IEnumerator MoveUp(float time, Vector3 direction)
        {
            while (transform.position.y < 10)
            {
                yield return new WaitForSeconds(time);
                transform.position += direction;
            }
        }


        private IEnumerator MoveAround()
        {
            transform.position += Vector3.up;
            yield return new WaitForSeconds(1);
            
            transform.position += Vector3.left;
            yield return new WaitForSeconds(1);
            
            transform.position += Vector3.down;
            yield return new WaitForSeconds(1);
            
            transform.position += Vector3.right;
        }


        private IEnumerator PrintAndDestroy()
        {
            int i = 10;
            while (true)
            {
                Debug.Log($"{i} seconds left");
                i--;
                if (i == 1) this.enabled = false;
                if (i == 0) Destroy(this.gameObject);
                yield return new WaitForSeconds(1);
            }
        }


        private IEnumerator PlayAnimation(string animation, float delay)
        {
            yield return new WaitForSeconds(delay);
            _animator.Play(animation);
        }


        private IEnumerator StopAnimation(float delay)
        {
            yield return new WaitForSeconds(delay);
            _animator.StopPlayback();
        }


        private IEnumerator PrintMessage()
        {
            while (true)
            {
                _timer += Time.deltaTime;
                Debug.Log("Test Message");
                yield return null;
            }
        }


        private IEnumerator GenericAnimation(Vector3 targetPosition, Color targetColor, float duration)
        {
            Vector3 startPosition = transform.position;
            Color startColor = _material.color;
            float progress = 0;
            
            while (progress <= 1)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
                _material.color = Color.Lerp(startColor, targetColor, progress);
                progress += Time.deltaTime / duration;
                yield return null;
            }

            transform.position = targetPosition;
            _material.color = targetColor;
        }
        
        
    }
}