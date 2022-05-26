using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUD : SingletonUIObject<HUD>, IGameScoreHandler, IBonusStateHanlder, IBuffStateHandler
{
    [SerializeField] Image _panel;
    [SerializeField] StringTextPair _giftTimerText;
    [SerializeField] StringTextPair _giftScoreText;
    // [SerializeField] StringTextPair _totalScoreText;
    [SerializeField] StringTextPair _playerScoreText;
    [SerializeField] Text _bonusStageText;
    [SerializeField] Text _buffText;

    [Header("timer text settings")]
    [SerializeField] Color _timerTextColorUrgent;
    [SerializeField] AnimationCurve _timeTextAnimCurve;
    [SerializeField] float _timeTextAnimStartTime = 3f;

    Color _initTimerTextColor;
    int _timerTextBaseFontSize;
    Vector3[] _wordCorners = null;

    protected override void Start()
    {
        base.Start();

        _initTimerTextColor = _giftTimerText.text.color;
        _timerTextBaseFontSize = _giftTimerText.text.fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        float time = GameConsts.gameManager.GiftTime;
        if (time < _timeTextAnimStartTime)
        {
            float scale = _timeTextAnimCurve.Evaluate(time);
            _giftTimerText.text.color = _timerTextColorUrgent;
            _giftTimerText.text.fontSize = Mathf.RoundToInt(_timerTextBaseFontSize * scale);
        }
        else
        {
            _giftTimerText.text.color = _initTimerTextColor;
            _giftTimerText.text.fontSize = _timerTextBaseFontSize;
        }

        _giftTimerText.Set(" ", time.ToString("n2"));
    }

    public float Height
    {
        get
        {
            if(_wordCorners == null)
            {
                _wordCorners = new Vector3[4];
                _panel.rectTransform.GetWorldCorners(_wordCorners);
                for(int i=0; i<4; ++i)
                    _wordCorners[i] = Camera.main.ScreenToWorldPoint(_wordCorners[i]);
            }
            return Mathf.Abs(_wordCorners[0].y - _wordCorners[1].y);
        }
    }

    public void OnGameScoreChange(GameScoreEventData eventData)
    {
        switch(eventData.type)
        {
            case GameScoreEventData.Type.TOTAL_SCORE_CHANGE:
                // _totalScoreText.Set($": {eventData.values[0]}");
                break;
            case GameScoreEventData.Type.GIFT_TARGET_SCORE_CHANGE:
                _giftScoreText.Set($": {eventData.values[0]} / {eventData.values[1]}");
                break;
            case GameScoreEventData.Type.TOTAL_PLAYER_SCORE_CHANGE:
                _playerScoreText.Set($": {eventData.values[0]}");
                break;
        }
    }

    public void OnBonusStateChange(BonusStateEventData eventData)
    {
        if (!eventData.hasBonus)
            _bonusStageText.text = "";
        else
            _bonusStageText.text = $"{eventData.bonusName}!! x{eventData.multiplier}";
    }

    public void OnBuffStateChange(BuffStateEventData eventData)
    {
        if (!eventData.buffEnabled)
            _buffText.text = "";
        else
            _buffText.text += " " + eventData.desc.Name;
    }
}