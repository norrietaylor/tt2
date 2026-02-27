using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TaekwondoTech.Input
{
    /// <summary>
    /// InputManager — abstracts input devices (keyboard, gamepad, touch) and exposes
    /// typed C# events for all gameplay actions. Implemented as a persistent singleton.
    ///
    /// Requires the Unity Input System package (com.unity.inputsystem) to be installed.
    /// Active Input Handling must be set to "Both" or "Input System Package (New)" in
    /// Project Settings > Player.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        // ── Singleton ─────────────────────────────────────────────────────────────
        public static InputManager Instance { get; private set; }

        // ── Action Maps ───────────────────────────────────────────────────────────
        private InputActionMap _gameplayMap;
        private InputActionMap _uiMap;

        // ── Gameplay Actions ──────────────────────────────────────────────────────
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _punchAction;
        private InputAction _kickAction;
        private InputAction _stompAction;

        // ── Public Events ─────────────────────────────────────────────────────────
        /// <summary>Fired each frame the move input changes. Value is a normalised direction vector.</summary>
        public event Action<Vector2> OnMove;

        /// <summary>Fired on the frame the jump button is first pressed.</summary>
        public event Action OnJumpStarted;

        /// <summary>Fired on the frame the jump button is released.</summary>
        public event Action OnJumpCanceled;

        /// <summary>Fired on the frame the punch button is pressed.</summary>
        public event Action OnPunch;

        /// <summary>Fired on the frame the kick button is pressed.</summary>
        public event Action OnKick;

        /// <summary>Fired on the frame the stomp button is pressed.</summary>
        public event Action OnStomp;

        // ── Public State ──────────────────────────────────────────────────────────
        /// <summary>The current move direction, updated every frame from the active device.</summary>
        public Vector2 MoveInput { get; private set; }

        // ── Unity Lifecycle ───────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            BuildActionMaps();
            BindCallbacks();
            EnableGameplay();
        }

        private void OnDestroy()
        {
            DisableAll();
            _gameplayMap?.Dispose();
            _uiMap?.Dispose();
        }

        // ── Action Map Construction ───────────────────────────────────────────────
        private void BuildActionMaps()
        {
            _gameplayMap = new InputActionMap("Gameplay");

            // Move — WASD composite, arrow-key composite, gamepad left stick
            _moveAction = _gameplayMap.AddAction("Move", InputActionType.Value);

            _moveAction.AddCompositeBinding("2DVector")
                .With("Up",    "<Keyboard>/w")
                .With("Down",  "<Keyboard>/s")
                .With("Left",  "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            _moveAction.AddCompositeBinding("2DVector")
                .With("Up",    "<Keyboard>/upArrow")
                .With("Down",  "<Keyboard>/downArrow")
                .With("Left",  "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");

            _moveAction.AddBinding("<Gamepad>/leftStick");

            // Jump — Space, gamepad South, primary touch tap
            _jumpAction = _gameplayMap.AddAction("Jump", InputActionType.Button);
            _jumpAction.AddBinding("<Keyboard>/space");
            _jumpAction.AddBinding("<Gamepad>/buttonSouth");
            _jumpAction.AddBinding("<Touchscreen>/primaryTouch/tap");

            // Punch — J, gamepad West
            _punchAction = _gameplayMap.AddAction("Punch", InputActionType.Button);
            _punchAction.AddBinding("<Keyboard>/j");
            _punchAction.AddBinding("<Gamepad>/buttonWest");

            // Kick — K, gamepad East
            _kickAction = _gameplayMap.AddAction("Kick", InputActionType.Button);
            _kickAction.AddBinding("<Keyboard>/k");
            _kickAction.AddBinding("<Gamepad>/buttonEast");

            // Stomp — L, gamepad North
            _stompAction = _gameplayMap.AddAction("Stomp", InputActionType.Button);
            _stompAction.AddBinding("<Keyboard>/l");
            _stompAction.AddBinding("<Gamepad>/buttonNorth");

            // UI map — navigation is handled by Unity's default EventSystem / UI actions
            _uiMap = new InputActionMap("UI");
        }

        private void BindCallbacks()
        {
            _moveAction.performed  += ctx =>
            {
                MoveInput = ctx.ReadValue<Vector2>();
                OnMove?.Invoke(MoveInput);
            };
            _moveAction.canceled   += _ =>
            {
                MoveInput = Vector2.zero;
                OnMove?.Invoke(MoveInput);
            };

            _jumpAction.started    += _ => OnJumpStarted?.Invoke();
            _jumpAction.canceled   += _ => OnJumpCanceled?.Invoke();

            _punchAction.started   += _ => OnPunch?.Invoke();
            _kickAction.started    += _ => OnKick?.Invoke();
            _stompAction.started   += _ => OnStomp?.Invoke();
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>
        /// Enables the Gameplay action map and disables the UI map.
        /// Call this when entering a gameplay scene.
        /// </summary>
        public void EnableGameplay()
        {
            _gameplayMap.Enable();
            _uiMap.Disable();
        }

        /// <summary>
        /// Enables the UI action map and disables Gameplay.
        /// Call this when opening a menu or pausing.
        /// </summary>
        public void EnableUI()
        {
            _uiMap.Enable();
            _gameplayMap.Disable();
        }

        /// <summary>
        /// Disables all action maps (e.g. during cutscenes or loading screens).
        /// </summary>
        public void DisableAll()
        {
            _gameplayMap?.Disable();
            _uiMap?.Disable();
        }
    }
}
