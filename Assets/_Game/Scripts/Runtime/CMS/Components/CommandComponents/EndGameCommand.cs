using Game.Runtime.Gameplay;
using System;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Commands
{
    public class EndGameCommand : Command
    {
        public GameObject TitlesPrefab;
        public override void Execute(Action onCompleted)
        {
            GameObject.Instantiate(TitlesPrefab);

            var titles = TitlesPrefab.GetComponent<Titles>(); 
        }
    }
}
