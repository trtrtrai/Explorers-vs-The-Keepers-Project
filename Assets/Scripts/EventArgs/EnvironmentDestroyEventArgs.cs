namespace EventArgs
{
    public class EnvironmentDestroyEventArgs : System.EventArgs
    {
        public readonly long EnvironmentChain;

        public EnvironmentDestroyEventArgs(long environmentChain)
        {
            EnvironmentChain = environmentChain;
        }
    }
}