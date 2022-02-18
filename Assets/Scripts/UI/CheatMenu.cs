using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class CheatMenu : MonoBehaviour
{
    [SerializeField] Text _timeScaleText;
    [SerializeField] Scrollbar _timeScaleScroll;

    [SerializeField] Button _infObjectiveButton;
    [SerializeField] Button _resetButton;

    [SerializeField] LevelTargetPool _debugTargetPool;


    FieldInfo _targetPoolMember;
    MethodInfo _targetChangeMeth;

    object _savedTargetPool = null;

    // Start is called before the first frame update
    void Start()
    {
        _timeScaleText.text = "时间速度";
        _timeScaleScroll.onValueChanged.AddListener(SetTimeScale);
        _timeScaleScroll.value = Time.timeScale;

        _infObjectiveButton.GetComponentInChildren<Text>().text = "使用调试任务表";
        _infObjectiveButton.onClick.AddListener(SetInfObjective);
        _resetButton.GetComponentInChildren<Text>().text = "恢复(有bug)";
        _resetButton.onClick.AddListener(Reset);

        _targetPoolMember = typeof(GameMgr).GetField("_targetPool", BindingFlags.Instance | BindingFlags.NonPublic);
        _targetChangeMeth = typeof(GameMgr).GetMethod("GiftTargetChange", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
