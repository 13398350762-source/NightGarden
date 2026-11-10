using UnityEngine;
[CreateAssetMenu(menuName="Nightgarden/Weapon")]
public class NightgardenWeaponDefinition : ScriptableObject
{
    public string weaponId;
    public Sprite weapon, projectile, muzzle, impact;
    public float cooldown=0.8f, range=6f, projectileSpeed=9f, damage=1f;
    public bool melee;
    public string mode="straight";
}
