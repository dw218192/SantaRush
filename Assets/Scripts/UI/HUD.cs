using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUD : MonoBehaviour, IGameScoreHandler, IBonusStateHanlder, IBuffStateHandler
{
    [SerializeField] Text _giftTimerText;
    [SerializeField] Text _giftScoreText;
    [SerializeField] Text _totalScoreText;
    [SerializeField] Text _playerScoreText;
    [SerializeField] Text _bonusStageText;
    [SerializeField] Text _buffText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _giftTimerText.text = $"目标剩余时间: " + GameConsts.gameManager.GiftTime.ToString("n2");
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