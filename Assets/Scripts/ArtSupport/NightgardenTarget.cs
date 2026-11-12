using System.Collections.Generic;
using UnityEngine;
public class NightgardenTarget : MonoBehaviour
{
    public static readonly List<NightgardenTarget> Active=new List<NightgardenTarget>();
    public float health=20, radius=.3f;
    public bool invulnerable;
    void OnEnable(){Active.Add(this);}
    void OnDisable(){Active.Remove(this);}
    public void Hit(float damage){if(invulnerable)return;health-=damage;if(health<=0)gameObject.SetActive(false);}
}
