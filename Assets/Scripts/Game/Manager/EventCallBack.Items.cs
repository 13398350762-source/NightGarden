using UnityEngine;

public partial class EventCallBack
{
    public void SellWeapon(WeaponItemPanel view)
    {
        if (view.isClosing || view.selectedWeapon == null || PlayerInfoManager.Instance.weaponList[view.index] != view.selectedWeapon) return;
        view.isClosing = true;
        view.btnSell.interactable = false;
        view.btnComposite.interactable = false;
        UIManager.Instance.HidePanel<WeaponItemPanel>();
        PlayerInfoManager.Instance.SellWeapon(view.index);
        view.selectedWeapon = null;
        RefreshShopInventory(false);
    }

    public void CompositeWeapon(WeaponItemPanel view)
    {
        if (view.isClosing || view.selectedWeapon == null || PlayerInfoManager.Instance.weaponList[view.index] != view.selectedWeapon) return;
        if (!PlayerInfoManager.Instance.CompositeWeapon(view.index)) return;
        view.isClosing = true;
        view.btnSell.interactable = false;
        view.btnComposite.interactable = false;
        view.selectedWeapon = null;
        UIManager.Instance.HidePanel<WeaponItemPanel>();
        RefreshShopInventory(false);
    }

    public void BuyAccessory(AccessoryShopItem view)
    {
        if (!view.btnBuy.interactable) return;
        //花钱买商品
        if (PlayerInfoManager.Instance.GetMoney() >= view.cost)
        {
            PlayerInfoManager.Instance.AddMoney(-view.cost);
            PlayerInfoManager.Instance.AddAccessory(view.data);
            //增加属性或被动
            AccessoryPassive.Instance.passiveDic[view.data.accessoryID]();
            view.RemoveFromShop();
            //更新面板
            RefreshShopInventory(true);
        }
        else
        {
            //播放购买失败音效
        }
    }

    public void BuyWeapon(WeaponShopItem view)
    {
        if (!view.btnBuy.interactable) return;
        //花钱买商品
        if (PlayerInfoManager.Instance.GetMoney() >= view.cost && PlayerInfoManager.Instance.SetWeapon(view.data))
        {
            PlayerInfoManager.Instance.AddMoney(-view.cost);
            view.RemoveFromShop();
            RefreshShopInventory(false);
        }
        else
        {
            //播放购买失败音效
        }
    }

    public void ChooseUpgrade(LevelUpItem view)
    {
        if (!LevelUpManager.Instance.Select(view)) return;

        UIManager.Instance.GetPanel<ShopPanel>(panel => panel.updateTXTDic());
        LevelUpManager.Instance.selectCount--;

        if (LevelUpManager.Instance.selectCount <= 0)
        {
            UIManager.Instance.HidePanel<LevelUpPanel>(() =>
            {
                UIManager.Instance.ShowPanel<ShopPanel>();
            });
            return;
        }

        UIManager.Instance.GetPanel<LevelUpPanel>(panel => panel.Refresh());
    }

    public void OpenWeapon(WeaponItem view)
    {
        WeaponData[] weapons = PlayerInfoManager.Instance.weaponList;
        if (isOpeningWeaponPanel || view.index < 0 || view.index >= weapons.Length || weapons[view.index] != view.data) return;
        isOpeningWeaponPanel = true;
        UIManager.Instance.ShowPanel<WeaponItemPanel>((panel) =>{
            isOpeningWeaponPanel = false;
            if (view == null || !view.gameObject.activeInHierarchy || PlayerInfoManager.Instance.weaponList[view.index] != view.data)
            {
                UIManager.Instance.HidePanel<WeaponItemPanel>();
                return;
            }
            panel.SetIndex(view.index, view.transform as RectTransform);
        });
    }
    private void RefreshShopInventory(bool accessory)
    {
        UIManager.Instance.GetPanel<ShopPanel>(panel => {
            if (accessory) panel.RefreshAccessory();
            else panel.RefreshWeapons();
            panel.updateTXTDic();
        });
    }
}
