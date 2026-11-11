using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    public InputField inputUN;
    public InputField inputPW;
    public InputField inputSU;
    public Button btnRe;
    public Button btnBack;
    private bool isInit;
    protected override void Init()
    {
        if (isInit)
            return;
        btnBack.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<RegisterPanel>();
            UIManager.Instance.ShowPanel<LoginPanel>();
        });

        btnRe.onClick.AddListener(() =>{
            if (inputUN.text == string.Empty)
            {
                UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                    panel.ChangeInfo("用户名不能为空");
                });
                return;
            }
            if (inputPW.text == string.Empty)
            {
                UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                    panel.ChangeInfo("密码不能为空");
                });
                inputSU.text= string.Empty;
                return;
            }
            if (inputSU.text != inputPW.text)
            {
                UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                    panel.ChangeInfo("第二次输入密码与第一次不同");
                });
                inputSU.text = string.Empty;
                inputPW.text = string.Empty;
                return;
            }
            if (DataManager.Instance.CheckUser(inputUN.text))
            {
                UIManager.Instance.ShowPanel<TipPanel>((panel) => {
                    panel.ChangeInfo("用户名已存在");
                });
                inputSU.text = string.Empty;
                inputPW.text = string.Empty;
                inputUN.text = string.Empty;
                return;
            }
            //显示提示面板
            UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                panel.ChangeInfo("注册成功");
            });
            DataManager.Instance.AddUser(new UserInfo(inputUN.text,inputPW.text));
            UIManager.Instance.HidePanel<RegisterPanel>();
            UIManager.Instance.ShowPanel<LoginPanel>();
        });
        isInit = true;
    }

    public override void Hide(UnityAction action)
    {
        base.Hide(action);
        inputUN.text = string.Empty;
        inputPW.text = string.Empty;
        inputSU.text = string.Empty;
    }
}
