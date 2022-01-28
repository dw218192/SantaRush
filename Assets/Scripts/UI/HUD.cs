using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUD : MonoBehaviour, IGameScoreHandler
{
    [SerializeField] Text _giftTimerText;
    [SerializeField] Text _giftScoreText;
    [SerializeField] Text _totalScoreText;

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
            case GameScoreEventData.Type.TOTAL_SCORE:
                _totalScoreText.text = $"总分数: {eventData.score}";
                break;
            case GameScoreEventData.Type.GIFT_TARGET_SCORE:
                _giftScoreText.text = $"目标分数: {eventData.score}/{eventData.targetScore}";
                break;
        }
    }
}