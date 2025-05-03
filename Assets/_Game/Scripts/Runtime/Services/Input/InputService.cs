using System;
using UnityEngine.InputSystem;

namespace Game.Runtime.Services.Input
{
    public class InputService : IService, IDisposable
    {
        private readonly InputMaps _inputMaps;

        public event Action OnRotateItem;
        public event Action OnDialogSkip;

        public InputService()
        {
            _inputMaps = new InputMaps();
            _inputMaps.Enable();
            
            Subscribe();
        }
        
        private void Subscribe()
        {
            _inputMaps.Player.Rotate.performed += HandleRotateItem;
            _inputMaps.Player.Skip.performed += HandleSkip;
        }
        
        private void Unsubscribe()
        {
            _inputMaps.Player.Rotate.performed -= HandleRotateItem;
            _inputMaps.Player.Skip.performed -= HandleSkip;
        }

        private void HandleRotateItem(InputAction.CallbackContext context)
        {
            OnRotateItem?.Invoke();
        }

        private void HandleSkip(InputAction.CallbackContext context)
        {
            OnDialogSkip?.Invoke();
        }

        public void Dispose()
        {
            Unsubscribe();
            _inputMaps?.Dispose();
        }
    }
}