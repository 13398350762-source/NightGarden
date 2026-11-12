using UnityEngine;
public class NightgardenShot : MonoBehaviour
{
    public Vector3 velocity;
    public float damage, lifetime=2;
    public Sprite impact;
    public bool visualOnly;
    public string mode;
    public Transform owner;
    Vector3 initialVelocity;
    void Start(){initialVelocity=velocity;}
    float age;
    public static void Effect(Sprite sprite,Vector3 position,float angle=0)
    {
        if(!sprite)return;var go=new GameObject("Weapon Effect");go.transform.position=position;go.transform.rotation=Quaternion.Euler(0,0,angle);
        var sr=go.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sortingOrder=30;
        var fx=go.AddComponent<NightgardenShot>();fx.visualOnly=true;fx.lifetime=.18f;
    }
    void Update()
    {
        age+=Time.deltaTime;if(age>=lifetime){Destroy(gameObject);return;}
        if(visualOnly){transform.localScale=Vector3.one*(.6f+age/lifetime*.6f);GetComponent<SpriteRenderer>().color=new Color(1,1,1,1-age/lifetime);return;}
        if(mode=="homing" && NightgardenTarget.Active.Count>0){var t=NightgardenTarget.Active[0];if(t)velocity=Vector3.Lerp(velocity,(t.transform.position-transform.position).normalized*initialVelocity.magnitude,Time.deltaTime*6);}
        if(mode=="boomerang"&&age>lifetime*.45f&&owner)velocity=(owner.position-transform.position).normalized*initialVelocity.magnitude;
        if(mode=="orbit"&&owner){float a=age*7;transform.position=owner.position+new Vector3(Mathf.Cos(a),Mathf.Sin(a),0)*1.5f;}
        Vector3 from=transform.position,to=from+velocity*Time.deltaTime,segment=to-from;
        foreach(var target in NightgardenTarget.Active.ToArray())
        {
            if(!target)continue;
            float t=segment.sqrMagnitude>0?Mathf.Clamp01(Vector3.Dot(target.transform.position-from,segment)/segment.sqrMagnitude):0;
            if(Vector3.Distance(from+segment*t,target.transform.position)>target.radius)continue;
            Effect(impact,target.transform.position);target.Hit(damage);
            if(mode=="explosive") foreach(var other in NightgardenTarget.Active.ToArray()) if(other&&other!=target&&Vector3.Distance(other.transform.position,to)<1.2f)other.Hit(damage);
            Destroy(gameObject);return;
        }
        transform.position=to;
    }
}
