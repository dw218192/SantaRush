using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FSM;

using Debug = UnityEngine.Debug;
using System.Text;

public class GameMgr : MonoBehaviour, IWagonCollisionHandler, IBuffStateHandler
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
                new GameScoreEventData(GameScoreEventData.Type.GIFT_TARGET_SCORE_CHANGE, value, _curTarget.giftNum));
        }
    }
    #endregion

    #region GiftBonus
    class GiftBonusState : IComparable<GiftBonusState>, IEquatable<GiftBonusState>
    {
        public static readonly GiftBonusState Invalid = new GiftBonusState(-1);
        public static GiftBonusState[] Stages { get; set; }
        public static GiftBonusState NoBonus { get; set; }
        public static void Init(LevelTargetPool.BonusStageDesc[] descs)
        {
            int id = Invalid._val + 1;
            NoBonus = new GiftBonusState(id++);

            Stages = new GiftBonusState[descs.Length];
            for (int i = 0; i < descs.Length; ++i)
                Stages[i] = new GiftBonusState(id++);
        }

        int _val;
        public GiftBonusState(int val)
        {
            this._val = val;
        }
        public int CompareTo(GiftBonusState other)
        {
            return _val.CompareTo(other._val);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(GiftBonusState other)
        {
            if (other == null)
                return false;
            return _val == other._val;
        }
        public override int GetHashCode()
        {
            return 339947245 + _val.GetHashCode();
        }
    }

    class ScoreEventWindow
    {
        // record 1000 seconds in game time at most
        readonly float _maxWindowDuration; 
        
        public float timer;
        LinkedList<float> _window = new LinkedList<float>();
        float Duration { get => _window.Count > 0 ? _window.Last.Value - _window.First.Value : 0; }

        public ScoreEventWindow(float maxWindowDuration)
        {
            _maxWindowDuration = maxWindowDuration;
            timer = 0;
        }

        public void Reset()
        {
            _window.Clear();
            timer = 0;
        }

        public void AddScoreEvent()
        {
            _window.AddLast(timer);

            // window length exceeds limit
            if (Duration > _maxWindowDuration) //evict first
                _window.RemoveFirst();

            DEBUG_CheckInvariant();
        }
        /*
         * check if a player scores for [numTimes] times consecutively in the past [numSecs] seconds,
         * with each scoring event less than [interval] seconds apart
        */
        public bool Check(int targetCount, float targetSecs, float interval)
        {
            // empty window
            if (_window.Count == 0)
                return false;

            // last scoring event is more than [interval] away
            if (timer - interval > _window.Last.Value)
                return false;

            DEBUG_CheckInvariant();

            int count = 1;
            float secs = timer - _window.Last.Value;
            LinkedListNode<float> node = _window.Last;
            while(node != null && secs < targetSecs)
            {
                LinkedListNode<float> prevNode = node.Previous;
                if (prevNode == null)
                    break;
                if (node.Value - prevNode.Value > interval)
                    break;

                ++count;
                if (count >= targetCount)
                    break;

                secs += node.Value - prevNode.Value;
                node = node.Previous;
            }

            return count >= targetCount;
        }

        [Conditional("DEBUG")]
        public void DEBUG_CheckInvariant()
        {
            if (_window.Count == 0)
                return;
            // duration must be less than maximum duration
            if (Duration > _maxWindowDuration)
                Debug.Break();

            // must be strictly increasing
            LinkedListNode<float> prev = null, node = _window.First;
            while(node != null)
            {
                if(prev != null && node.Value < prev.Value)
                {
                    // invariant violation
                    Debug.Break();
                }

                prev = node;
                node = node.Next;
            }
        }

        [Conditional("DEBUG")]
        public void DEBUG_OnGUI()
        {
            GUILayout.Label("=====ScoreEventWindow DEBUG info=====");

            StringBuilder str = new StringBuilder();
            
            str.Append("[ ");
            foreach (float f in _window)
            {
                str.Append(f.ToString("0.00"));
                str.Append(", ");
            }
            if(str.Length >= 4)
                str.Remove(str.Length - 2, 2);
            str.Append(" ]");

            GUILayout.Label(str.ToString());
            GUILayout.Label(timer.ToString("0.00"));
        }
    }

    // max value in _scoreEventsWindow minus min value in _scoreEventsWindow
    ScoreEventWindow _scoreEventWindow;
    LevelTargetPool.BonusStageDesc _curBonusStage = null;
    LevelTargetPool.BonusStageDesc CurBonusStage
    {
        get => _curBonusStage;
        set
        {
            _curBonusStage = value;

            if (value == null)
                GameConsts.eventManager.InvokeEvent(typeof(IBonusStateHanlder), new BonusStateEventData("",false,0));
            else
                GameConsts.eventManager.InvokeEvent(typeof(IBonusStateHanlder), new BonusStateEventData(value.DisplayName, true, value.ScoreMultiplier));
        }
    }
    float _curStageTimer = 0;

    // bonus stage state machine
    StateMachine<GiftBonusState> _bonusStageFSM = new StateMachine<GiftBonusState>();

    class SimpleBuffStateMachine
    {
        enum BuffFSMState
        {
            NOT_APPLIED,
            APPLIED
        }

        StateMachine<BuffFSMState> _fsm = null;
        int _giftCount = 0;
        int _targetCount = 0;
        float _timer = 0;
        GameMgr _gameMgr = null;
        PlayerBuffDesc _buffDesc;

        SimpleBuffStateMachine() { }
        public SimpleBuffStateMachine(GameMgr gameMgr, PlayerBuffDesc buffDesc, int targetScore, float duration)
        {
            _fsm = new StateMachine<BuffFSMState>();
            _buffDesc = buffDesc;
            _targetCount = targetScore;
            _gameMgr = gameMgr;
            _gameMgr._onScoreChangeEvent += SetGiftCount;
            
            _fsm.AddState(BuffFSMState.NOT_APPLIED, onEnter: OnEnterStateNotApplied);
            _fsm.AddState(BuffFSMState.APPLIED, onEnter: OnEnterStateApplied, onLogic: OnStateApplied);
            _fsm.AddTransition(BuffFSMState.NOT_APPLIED, BuffFSMState.APPLIED, condition: CheckGiftCount);
    
            if (Mathf.Approximately(0, duration))
                _fsm.AddTransition(BuffFSMState.APPLIED, BuffFSMState.NOT_APPLIED, (t) => { return true; });
            else
                _fsm.AddTransition(BuffFSMState.APPLIED, BuffFSMState.NOT_APPLIED, (t)=> { return _timer >= _buffDesc.Duration; });

            _fsm.SetStartState(BuffFSMState.NOT_APPLIED);
            _fsm.Init();
        }
        ~SimpleBuffStateMachine()
        {
            _gameMgr._onScoreChangeEvent -= SetGiftCount;
        }

        void SetGiftCount(int newScore, int prevScore)
        {
            if(_fsm.ActiveStateName == BuffFSMState.NOT_APPLIED)
                _giftCount += (newScore - prevScore);
        }

        bool CheckGiftCount(Transition<BuffFSMState> transition)
        {
            return _giftCount >= _targetCount;
        }

        [StateMachineFunction]
        void OnEnterStateNotApplied(State<BuffFSMState> state)
        {
            GameConsts.eventManager.InvokeEvent(typeof(IBuffStateHandler), new BuffStateEventData(false, _buffDesc));

            _giftCount = 0;
        }
        [StateMachineFunction]
        void OnEnterStateApplied(State<BuffFSMState> state)
        {
            GameConsts.eventManager.InvokeEvent(typeof(IBuffStateHandler), new BuffStateEventData(true, _buffDesc));

            _timer = 0;
        }
        [StateMachineFunction]
        void OnStateApplied(State<BuffFSMState> state)
        {
            _timer += Time.deltaTime;
        }

        public void OnLogic()
        {
            _fsm.OnLogic();
        }

        [Conditional("DEBUG")]
        public void DEBUG_OnGUI()
        {
            GUILayout.Label($"====={_buffDesc.Name} DEBUG info=====");
            GUILayout.Label($"current state: {_fsm.ActiveStateName.ToString()}");
            GUILayout.Label($"timer:{_timer.ToString("0.0")}/{_buffDesc.Duration.ToString("0.0")}");
            GUILayout.Label($"gift count: {_giftCount}/{_targetCount}");
        }

        [Conditional("DEBUG")]
        public void DEBUG_ForceApply()
        {
            _fsm.RequestStateChange(BuffFSMState.APPLIED, true);
        }
    }

    // player super status buff state machine
    SimpleBuffStateMachine _superStatusFSM = null;
    // player hp regen buff state machine
    SimpleBuffStateMachine _hpBonusFSM = null;

    [StateMachineFunction]
    bool TransitionAnyToNoBonus(Transition<GiftBonusState> transition)
    {
        return _curStageTimer >= _curBonusStage.TimeLimit;
    }

    [StateMachineFunction]
    bool TransitionNoBonusToFirstStage(Transition<GiftBonusState> transition)
    {
        return _scoreEventWindow.Check(_targetPool.BonusConfig.BonusStartGiftNum, _targetPool.BonusConfig.BonusStartTimeLimit, float.PositiveInfinity);
    }

    [StateMachineFunction]
    bool TransitionBetweenStages(Transition<GiftBonusState> transition)
    {
        return _scoreEventWindow.Check(_curBonusStage.TargetScore, _curBonusStage.TimeLimit, _curBonusStage.TimeBetweenScores);
    }
    
    [StateMachineFunction]
    void OnEnterNoBonus(State<GiftBonusState> state)
    {
        _scoreEventWindow.Reset();
        _targetPool.ResetBonusStage();
        CurBonusStage = null;
        _curStageTimer = 0;
    }

    [StateMachineFunction]
    void OnEnterStage(State<GiftBonusState> state)
    {
        _scoreEventWindow.Reset();
        CurBonusStage = _targetPool.GetNextBonusStage();
        _curStageTimer = 0;
    }

    [StateMachineFunction]
    void OnStage(State<GiftBonusState> state)
    {
        _curStageTimer += Time.deltaTime;
        _scoreEventWindow.timer += Time.deltaTime;
    }

#if DEBUG
    Dictionary<GiftBonusState, string> _stateToName = new Dictionary<GiftBonusState, string>();
#endif
    [Conditional("DEBUG")]
    void DEBUG_Init_StateToName(GiftBonusState[] states)
    {
#if DEBUG
        _stateToName[GiftBonusState.NoBonus] = "NoBonus";
        for (int i = 0; i < states.Length; ++i)
            _stateToName[states[i]] = $"Stage {i}";
#endif
    }
    [Conditional("DEBUG")]
    void DEBUG_BonusFSM_OnGUI()
    {
#if DEBUG
        GUILayout.Label($"current state: {_stateToName[_bonusStageFSM.ActiveState.name]}");
#endif
    }
    #endregion


    // total score (total number of gifts obtained)

    // internal score events
    delegate void ScoreChangeHandler(int newScore, int prevScore);
    event ScoreChangeHandler _onScoreChangeEvent;

    int _score;
    public int Score 
    {
        get => _score;
        private set
        {
            int prevScore = _score;
            _score = value;
            _onScoreChangeEvent?.Invoke(_score, prevScore);
            GameConsts.eventManager.InvokeEvent(typeof(IGameScoreHandler),
                new GameScoreEventData(GameScoreEventData.Type.TOTAL_SCORE_CHANGE, value));
        }
    }

    // total score (actual player score with bonus stuff added)
    float _globalPlayerScoreMultiplier = 1;
    public float GlobalPlayerScoreMultiplier { get => _globalPlayerScoreMultiplier; private set => _globalPlayerScoreMultiplier = value; }

    int _playerScore;
    public int PlayerScore
    {
        get => _playerScore;
        private set
        {
            _playerScore = value;
            GameConsts.eventManager.InvokeEvent(typeof(IGameScoreHandler),
                new GameScoreEventData(GameScoreEventData.Type.TOTAL_PLAYER_SCORE_CHANGE, value));
        }
    }

    // the highest score of the player ever recoreded
    public int HighestPlayerScore
    {
        get;
        private set;
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
    public float GlobalSpeedIncrease { get; private set; }

    public LevelStageTable StageTable { get => _stageTable; }

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
        void GameInit()
        {
            _speedChangeStat = _stageTable.SpeedChangeConfig;
            _totalSpeedChangeTime = _speedChangeStat.numIntervals * _speedChangeStat.intervalLength;
            _curSpeedChangeInterval = 0;
        }

        void GiftTargetInit()
        {
            LevelTime = 0;
            Score = 0;
            PlayerScore = 0;
            GiftTargetChange();
        }

        void BonusFSMInit()
        {
            if (_targetPool.BonusStages.Length == 0)
            {
                Debug.LogError("No Bonus stages were set!");
                Debug.Break();
            }
            else
            {
                int numStages = _targetPool.BonusStages.Length;
                float maxDuration = 0;
                for (int i = 0; i < numStages; ++i)
                    maxDuration = Mathf.Max(maxDuration, _targetPool.BonusStages[i].TimeLimit);
                _scoreEventWindow = new ScoreEventWindow(maxDuration + 10f);

                // initialize static state objects
                GiftBonusState.Init(_targetPool.BonusStages);

                // create states
                _bonusStageFSM.AddState(GiftBonusState.NoBonus, new State<GiftBonusState>(onEnter: OnEnterNoBonus));
                for (int i = 0; i < numStages; ++i)
                    _bonusStageFSM.AddState(GiftBonusState.Stages[i], onEnter: OnEnterStage, onLogic: OnStage);

                // no bonus --> fisrt
                _bonusStageFSM.AddTransition(GiftBonusState.NoBonus, GiftBonusState.Stages[0], condition: TransitionNoBonusToFirstStage);
                // first --> second --> third, etc
                for (int i = 1; i < numStages; ++i)
                {
                    _bonusStageFSM.AddTransition(GiftBonusState.Stages[i - 1], GiftBonusState.Stages[i], condition: TransitionBetweenStages);
                }
                // any --> no bonus
                _bonusStageFSM.AddTransitionFromAny(new Transition<GiftBonusState>(GiftBonusState.Invalid, GiftBonusState.NoBonus, condition: TransitionAnyToNoBonus));

                // last --> last
                GiftBonusState lastStageState = GiftBonusState.Stages[numStages - 1];
                LevelTargetPool.BonusStageDesc lastStageDesc = _targetPool.BonusStages[numStages - 1];
                _bonusStageFSM.AddTransition(lastStageState, lastStageState, condition: TransitionBetweenStages);

                _bonusStageFSM.SetStartState(GiftBonusState.NoBonus);
                _bonusStageFSM.Init();

                DEBUG_Init_StateToName(GiftBonusState.Stages);
            }
        }

        void BuffFSMInit()
        {
            PlayerBuffDesc superStatusBuff = new PlayerBuffDesc(
                PlayerBuffType.SUPER_STATUS, 
                _targetPool.PlayerBuffConfig.SuperStatusName, 
                _targetPool.PlayerBuffConfig.SuperStatusTime);

            PlayerBuffDesc hpRewardBuff = new PlayerBuffDesc(PlayerBuffType.HP_REWARD, "", 0f);

            _superStatusFSM = new SimpleBuffStateMachine(this, superStatusBuff, _targetPool.PlayerBuffConfig.SuperStatusGiftNum, _targetPool.PlayerBuffConfig.SuperStatusTime);
            _hpBonusFSM = new SimpleBuffStateMachine(this, hpRewardBuff, _targetPool.PlayerBuffConfig.HPRewardGiftNum, 0f);
        }

        void PlayerSaveInit()
        {
            HighestPlayerScore = PlayerPrefs.GetInt(GameConsts.k_PlayerPrefHighestScore, 0);
        }

        GameInit();
        GiftTargetInit();
        // FSM initialization
        BonusFSMInit();
        BuffFSMInit();
        PlayerSaveInit();

        State = GameState.RUNNING;
    }

    // Update is called once per frame
    void Update()
    {
        if (State != GameState.RUNNING)
            return;

        LevelTime += Time.deltaTime;
        GiftTime = Mathf.Max(0, GiftTime - Time.deltaTime);

        if(_curSpeedChangeInterval <= _speedChangeStat.numIntervals)
        {
            if (LevelTime >= _speedChangeStat.intervalStartTime + _curSpeedChangeInterval * _speedChangeStat.intervalLength)
            {
                SpeedChange();
            }
        }
        
        // gift score updates
        if(GiftTargetScore >= _curTarget.giftNum)
        {
            GiftTargetChange();
        }
        else if (Mathf.Approximately(GiftTime, 0))
        {
            FailGame();
        }


        // bonus stage updates
        _bonusStageFSM.OnLogic();

        // buff updates
        _hpBonusFSM.OnLogic();
        _superStatusFSM.OnLogic();
    }

    private void OnGUI()
    {
        GUILayout.Label($"{Application.version} { ( GameConsts.IsOnMobile() ? "Mobile" : "PC") }");

        _scoreEventWindow.DEBUG_OnGUI();
        DEBUG_BonusFSM_OnGUI();
        _superStatusFSM.DEBUG_OnGUI();
        _hpBonusFSM.DEBUG_OnGUI();
#if DEBUG
        GUILayout.Label("=====GameMgr DEBUG info=====");
        GUILayout.Label($"Global Score Multiplier = {GlobalPlayerScoreMultiplier.ToString("0.00")}");
        GUILayout.Label($"Global Speed Increase = {GlobalSpeedIncrease.ToString("0.00")}");
#endif
    }

    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {
            // do not show pause menu if the player is at end game screen
            if (ReferenceEquals(GameConsts.uiMgr.GetActiveMenu(), EndScreen.Instance))
                return;

            PauseGame();
        }
    }

    void SpeedChange()
    {
        ++ _curSpeedChangeInterval;
        TileSpeedMultiplier = Mathf.Lerp(_speedChangeStat.initialMultiplier, _speedChangeStat.finalMultiplier, 
            Mathf.Clamp01((LevelTime - _speedChangeStat.intervalStartTime) / _totalSpeedChangeTime));

        GameConsts.eventManager.InvokeEvent(typeof(ILevelStageHandler), new LevelStageEventData(TileSpeedMultiplier, GlobalSpeedIncrease));
    }

    void GiftTargetChange()
    {
        _curTarget = _targetPool.GetNextTarget();

        GiftTargetScore = 0;
        if (float.MaxValue - _curTarget.duration <= GiftTime)
            GiftTime = float.MaxValue;
        else
            // GiftTime += _curTarget.duration;
            GiftTime = _curTarget.duration;
    }

    public void AddScore(int delta)
    {
        _scoreEventWindow.AddScoreEvent();
        
        // gift count
        ++ GiftTargetScore;
        ++ Score;

        // player score
        if (_curBonusStage != null)
            delta = _curBonusStage.GetModifiedScoreDelta(delta);

        PlayerScore += Mathf.RoundToInt(delta * GlobalPlayerScoreMultiplier);
    }

    public void FailGame()
    {
        State = GameState.OVER;

        // record player score if it is the new highest score
        if (_playerScore > HighestPlayerScore)
        {
            PlayerPrefs.SetInt(GameConsts.k_PlayerPrefHighestScore, _playerScore);
        }

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


    public void OnBuffStateChange(BuffStateEventData eventData)
    {
        if (eventData.buffEnabled)
        {
            if (eventData.desc.Type == PlayerBuffType.SUPER_STATUS)
            {
                GlobalSpeedIncrease = _targetPool.PlayerBuffConfig.SuperStatusSpeedIncrease;
                GlobalPlayerScoreMultiplier = _targetPool.PlayerBuffConfig.SuperStatusScoreMultiplier;
            }
        }
        else
        {
            if (eventData.desc.Type == PlayerBuffType.SUPER_STATUS)
            {
                GlobalPlayerScoreMultiplier = 1;
                GlobalSpeedIncrease = 0;
            }
        }
    }
}
