using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Services;
using UnityEngine;

namespace Game.Runtime.Gameplay.Implants.Services
{
    public class ImplantsHolderService : IService, IInitializable
    {
        private readonly List<ImplantBehaviour> _implants = new();
        private ImplantsHolder _implantsHolder;
            
        public void Initialize()
        {
            _implantsHolder = ServiceLocator.Get<HUDService>().ImplantsHolder;
        }
        
        public async UniTask SetImplantsAsync(List<ImplantBehaviour> implants, bool force = false)
        {
            _implants.AddRange(implants);
            await _implantsHolder.SetImplants(implants, force);
        }
        
        public void ForceUpdateImplantPositions()
        {
            _implantsHolder.SetImplants(_implants, true).Forget();
        }


        public void TryRemoveItem(ImplantBehaviour implantBehaviour)
        {
            if (_implants.Contains(implantBehaviour))
                _implants.Remove(implantBehaviour);
        }
        
        public bool HasItem(ImplantBehaviour implantBehaviour)
        {
            return _implants.Contains(implantBehaviour);
        }

        public bool TryReturnToHolder(ImplantBehaviour implantBehaviour, Vector2 position)
        {
            if (_implants.Contains(implantBehaviour)) 
                return false;
            
            if (!_implantsHolder.IsInsideHolder(position))
                return false;

            _implants.Add(implantBehaviour);
            ForceUpdateImplantPositions();
            
            return true;
        }

        public async UniTask ReleaseImplants()
        {
            var releaseTasks = new List<UniTask>();
            foreach (var item in _implants)
                releaseTasks.Add(item.ReleaseImplant());
            
            await UniTask.WhenAll(releaseTasks);
            
            _implants.Clear();
        }
    }
}