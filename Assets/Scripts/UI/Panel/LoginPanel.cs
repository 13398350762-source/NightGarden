using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public InputField inputUN;
    public InputField inputPW;
    public Button btnLogin;
    public Button btnRe;
    protected override void Init()
    {
        btnLogin.onClick.AddListener(() =>{

        });

        btnRe.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<LoginPanel>(() => {
                inputPW.text = string.Empty;
                inputUN.text = string.Empty;
                });
            UIManager.Instance.ShowPanel<RegisterPanel>();
        });
    }
}
