public class MothQueenEnemy : MothEnemy
{
    protected override void Shoot()
    {
        if (shotCount % 2 == 0)
            FireRing(12, hp <= maxHp / 2 ? shotCount / 2 % 2 * 15f : 0f);
        else
            FireFan(7, 60f, PlayerDirection());
    }
}
