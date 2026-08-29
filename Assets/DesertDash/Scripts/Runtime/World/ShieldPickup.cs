namespace DesertDash.World
{
    public sealed class ShieldPickup : PooledPickup
    {
        public bool TryCollect()
        {
            return Consume();
        }
    }
}
