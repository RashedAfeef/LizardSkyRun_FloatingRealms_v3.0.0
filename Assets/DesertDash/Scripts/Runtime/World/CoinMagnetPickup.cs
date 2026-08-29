namespace DesertDash.World
{
    public sealed class CoinMagnetPickup : PooledPickup
    {
        public bool TryCollect()
        {
            return Consume();
        }
    }
}
