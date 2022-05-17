using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : MonoBehaviour
{
    //a linkedlist that behaves like a stack
    LinkedList<IGameMenu> _MenuStack = new LinkedList<IGameMenu>();
    List<IGameMenu> _MenuInstances = new List<IGameMenu>();
    Transform _MenuParent = null;

    private void Awake()
    {
        Debug.Assert(GameConsts.eventManager);

        if (GameConsts.uiMgr != null)
            Destroy(this);
        else
            GameConsts.uiMgr = this;

        _MenuParent = new GameObject("GameMenus").transform;

        GameObject[] objs = Resources.LoadAll<GameObject>(GameConsts.k_ResourcesUIPrefabPath);
        foreach(GameObject obj in objs)
        {
            IGameMenu menu = obj.GetComponent<IGameMenu>();
            if(menu != null)
            {
                GameObject ins = Instantiate(obj, _MenuParent);

                IGameMenu insMenu = ins.GetComponent<IGameMenu>();
                GameConsts.eventManager.Register(insMenu as MonoBehaviour);
                RegisterMenu(insMenu);
                ins.SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RegisterMenu(IGameMenu menu)
    {
        bool found = false;
        foreach (var ins in _MenuInstances)
        {
            if (ReferenceEquals(ins, menu))
            {
                found = true;
            }
        }

        if (!found)
        {
            _MenuInstances.Add(menu);
        }
    }

    public void OpenMenu(IGameMenu menuInstance)
    {
        if (menuInstance == null)
        {
            Debug.LogWarning("MenuManager.OpenMenu()- menu instance is null");
            return;
        }

        if (!menuInstance.CanOpen())
        {
            return;
        }

        if (_MenuStack.Count > 0)
        {
            for (var curNode = _MenuStack.First; curNode != null; curNode = curNode.Next)
            {
                if (ReferenceEquals(curNode.Value, menuInstance))
                {
                    //already opened
                    _MenuStack.Remove(curNode);
                    break;
                }
            }

            if (_MenuStack.Count > 0)
            {
                _MenuStack.First.Value.OnLoseFocus();
            }
        }

        menuInstance.OnEnterMenu();
        _MenuStack.AddFirst(menuInstance);
    }
    public void CloseCurrentMenu()
    {
        if (_MenuStack.Count == 0 || !GetActiveMenu().CanClose())
        {
            return;
        }

        IGameMenu topMenu = _MenuStack.First.Value;
        _MenuStack.RemoveFirst();
        topMenu.OnLeaveMenu();

        if (_MenuStack.Count > 0)
        {
            IGameMenu nextMenu = _MenuStack.First.Value;
            nextMenu.OnFocus();
        }
    }
    public IGameMenu GetActiveMenu()
    {
        if (_MenuStack.Count > 0)
        {
            return _MenuStack.First.Value;
        }
        else
        {
            return null;
        }
    }
    public int GetOpenMenuCount()
    {
        return _MenuStack.Count;
    }
}
