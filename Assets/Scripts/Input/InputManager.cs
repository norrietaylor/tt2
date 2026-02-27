using UnityEngine;
using UnityEngine.InputSystem;

namespace TaekwondoTech.Input
{
    /// <summary>
    /// InputManager — abstracts input across keyboard, touch, and gamepad using Unity's Input System.
    /// Provides action maps for Gameplay and UI with automatic device switching.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private PlayerInput playerInput;
        private InputActionMap gameplayActions;
        private InputActionMap uiActions;

        public Vector2 MoveInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool PunchPressed { get; private set; }
        public bool KickPressed { get; private set; }
        public bool SpecialPressed { get; private set; }
        public bool PowerUpPressed { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            playerInput = GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("InputManager requires a PlayerInput component.");
                return;
            }

            gameplayActions = playerInput.actions.FindActionMap("Gameplay");
            uiActions = playerInput.actions.FindActionMap("UI");

            SetupGameplayActions();
        }

        private void SetupGameplayActions()
        {
            if (gameplayActions == null) return;

            var moveAction = gameplayActions.FindAction("Move");
            if (moveAction != null)
            {
                moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
                moveAction.canceled += ctx => MoveInput = Vector2.zero;
            }

            var jumpAction = gameplayActions.FindAction("Jump");
            if (jumpAction != null)
            {
                jumpAction.performed += ctx => JumpPressed = true;
                jumpAction.canceled += ctx => JumpPressed = false;
            }

            var punchAction = gameplayActions.FindAction("Punch");
            if (punchAction != null)
            {
                punchAction.performed += ctx => PunchPressed = true;
                punchAction.canceled += ctx => PunchPressed = false;
            }

            var kickAction = gameplayActions.FindAction("Kick");
            if (kickAction != null)
            {
                kickAction.performed += ctx => KickPressed = true;
                kickAction.canceled += ctx => KickPressed = false;
            }

            var specialAction = gameplayActions.FindAction("Special");
            if (specialAction != null)
            {
                specialAction.performed += ctx => SpecialPressed = true;
                specialAction.canceled += ctx => SpecialPressed = false;
            }

            var powerUpAction = gameplayActions.FindAction("PowerUp");
            if (powerUpAction != null)
            {
                powerUpAction.performed += ctx => PowerUpPressed = true;
                powerUpAction.canceled += ctx => PowerUpPressed = false;
            }
        }

        public void EnableGameplayInput()
        {
            gameplayActions?.Enable();
            uiActions?.Disable();
        }

        public void EnableUIInput()
        {
            uiActions?.Enable();
            gameplayActions?.Disable();
        }

        private void OnDestroy()
        {
            if (gameplayActions != null)
            {
                gameplayActions.Disable();
            }
            if (uiActions != null)
            {
                uiActions.Disable();
            }
        }
    }
}
