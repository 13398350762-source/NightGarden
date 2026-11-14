using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ResManager.Instance.LoadRes<SpriteAtlas>("atlas", "Componets");
        UIManager.Instance.ShowPanel<RoleSelectPanel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
