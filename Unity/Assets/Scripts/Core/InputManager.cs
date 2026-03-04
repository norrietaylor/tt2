using UnityEngine;
using UnityEngine.Events;

namespace TaekwondoTech.Core
{
    /// <summary>
    /// InputManager handles player input and raises events for game systems to consume.
    /// Provides a central place for input configuration and event dispatching.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [Header("Input Events")]
        public UnityEvent OnPunchInput;
        public UnityEvent OnKickInput;
        public UnityEvent OnJumpInput;

        [Header("Input Settings")]
        [SerializeField] private KeyCode _punchKey = KeyCode.Z;
        [SerializeField] private KeyCode _kickKey = KeyCode.X;
        [SerializeField] private KeyCode _jumpKey = KeyCode.Space;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (OnPunchInput == null)
            {
                OnPunchInput = new UnityEvent();
            }

            if (OnKickInput == null)
            {
                OnKickInput = new UnityEvent();
            }

            if (OnJumpInput == null)
            {
                OnJumpInput = new UnityEvent();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(_punchKey))
            {
                OnPunchInput?.Invoke();
            }

            if (Input.GetKeyDown(_kickKey))
            {
                OnKickInput?.Invoke();
            }

            if (Input.GetKeyDown(_jumpKey))
            {
                OnJumpInput?.Invoke();
            }
        }

        /// <summary>
        /// Get horizontal input axis (-1 to 1).
        /// </summary>
        public float GetHorizontalAxis()
        {
            return Input.GetAxis("Horizontal");
        }

        /// <summary>
        /// Get vertical input axis (-1 to 1).
        /// </summary>
        public float GetVerticalAxis()
        {
            return Input.GetAxis("Vertical");
        }
    }
}
