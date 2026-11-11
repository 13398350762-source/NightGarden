using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public InputField inputUN;
    public InputField inputPW;
    public Button btnLogin;
    public Button btnRe;
    private bool isInit=false;
    protected override void Init()
    {
        if (isInit)
            return;
        btnLogin.onClick.AddListener(() =>{
            if (inputUN.text == string.Empty)
            {
                UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                    panel.ChangeInfo("用户名不能为空");
                });
            }
            if (inputPW.text == string.Empty)
            {
                UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                    panel.ChangeInfo("密码不能为空");
                });
            }
            if (DataManager.Instance.CheckLogin(inputUN.text, inputPW.text))
            {
                ScenesManager.Instance.LoadSceneAsync("BeginScene", () => {
                    UIManager.Instance.HidePanel<LoginPanel>();
                    UIManager.Instance.ShowPanel<MainPanel>();
                });
            }
            else
            {
                UIManager.Instance.ShowPanel<TipPanel>((panel) => {
                    panel.ChangeInfo("密码不正确");
                });
            }
        });

        btnRe.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<LoginPanel>();
            UIManager.Instance.ShowPanel<RegisterPanel>();
        });
        isInit = true;
    }

    public override void Hide(UnityAction action)
    {
        base.Hide(action);
        inputPW.text = string.Empty;
        inputUN.text = string.Empty;
    }
}
