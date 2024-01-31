namespace EventArgs
{
    public class GeneralRequireTriggerEventArgs : System.EventArgs
    {
        public readonly float OldProgress;
        public readonly float NewProgress;

        public GeneralRequireTriggerEventArgs(float oldProgress, float newProgress)
        {
            OldProgress = oldProgress;
            NewProgress = newProgress;
        }
    }
}