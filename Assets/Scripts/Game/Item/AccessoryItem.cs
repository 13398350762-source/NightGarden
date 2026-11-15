using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class AccessoryItem : MonoBehaviour
{
    public Image imgAccessory;
    public void Init(AccessoryData data)
    {
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Accessory", (atlas) =>{
            imgAccessory.sprite=atlas.GetSprite(data.resIcon);
        });
    }
}
