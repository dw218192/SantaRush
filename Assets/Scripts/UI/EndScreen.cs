using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : SingletonGameMenu<EndScreen>
{
    [System.Serializable]
    struct FailImageDesc
    {
        public GameMgr.GameFailCause cause;
        public Sprite image;
        public LocalizedString desc;
    }

    [SerializeField] FailImageDesc[] _failImageDescs;
    [SerializeField] StringTextPair _restartText;
    [SerializeField] StringTextPair _quitGameText;
    [SerializeField] StringTextPair _highestScoreText;
    [SerializeField] StringTextPair _scoreText;
    [SerializeField] LocalizedString _personalBestNoteStr;

    [SerializeField] Button _restartButton;
    [SerializeField] Button _quitGameButton;
    [SerializeField] Image _failImage;
    [SerializeField] Text _failText;

    public GameMgr.GameFailCause GameFailCause { get; set; } = GameMgr.GameFailCause.DEATH;

    protected override void Start()
    {
        base.Start();

        _restartButton.onClick.AddListener(RestartGame);
        _quitGameButton.onClick.AddListener(QuitGame);
    }

    public override void OnEnterMenu()
    {
        base.OnEnterMenu();

        foreach(var desc in _failImageDescs)
        {
            if (desc.cause == GameFailCause)
            {
                _failImage.sprite = desc.image;
                _failText.text = desc.desc;
                break;
            }
        }

        int highestScore = GameConsts.gameManager.HighestPlayerScore;
        int sessionScore = GameConsts.gameManager.PlayerScore;

        if(highestScore < sessionScore)
        {
            _highestScoreText.Set($": {sessionScore.ToString()}");
            _scoreText.Set($": {sessionScore.ToString()} ", _personalBestNoteStr);
        }
        else
        {
            _highestScoreText.Set($": {highestScore.ToString()}");
            _scoreText.Set($": {sessionScore.ToString()}");
        }
    }

    public override void OnLeaveMenu()
    {
        base.OnLeaveMenu();
        GameConsts.gameManager.ResumeGame();
    }
    public override bool CanClose()
    {
        return false;
    }

    void QuitGame()
    {
        GameConsts.gameManager.QuitGame();
    }

    void RestartGame()
    {
        GameConsts.gameManager.RestartGame();
    }
}
