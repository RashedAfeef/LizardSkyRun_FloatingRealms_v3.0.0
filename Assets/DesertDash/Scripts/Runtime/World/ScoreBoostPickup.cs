namespace DesertDash.World
{
    public sealed class ScoreBoostPickup : PooledPickup
    {
        public bool TryCollect()
        {
            return Consume();
        }
    }
}
