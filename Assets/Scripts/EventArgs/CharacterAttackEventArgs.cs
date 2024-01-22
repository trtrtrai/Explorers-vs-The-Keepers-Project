using Models;

namespace EventArgs
{
    public class CharacterAttackEventArgs : System.EventArgs
    {
        public readonly Character Target;
        public readonly int DamageDeal;

        public CharacterAttackEventArgs(Character target, int damageDeal)
        {
            Target = target;
            DamageDeal = damageDeal;
        }
    }
}