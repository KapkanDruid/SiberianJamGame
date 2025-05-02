namespace Game.Runtime.Gameplay
{
    public class WarriorTurnData
    {
        public float Heal;
        public float Damage;
        public float Armor;

        public WarriorTurnData(float heal, float damage, float armor)
        {
            Heal = heal;
            Damage = damage;
            Armor = armor;
        }
    }
}
