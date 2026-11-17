public class SporeShamanEnemy : PuffballEnemy
{
    protected override void Shoot()
    {
        int count = shotCount % 2 == 0 ? 5 : 1;
        FireFan(count, spread, PlayerDirection());
    }
}
