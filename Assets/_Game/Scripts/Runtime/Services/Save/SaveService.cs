using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Runtime._Game.Scripts.Runtime.Utils.Extensions;
using UnityEngine;

namespace Runtime._Game.Scripts.Runtime.Services.Save
{
    public class SaveService : IService, IDisposable
    {
        private const string SaveId = "SaveId";
        private CancellationTokenSource _disposeTokenSource = new();

        public SaveData SaveData { get; private set; }
        
        public SaveService()
        {
            TryLoading();
            //StartAutoSave().Forget();
        }

        private void TryLoading()
        {
            SaveData = JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString(SaveId, "{}"));
        }

        public void Save()
        {
            var save = JsonConvert.SerializeObject(SaveData);
            PlayerPrefs.SetString(SaveId, save);
        }
        
        private async UniTaskVoid StartAutoSave()
        {
            while (_disposeTokenSource.IsValid())
            {
                await UniTask.WaitForSeconds(1, cancellationToken: _disposeTokenSource.Token)
                    .SuppressCancellationThrow();

                if (_disposeTokenSource.IsValid())
                {
                    var str = JsonConvert.SerializeObject(SaveData);
                    PlayerPrefs.SetString(SaveId, str);
                }
            }
        }

        public void Dispose()
        {
            _disposeTokenSource?.Dispose();
            _disposeTokenSource = null;
        }
    }
}