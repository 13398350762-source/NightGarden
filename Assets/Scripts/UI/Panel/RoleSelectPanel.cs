using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoleSelectPanel : BasePanel
{
    public Button btnBack;
    public Button btnSure;
    public List<Toggle> togList;
    public List<RoleData> roleList;
    public List<Text> txtStateList;
    public Text txtName;
    public Text txtRemark;
    public Text txtPassive;
    public Text LockCondition;
    public Text txtLockCondition;
    public ToggleGroup toggleGroup;
    private bool isInit = false;
    protected override void Init()
    {
        if (isInit)
            return;
        toggleGroup.allowSwitchOff = true;
        btnBack.onClick.AddListener(() => {
            UIManager.Instance.HidePanel<RoleSelectPanel>();
            UIManager.Instance.ShowPanel<MainPanel>();
        });
        btnSure.onClick.AddListener(() =>{
            //如果有角色被选中
            if (PlayerInfoManager.Instance.GetRole() == null)
            {
                UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                    panel.ChangeInfo("请选择角色");
                });
                return;
            }
            if (PlayerInfoManager.Instance.GetRole() == roleList[3] && !UserManager.Instance.IsChiliUnlock())
            {
                UIManager.Instance.ShowPanel<TipPanel>((panel) => {
                    panel.ChangeInfo("角色未解锁");
                });
                return;
            }
            if (PlayerInfoManager.Instance.GetRole() == roleList[4] && !UserManager.Instance.IsPeaUnlock())
            {
                UIManager.Instance.ShowPanel<TipPanel>((panel) => {
                    panel.ChangeInfo("角色未解锁");
                });
                return;
            }
            UIManager.Instance.ShowPanel<WeaponSelectPanel>();
            UIManager.Instance.HidePanel<RoleSelectPanel>();
        });
        togList[0].onValueChanged.AddListener((value) => {
            if(value)
            ChangeRoleInfo(roleList[0]);
        });
        togList[1].onValueChanged.AddListener((value) => {
            if(value)
            ChangeRoleInfo(roleList[1]);
        });
        togList[2].onValueChanged.AddListener((value) => {
            if(value)
            ChangeRoleInfo(roleList[2]);
        });
        togList[3].onValueChanged.AddListener((value) => {
            if(value)
            ChangeRoleInfo(roleList[3]);
        });
        togList[4].onValueChanged.AddListener((value) => {
            if(value)
            ChangeRoleInfo(roleList[4]);
        });
        toggleGroup.SetAllTogglesOff();
        isInit = true;
    }
    private void ChangeRoleInfo(RoleData data)
    {
        txtName.text = data.roleID;
        txtPassive.text = data.passive;
        txtRemark.text = data.remark;
        for (int i = 0; i < 5; i++)
        {
            if (roleList[i]==data)
                txtStateList[i].text = "已选择";
            else
                txtStateList[i].text = "可使用";
        }
        if (!UserManager.Instance.IsChiliUnlock())
        {
            txtStateList[3].text = "未解锁";
        }
        if (!UserManager.Instance.IsPeaUnlock())
        {
            txtStateList[4].text = "未解锁";
        }
        switch (data.roleID)
        {
            case "辣椒游侠":
                LockCondition.enabled = true;
                txtLockCondition.enabled = true;
                txtLockCondition.text = "1";
                break;
            case "豌豆射手":
                LockCondition.enabled = true;
                txtLockCondition.enabled = true;
                txtLockCondition.text = "2";
                break;
            default:
                LockCondition.enabled = false;
                txtLockCondition.enabled = false;
                break;
        }
        PlayerInfoManager.Instance.SetRole(data);
    }

    public override void Show()
    {
        txtName.text = string.Empty;
        txtPassive.text = string.Empty;
        txtRemark.text = string.Empty;
        LockCondition.enabled = false;
        txtLockCondition.enabled = false;
        base.Show();
        for (int i = 0; i < 5; i++)
        {
            txtStateList[i].text = "可使用";
        }
        //检测用户是否解锁角色
        if (UserManager.Instance.IsChiliUnlock())
        {
            togList[3].transform.Find("imgLock").gameObject.SetActive(false);
        }
        else
        {
            togList[3].transform.Find("imgLock").gameObject.SetActive(true);
            txtStateList[3].text = "未解锁";
        }

        if (UserManager.Instance.IsPeaUnlock())
        {
            togList[4].transform.Find("imgLock").gameObject.SetActive(false);
        }
        else
        {
            togList[4].transform.Find("imgLock").gameObject.SetActive(true);
            txtStateList[4].text = "未解锁";
        }
        SetRoleCardAlpha(3, UserManager.Instance.IsChiliUnlock() ? 1f : 0.5f);
        SetRoleCardAlpha(4, UserManager.Instance.IsPeaUnlock() ? 1f : 0.5f);
    }

    private void SetRoleCardAlpha(int index, float alpha)
    {
        CanvasGroup group = togList[index].GetComponent<CanvasGroup>();
        if (group == null)
            group = togList[index].gameObject.AddComponent<CanvasGroup>();
        group.alpha = alpha;
    }

    public override void Hide(UnityAction action)
    {
        txtName.text = string.Empty;
        txtPassive.text = string.Empty;
        txtRemark.text = string.Empty;
        LockCondition.enabled = false;
        txtLockCondition.enabled = false;
        toggleGroup.SetAllTogglesOff();
        PlayerInfoManager.Instance.SetRole(null);
        base.Hide(action);
    }

}
