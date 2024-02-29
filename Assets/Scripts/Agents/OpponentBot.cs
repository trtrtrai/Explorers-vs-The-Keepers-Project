using EventArgs;
using Unity.MLAgents;

namespace Agents
{
    public abstract class OpponentBot : Agent
    {
        public abstract void OnHeadquarterDestroy(object sender, System.EventArgs args);
        public abstract void OnCharacterSpawn(object sender, CharacterSpawnEventArgs args);
    }
}