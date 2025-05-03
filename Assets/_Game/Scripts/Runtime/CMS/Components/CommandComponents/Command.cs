using System;

namespace Game.Runtime.CMS.Components.Commands
{
    public abstract class Command : CMSComponent
    {
        public abstract void Execute(Action onCompleted);
    }
}
