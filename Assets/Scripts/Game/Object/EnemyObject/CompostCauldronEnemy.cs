public class CompostCauldronEnemy : MothEnemy
{
    protected override void Shoot()
    {
        if (hp <= maxHp / 2 && shotCount % 2 != 0)
            FireFan(3, 40f, PlayerDirection());
        else
            FireRing(12);
    }
}
