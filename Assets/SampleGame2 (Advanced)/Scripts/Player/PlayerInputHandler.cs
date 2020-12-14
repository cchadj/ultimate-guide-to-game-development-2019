using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Zenject.SpaceFighter
{
    public class PlayerInputHandler : Controls.IPlayerActions
    {
        private readonly PlayerInputState _inputState;
        private readonly Controls _controls;

        public PlayerInputHandler(PlayerInputState inputState)
        {
            Debug.Log("Hello");
            _inputState = inputState;
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
            _controls.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Debug.Log(context.performed);
            var direction = context.ReadValue<Vector2>();
            _inputState.IsMovingRight = direction.x > 0;
            _inputState.IsMovingLeft = direction.x < 0;
            _inputState.IsMovingUp = direction.y > 0;
            _inputState.IsMovingDown = direction.y < 0;
        }

        public void DisableControls()
        {
            _controls.Disable();
        }

        public void EnableControls()
        {
            _controls.Enable();
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            Debug.Log("Hello");
            _inputState.IsFiring = true;
        }
        
    }
}
