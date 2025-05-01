using Game.Runtime.Gameplay.Grid;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Services;

namespace Game.Runtime.Gameplay.Unit
{
    public class BrainController
    {
        private GridData _brainGrid;
        
        public void SetBrainGrid(GridData gridData)
        {
            _brainGrid = gridData;
            SL.Get<HUDService>().SetBrainGrid(_brainGrid);
        }
    }
}