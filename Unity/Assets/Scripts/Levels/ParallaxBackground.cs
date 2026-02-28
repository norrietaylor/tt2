using UnityEngine;

namespace TaekwondoTech.Levels
{
    /// <summary>
    /// ParallaxBackground — creates parallax scrolling effect for background layers.
    /// Each layer moves at a different speed relative to the camera movement.
    /// </summary>
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Parallax Settings")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _parallaxEffectMultiplier = 0.5f;

        [Header("Layer Settings")]
        [Tooltip("Enable to repeat the background infinitely")]
        [SerializeField] private bool _infiniteRepeat = false;
        [SerializeField] private float _textureUnitSizeX = 20f;

        private Vector3 _lastCameraPosition;
        private float _startPositionX;

        private void Start()
        {
            if (_cameraTransform == null)
            {
                _cameraTransform = Camera.main.transform;
            }

            _lastCameraPosition = _cameraTransform.position;
            _startPositionX = transform.position.x;
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null)
            {
                return;
            }

            Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
            transform.position += new Vector3(deltaMovement.x, deltaMovement.y, 0f) * _parallaxEffectMultiplier;

            _lastCameraPosition = _cameraTransform.position;

            if (_infiniteRepeat)
            {
                float distanceFromStart = _cameraTransform.position.x - _startPositionX;
                float tempX = distanceFromStart * (1f - _parallaxEffectMultiplier);

                if (tempX > _startPositionX + _textureUnitSizeX)
                {
                    transform.position = new Vector3(_startPositionX + _textureUnitSizeX,
                                                     transform.position.y,
                                                     transform.position.z);
                }
                else if (tempX < _startPositionX - _textureUnitSizeX)
                {
                    transform.position = new Vector3(_startPositionX - _textureUnitSizeX,
                                                     transform.position.y,
                                                     transform.position.z);
                }
            }
        }

        /// <summary>
        /// Sets the camera to follow for parallax effect.
        /// </summary>
        public void SetCamera(Transform camera)
        {
            _cameraTransform = camera;
            _lastCameraPosition = camera.position;
        }

        /// <summary>
        /// Sets the parallax multiplier. Use lower values (0.1-0.3) for far backgrounds,
        /// higher values (0.5-0.8) for closer layers.
        /// </summary>
        public void SetParallaxMultiplier(float multiplier)
        {
            _parallaxEffectMultiplier = multiplier;
        }
    }
}
