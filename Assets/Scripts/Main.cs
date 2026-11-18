using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    private BaseEnemyObject enemy;
    private PlayerController player;
    void Start()
    {
        EventCallBack.Instance.Init();
        AudioManager.Instance.PlaySceneMusic(SceneManager.GetActiveScene().name);
        ResManager.Instance.LoadRes<SpriteAtlas>("atlas", "Components");
        UIManager.Instance.ShowPanel<LoginPanel>();
        Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    void Update()
    {
        
    }
}
