using System;
using _Game.Scripts.Runtime.Services.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime._Game.Scripts.Runtime.Services.Input
{
    public class InputService : IService, IDisposable
    {
        private readonly InputMaps _inputMaps;

        public event Action<Vector2> OnLook;
        public event Action<Vector2> OnMove;

        public InputService()
        {
            _inputMaps = new InputMaps();
            _inputMaps.Enable();
            
            Subscribe();
        }
        
        private void Subscribe()
        {
            _inputMaps.Player.Move.performed += HandleMove;
            _inputMaps.Player.Move.canceled += HandleMove;
            
            _inputMaps.Player.Look.performed += HandleLook;
            _inputMaps.Player.Look.canceled += HandleLook;
        }
        
        private void Unsubscribe()
        {
            _inputMaps.Player.Look.performed -= HandleLook;
            _inputMaps.Player.Look.canceled -= HandleLook;
        }

        private void HandleMove(InputAction.CallbackContext context)
        {
            OnMove?.Invoke(context.ReadValue<Vector2>());
        }
        
        private void HandleLook(InputAction.CallbackContext context)
        {
            OnLook?.Invoke(context.ReadValue<Vector2>());
        }

        public void Dispose()
        {
            Unsubscribe();
            _inputMaps?.Dispose();
        }
    }
}