using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class CheatMenu : DebugBehaviour
{
#if UNITY_EDITOR
    [SerializeField] Text _timeScaleText;
    [SerializeField] Scrollbar _timeScaleScroll;

    [SerializeField] Button _infObjectiveButton;
    [SerializeField] Button _resetButton;
    [SerializeField] Button _superStatusButton;

    [SerializeField] Button _hideMenuButton;
    [SerializeField] GameObject[] _hideItems;

    [SerializeField] LevelTargetPool _debugTargetPool;


    FieldInfo _targetPoolMember;
    FieldInfo _superStatusFSMMember;
    MethodInfo _targetChangeMeth;

    object _savedTargetPool = null;
    bool _hidden = false;

    // Start is called before the first frame update
    void Start()
    {
        void ConfigBtn(Button but, string txt, UnityEngine.Events.UnityAction callback)
        {
            if(callback != null)
                but.onClick.AddListener(callback);
            but.GetComponentInChildren<Text>().text = txt;
        }

        _timeScaleText.text = "时间速度";
        _timeScaleScroll.onValueChanged.AddListener(SetTimeScale);
        _timeScaleScroll.value = Time.timeScale;

        ConfigBtn(_infObjectiveButton, "使用调试任务表", SetInfObjective);
        ConfigBtn(_resetButton, "恢复(有bug)", Reset);
        ConfigBtn(_superStatusButton, "启用头槌", EnableSuperStatus);
        ConfigBtn(_hideMenuButton, "X", ToggleHide);

        _targetPoolMember = typeof(GameMgr).GetField("_targetPool", BindingFlags.Instance | BindingFlags.NonPublic);
        _targetChangeMeth = typeof(GameMgr).GetMethod("GiftTargetChange", BindingFlags.Instance | BindingFlags.NonPublic);
        _superStatusFSMMember = typeof(GameMgr).GetField("_superStatusFSM", BindingFlags.Instance | BindingFlags.NonPublic);

        // start disabled
        ToggleHide();
    }


    void EnableSuperStatus()
    {
        object fsm = _superStatusFSMMember.GetValue(GameConsts.gameManager);
        var forceApplyMethod = fsm.GetType().GetMethod("DEBUG_ForceApply", BindingFlags.Public | BindingFlags.Instance);
        forceApplyMethod.Invoke(fsm, new object[] { });
    }

    void ToggleHide()
    {
        _hidden = !_hidden;
        foreach (var item in _hideItems)
            item.SetActive(!_hidden);
    }

    void SetTimeScale(float scale)
    {
        Time.timeScale = Mathf.Clamp(scale, 0, 1);
    }

    void SetInfObjective()
    {
        if (_debugTargetPool == null)
            return;
        _savedTargetPool = _targetPoolMember.GetValue(GameConsts.gameManager);
        _targetPoolMember.SetValue(GameConsts.gameManager, _debugTargetPool);
        _targetChangeMeth.Invoke(GameConsts.gameManager, new object[] { });
    }

    void Reset()
    {
        Time.timeScale = 1;
        if(_savedTargetPool != null)
        {
            _targetPoolMember.SetValue(GameConsts.gameManager, _savedTargetPool);
        }
    }
#endif
}
