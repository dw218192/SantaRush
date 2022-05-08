using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUD : SingletonUIObject<HUD>, IGameScoreHandler, IBonusStateHanlder, IBuffStateHandler
{
    [SerializeField] Image _panel;
    [SerializeField] Text _giftTimerText;
    [SerializeField] Text _giftScoreText;
    [SerializeField] Text _totalScoreText;
    [SerializeField] Text _playerScoreText;
    [SerializeField] Text _bonusStageText;
    [SerializeField] Text _buffText;

    Vector3[] _wordCorners = null;

    protected override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    void Update()
    {

        _giftTimerText.text = $"目标剩余时间: " + GameConsts.gameManager.GiftTime.ToString("n2");
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
                _totalScoreText.text = $"获得礼物数: {eventData.values[0]}";
                break;
            case GameScoreEventData.Type.GIFT_TARGET_SCORE_CHANGE:
                _giftScoreText.text = $"目标礼物数: {eventData.values[0]}/{eventData.values[1]}";
                break;
            case GameScoreEventData.Type.TOTAL_PLAYER_SCORE_CHANGE:
                _playerScoreText.text = $"玩家分数: {eventData.values[0]}";
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