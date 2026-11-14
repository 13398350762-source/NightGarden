using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    public Button btnItem;
    public Image imgbk;
    public Image imgIcon;
    public Text txtLevel;
    public void Init(WeaponData data)
    {
        int level = data.level;
        string name= "weapon_frame_"+level.ToString();
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Components", (atlas) =>{
            imgbk.sprite = atlas.GetSprite(name);
        });
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Weapon", (atlas) => {
            imgIcon.sprite = atlas.GetSprite(data.resName);
        });
        txtLevel.text = (level == 4 ? "IV" : new string('I', level));
    }
}
