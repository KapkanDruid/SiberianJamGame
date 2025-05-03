using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public class TestCommand : Command
    {
        public override void Execute(Action onCompleted)
        {
            Debug.Log("Test Command executed");
        }
    }
}
