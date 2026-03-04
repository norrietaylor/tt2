using UnityEngine;

namespace TaekwondoTech.Levels
{
    /// <summary>
    /// CameraFollower — makes the camera follow a target (typically the player)
    /// with smooth movement and optional bounds constraints.
    /// </summary>
    public class CameraFollower : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _target;

        [Header("Follow Settings")]
        [SerializeField] private float _smoothSpeed = 0.125f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, -10f);

        [Header("Bounds")]
        [SerializeField] private bool _useBounds = true;
        [SerializeField] private Vector2 _minBounds = new Vector2(-10f, 0f);
        [SerializeField] private Vector2 _maxBounds = new Vector2(10f, 10f);

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            Vector3 desiredPosition = _target.position + _offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);

            if (_useBounds)
            {
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, _minBounds.x, _maxBounds.x);
                smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, _minBounds.y, _maxBounds.y);
            }

            transform.position = smoothedPosition;
        }

        /// <summary>
        /// Sets the target for the camera to follow.
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        /// <summary>
        /// Sets the camera bounds.
        /// </summary>
        public void SetBounds(Vector2 min, Vector2 max)
        {
            _minBounds = min;
            _maxBounds = max;
            _useBounds = true;
        }

        /// <summary>
        /// Disables camera bounds.
        /// </summary>
        public void DisableBounds()
        {
            _useBounds = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_useBounds)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Vector3 bottomLeft = new Vector3(_minBounds.x, _minBounds.y, 0f);
            Vector3 topLeft = new Vector3(_minBounds.x, _maxBounds.y, 0f);
            Vector3 topRight = new Vector3(_maxBounds.x, _maxBounds.y, 0f);
            Vector3 bottomRight = new Vector3(_maxBounds.x, _minBounds.y, 0f);

            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
        }
#endif
    }
}
