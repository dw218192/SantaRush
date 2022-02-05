using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour, IWagonCollisionHandler
{
    public enum GameState
    {
        PAUSED,
        RUNNING,
        OVER
    }

    [SerializeField] LevelStageTable _stageTable = null;
    [SerializeField] LevelTargetPool _targetPool = null;

    #region SpeedChange States
    // runtime
    float _totalSpeedChangeTime = 0;
    int _curSpeedChangeInterval = 0;
    public float LevelTime { get; private set; }
    LevelStageTable.SpeedChangeStat _speedChangeStat = null;
    #endregion

    #region Objective States
    LevelTargetPool.LevelTargetDesc _curTarget = null;
    // time left for current gift objective
    public float GiftTime { get; private set; }

    // score of current gift target
    int _giftTargetScore;
    public int GiftTargetScore
    { 
        get => _giftTargetScore;
        private set
        {
            _giftTargetScore = value;
            GameConsts.eventManager.InvokeEvent(typeof(IGameScoreHandler),
                new GameScoreEventData(GameScoreEventData.Type.GIFT_TARGET_SCORE, value, _curTarget.giftNum));
        }
    }
    #endregion

    // total score
    int _score;
    public int Score 
    {
        get => _score;
        private set
        {
            _score = value;
            GameConsts.eventManager.InvokeEvent(typeof(IGameScoreHandler),
                new GameScoreEventData(GameScoreEventData.Type.TOTAL_SCORE, value, 0));
        }
    }

    GameState _state;
    public GameState State 
    {
        get => _state;
        private set
        {
            if (value == _state)
                return;
            switch(value)
            {
                case GameState.RUNNING:
                    Time.timeScale = 1;
                    break;
                case GameState.PAUSED:
                    Time.timeScale = 0;
                    break;
                case GameState.OVER:
                    Time.timeScale = 0;
                    break;
            }
            _state = value;
        }
    }

    public float TileSpeedMultiplier { get; private set; }

    void Awake()
    {
        if(!GameConsts.gameManager)
        {
            GameConsts.gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _speedChangeStat = _stageTable.speedChangeStat;
        _totalSpeedChangeTime = _speedChangeStat.numIntervals * _speedChangeStat.intervalLength;
        _curSpeedChangeInterval = 0;

        LevelTime = 0;
        Score = 0;
        GiftTargetChange();

        State = GameState.RUNNING;
    }

    // Update is called once per frame
    void Update()
    {
        if (State != GameState.RUNNING)
            return;

        LevelTime += Time.deltaTime;
        GiftTime -= Time.deltaTime;

        if(_curSpeedChangeInterval <= _speedChangeStat.numIntervals)
        {
            if (LevelTime >= _speedChangeStat.intervalStartTime + _curSpeedChangeInterval * _speedChangeStat.intervalLength)
            {
                SpeedChange();
            }
        }
        
        if(GiftTargetScore >= _curTarget.giftNum)
        {
            GiftTargetChange();
        }
        else if (GiftTime <= 0)
        {
            FailGame();
        }
    }

    private void OnGUI()
    {
        GUILayout.Label(Application.version);
    }

    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {
            PauseGame();
        }
    }

    void SpeedChange()
    {
        ++ _curSpeedChangeInterval;
        float curMultiplier = Mathf.Lerp(_speedChangeStat.initialMultiplier, _speedChangeStat.finalMultiplier, 
            Mathf.Clamp01((LevelTime - _speedChangeStat.intervalStartTime) / _totalSpeedChangeTime));

        GameConsts.eventManager.InvokeEvent(typeof(ILevelStageHandler), new LevelStageEventData(curMultiplier));
    }

    void GiftTargetChange()
    {
        _curTarget = _targetPool != null ? _targetPool.GetNextTarget() : null;

        GiftTargetScore = 0;
        GiftTime += _curTarget.duration;
    }

    public void AddScore(int delta)
    {
        GiftTargetScore += delta;
        Score += delta;
    }

    public void FailGame()
    {
        State = GameState.OVER;
        GameConsts.uiMgr.OpenMenu(EndScreen.Instance);
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }

    public void ResumeGame()
    {
        State = GameState.RUNNING;
    }

    public void PauseGame()
    {
        if (State == GameState.PAUSED)
            return;
        State = GameState.PAUSED;
        GameConsts.uiMgr.OpenMenu(PauseMenu.Instance);
    }

    public void RestartGame()
    {
        State = GameState.OVER;
        SceneManager.LoadScene(GameConsts.k_MainSceneIndex);
    }

    public void OnWagonCollide(WagonCollisionEventData eventData)
    {
        if (eventData.partCount == 1)
            FailGame();
    }
}
