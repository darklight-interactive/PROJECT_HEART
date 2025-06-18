using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace RemixSurvivors
{
    public class MedWaveAttackController : MonoBehaviour
    {
        [SerializeField] private float _waveDuration = 1f;
        [SerializeField] private float _maxSize = 10f;    

        private Coroutine _waveCoroutine;


        [Button]
        public void StartWave()
        {
            if (_waveCoroutine != null)
                StopCoroutine(_waveCoroutine);
            _waveCoroutine = StartCoroutine(WaveRoutine());
        }

        IEnumerator WaveRoutine()
        {
            float timer = 0f;
            this.transform.localScale = Vector3.zero;

            while (timer < _waveDuration)
            {
                this.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * _maxSize, timer / _waveDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            this.transform.localScale = Vector3.zero;
        }
    
        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent<Entity>(out var entity))
            {
                entity.HealthController.TakeDamage(10);
            }
        }
    }
}