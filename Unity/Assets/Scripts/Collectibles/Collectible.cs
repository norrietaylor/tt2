using UnityEngine;
using UnityEngine.Events;

namespace TaekwondoTech.Collectibles
{
    /// <summary>
    /// Base class for all collectible items.
    /// Detects player collision and raises collection event.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public abstract class Collectible : MonoBehaviour
    {
        [Header("Collectible Settings")]
        [SerializeField] private float _destroyDelay = 0.1f;

        [Header("Effects")]
        [SerializeField] private ParticleSystem _collectEffect;
        [SerializeField] private AudioClip _collectSound;

        [Header("Events")]
        public UnityEvent OnCollected;

        private bool _isCollected;

        protected virtual void Awake()
        {
            Collider2D collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isCollected)
                return;

            if (other.CompareTag("Player"))
            {
                Collect();
            }
        }

        private void Collect()
        {
            _isCollected = true;

            OnCollected?.Invoke();
            OnCollectedEffect();
            OnCollectedLogic();

            Destroy(gameObject, _destroyDelay);
        }

        /// <summary>
        /// Override to add specific collection logic.
        /// </summary>
        protected abstract void OnCollectedLogic();

        /// <summary>
        /// Play visual and audio effects.
        /// </summary>
        private void OnCollectedEffect()
        {
            if (_collectEffect != null)
            {
                ParticleSystem effectInstance = Instantiate(_collectEffect, transform.position, Quaternion.identity);
                ParticleSystem.MainModule mainModule = effectInstance.main;
                float maxLifetime = mainModule.startLifetime.constantMax;
                float destroyDelay = mainModule.duration + maxLifetime;
                Destroy(effectInstance.gameObject, destroyDelay);
            }

            if (_collectSound != null && Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(_collectSound, Camera.main.transform.position);
            }
        }
    }
}
