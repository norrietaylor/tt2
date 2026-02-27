using UnityEngine;
using UnityEngine.Events;

namespace TaekwondoTech.Collectibles
{
    /// <summary>
    /// Collectible — base class for all collectible items (coins, robot parts, power-ups).
    /// Triggers collection on contact with player and invokes events for loose coupling.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Collectible : MonoBehaviour
    {
        [Header("Collection")]
        [SerializeField] protected int value = 1;
        [SerializeField] protected bool destroyOnCollect = true;

        [Header("Visual")]
        [SerializeField] protected float shimmerSpeed = 1f;
        [SerializeField] protected float shimmerAmount = 0.3f;

        [Header("Events")]
        public UnityEvent<int> OnCollected;

        protected SpriteRenderer spriteRenderer;
        protected bool isCollected;

        public int Value => value;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }
        }

        protected virtual void Update()
        {
            ApplyShimmer();
        }

        protected virtual void ApplyShimmer()
        {
            if (spriteRenderer != null)
            {
                float alpha = 1f + Mathf.Sin(Time.time * shimmerSpeed) * shimmerAmount;
                Color color = spriteRenderer.color;
                color.a = Mathf.Clamp01(alpha);
                spriteRenderer.color = color;
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (isCollected) return;

            if (collision.CompareTag("Player"))
            {
                Collect();
            }
        }

        protected virtual void Collect()
        {
            isCollected = true;
            OnCollected?.Invoke(value);

            if (destroyOnCollect)
            {
                Destroy(gameObject);
            }
        }
    }
}
