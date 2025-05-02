using System;
using UnityEngine.InputSystem;

namespace Game.Runtime.Services.Input
{
    public class InputService : IService, IDisposable
    {
        private readonly InputMaps _inputMaps;

        public event Action OnRotateItem;

        public InputService()
        {
            _inputMaps = new InputMaps();
            _inputMaps.Enable();
            
            Subscribe();
        }
        
        private void Subscribe()
        {
            _inputMaps.Player.Rotate.performed += HandleRotateItem;
        }
        
        private void Unsubscribe()
        {
            _inputMaps.Player.Rotate.performed -= HandleRotateItem;
        }

        private void HandleRotateItem(InputAction.CallbackContext context)
        {
            OnRotateItem?.Invoke();
        }

        public void Dispose()
        {
            Unsubscribe();
            _inputMaps?.Dispose();
        }
    }
}