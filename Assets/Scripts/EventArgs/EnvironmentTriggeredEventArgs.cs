using Models;

namespace EventArgs
{
    public class EnvironmentTriggeredEventArgs : System.EventArgs
    {
        public readonly Character Target;
        public readonly int Amount;

        public EnvironmentTriggeredEventArgs(Character target, int amount)
        {
            Target = target;
            Amount = amount;
        }
    }
}