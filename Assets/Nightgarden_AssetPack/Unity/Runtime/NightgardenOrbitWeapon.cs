using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class NightgardenOrbitWeapon : MonoBehaviour
{
    public NightgardenWeaponDefinition definition;
    public Transform owner;
    public int slot, slotCount=6;
    public float orbitRadius=.85f;
    float cooldown,attackAge=10;
    public int ShotsFired {get;private set;}
    public void Tick(float dt)
    {
        if(!definition||!owner)return;
        var renderer=GetComponent<SpriteRenderer>();renderer.sprite=definition.weapon;renderer.sortingOrder=10;
        float orbit=2*Mathf.PI*slot/Mathf.Max(1,slotCount);Vector3 home=owner.position+new Vector3(Mathf.Cos(orbit),Mathf.Sin(orbit),0)*orbitRadius;
        NightgardenTarget nearest=null;float best=definition.range*definition.range;
        foreach(var t in NightgardenTarget.Active){if(!t)continue;float d=(t.transform.position-home).sqrMagnitude;if(d<best){best=d;nearest=t;}}
        Vector3 direction=nearest?(nearest.transform.position-home).normalized:Vector3.right;
        float angle=Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg;
        cooldown-=dt;attackAge+=dt;
        if(nearest&&cooldown<=0)
        {
            cooldown=definition.cooldown;attackAge=0;ShotsFired++;
            Vector3 tip=home+direction*.45f;
            if(definition.melee){NightgardenShot.Effect(definition.impact,nearest.transform.position,angle);nearest.Hit(definition.damage);}
            else
            {
                NightgardenShot.Effect(definition.muzzle,tip,angle);
                if(definition.mode=="beam")
                {
                    var beam=new GameObject("Beam");var line=beam.AddComponent<LineRenderer>();line.material=new Material(Shader.Find("Sprites/Default"));line.startColor=new Color(1,.8f,.25f);line.endColor=line.startColor;line.startWidth=.06f;line.endWidth=.02f;line.positionCount=2;line.SetPosition(0,tip);line.SetPosition(1,nearest.transform.position);line.sortingOrder=20;Destroy(line.material,.12f);Destroy(beam,.12f);NightgardenShot.Effect(definition.impact,nearest.transform.position);nearest.Hit(definition.damage);
                }
                else {
                var go=new GameObject(definition.weaponId+" projectile");go.transform.position=tip;go.transform.rotation=Quaternion.Euler(0,0,angle);
                var sr=go.AddComponent<SpriteRenderer>();sr.sprite=definition.projectile;sr.sortingOrder=15;
                var shot=go.AddComponent<NightgardenShot>();shot.velocity=direction*definition.projectileSpeed;shot.damage=definition.damage;shot.impact=definition.impact;shot.lifetime=definition.range/definition.projectileSpeed+.25f;
                shot.mode=definition.mode;shot.owner=owner;if(definition.mode=="orbit"){shot.velocity=Vector3.zero;shot.lifetime=2;}
                }
            }
        }
        float motion=attackAge<.18f?Mathf.Sin(attackAge/.18f*Mathf.PI):0;
        transform.position=home-direction*(definition.melee?-.25f:.12f)*motion;
        transform.rotation=Quaternion.Euler(0,0,angle+(definition.melee?65*motion:0));renderer.flipY=direction.x<0;
    }
    void Update(){Tick(Time.deltaTime);}
}
