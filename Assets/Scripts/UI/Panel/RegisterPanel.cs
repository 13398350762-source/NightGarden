using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    public InputField inputUN;
    public InputField inputPW;
    public InputField inputSU;
    public Button btnRe;
    public Button btnBack;
    protected override void Init()
    {
        btnBack.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<RegisterPanel>(() =>
            {
                inputUN.text = string.Empty;
                inputPW.text = string.Empty;
                inputSU.text = string.Empty;
            });
            UIManager.Instance.ShowPanel<LoginPanel>();
        });

        btnRe.onClick.AddListener(() =>{

        });
    }
}
