using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commands;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Services.Audio
{
    public class Invoker : IService
    {
        public void Play(CMSEntity blockEntity)
        {
            List<Command> commands = new List<Command>();

            var components = blockEntity.GetAll();

            foreach (var component in components)
            {
                if (component is Command command)
                    commands.Add(command);
            }

            if (commands.Count == 0)
            {
                Debug.LogError($"[Invoker] entity {blockEntity.EntityId} does not contain commands");
                return;
            }

            ExecuteCommands(commands).Forget();
        }

        private async UniTask ExecuteCommands(List<Command> commands)
        {
            foreach (var command in commands)
            {
                bool commandCompleted = false;

                command.Execute(() => commandCompleted = true);

                await UniTask.WaitUntil(() => commandCompleted == true);
            }
        }
    }
}
