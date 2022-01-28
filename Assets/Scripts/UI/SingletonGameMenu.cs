using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IGameMenu
{
    void OnEnterMenu();
    void OnLeaveMenu();
    void OnBackPressed();
    void OnFocus();
    void OnLoseFocus();
    bool CanClose();
    bool CanOpen();
}

public abstract class GameMenu : MonoBehaviour, IGameMenu
{
    List<Graphic> _raycastTargets;

    protected virtual void Awake()
    {
        _raycastTargets = new List<Graphic>();
        Graphic[] graphics = GetComponentsInChildren<Graphic>();
        foreach (var graphic in graphics)
        {
            if (graphic.raycastTarget)
                _raycastTargets.Add(graphic);
        }
    }

    protected virtual void Start()
    {
        gameObject.SetActive(false);
        GameConsts.uiMgr.RegisterMenu(this);
    }

    public virtual void OnEnterMenu()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public virtual void OnFocus()
    {
        foreach (var target in _raycastTargets)
            target.raycastTarget = true;

        transform.SetAsLastSibling();
    }

    public virtual void OnLoseFocus()
    {
        foreach (var target in _raycastTargets)
            target.raycastTarget = false;
    }

    public virtual void OnLeaveMenu()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnBackPressed()
    {
        if (GameConsts.uiMgr != null)
        {
            GameConsts.uiMgr.CloseCurrentMenu();
        }
    }

    public virtual bool CanOpen()
    {
        return true;
    }

    public virtual bool CanClose()
    {
        return true;
    }
}

[DisallowMultipleComponent]
public abstract class SingletonGameMenu<MenuType> : GameMenu where MenuType : SingletonGameMenu<MenuType>
{
    private static MenuType _Instance;
    public static MenuType Instance { get { return _Instance; } }

    protected override void Awake()
    {
        base.Awake();

        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _Instance = (MenuType)this;
        }
    }

    protected virtual void OnDestroy()
    {
        if (_Instance == this)
        {
            _Instance = null;
        }
    }
}