using Models;

namespace EventArgs
{
    public class EnvironmentTriggeredEventArgs : System.EventArgs
    {
        public Character Target;
        public int Amount;

        public EnvironmentTriggeredEventArgs(Character target, int amount)
        {
            Target = target;
            Amount = amount;
        }
    }
}