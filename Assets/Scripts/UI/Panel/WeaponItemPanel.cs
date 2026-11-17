using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WeaponItemPanel : BasePanel
{
    public Button btnSell;
    public Button btnComposite;
    private bool isInit=false;
    internal int index;
    private int showFrame;
    internal bool isClosing;
    internal WeaponData selectedWeapon;
    protected override void Init()
    {
        if (isInit)
            return;
        EventCallBack.Instance.Bind(btnSell, nameof(EventCallBack.SellWeapon), this);
        EventCallBack.Instance.Bind(btnComposite, nameof(EventCallBack.CompositeWeapon), this);
        isInit = true;
    }

    public void SetIndex(int index, RectTransform target)
    {
        WeaponData[] weapons = PlayerInfoManager.Instance.weaponList;
        if (index < 0 || index >= weapons.Length || weapons[index] == null || target == null) return;
        this.index = index;
        selectedWeapon = weapons[index];
        Show();
        transform.SetAsLastSibling();
        RectTransform rect = transform as RectTransform;
        RectTransform parent = transform.parent as RectTransform;
        Canvas.ForceUpdateCanvases();
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);
        Canvas sourceCanvas = target.GetComponentInParent<Canvas>();
        Canvas popupCanvas = GetComponentInParent<Canvas>();
        Camera sourceCamera = sourceCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : sourceCanvas.worldCamera;
        Camera popupCamera = popupCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : popupCanvas.worldCamera;
        Vector2 leftTop;
        Vector2 rightTop;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,
            RectTransformUtility.WorldToScreenPoint(sourceCamera, corners[1]), popupCamera, out leftTop);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,
            RectTransformUtility.WorldToScreenPoint(sourceCamera, corners[2]), popupCamera, out rightTop);
        float width = rect.rect.width;
        float height = rect.rect.height;
        float x = rightTop.x + 8f + width * rect.pivot.x;
        if (x + width * (1f - rect.pivot.x) > parent.rect.xMax)
            x = leftTop.x - 8f - width * (1f - rect.pivot.x);
        float y = rightTop.y - height * (1f - rect.pivot.y);
        x = Mathf.Clamp(x, parent.rect.xMin + width * rect.pivot.x, parent.rect.xMax - width * (1f - rect.pivot.x));
        y = Mathf.Clamp(y, parent.rect.yMin + height * rect.pivot.y, parent.rect.yMax - height * (1f - rect.pivot.y));
        rect.localPosition = new Vector3(x, y, rect.localPosition.z);
    }

    public override void Show()
    {
        base.Show();
        showFrame = Time.frameCount;
        isClosing = false;
        btnSell.interactable = true;
        btnComposite.interactable = true;
    }

    public override void Hide(UnityAction action)
    {
        isClosing = true;
        btnSell.interactable = false;
        btnComposite.interactable = false;
        base.Hide(action);
    }

    protected override void Update()
    {
        base.Update();

        // 避免打开面板的这次点击，又立即把面板关闭。
        if (isClosing || Time.frameCount == showFrame)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            RectTransform rect = transform as RectTransform;
            Canvas canvas = GetComponentInParent<Canvas>();
            Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

            if (!RectTransformUtility.RectangleContainsScreenPoint(
                rect, Input.mousePosition, camera))
            {
                UIManager.Instance.HidePanel<WeaponItemPanel>();
            }
        }
    }
}
