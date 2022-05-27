using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] StringTextPair _mainMenuText;
    [SerializeField] StringTextPair _highestScoreText;
    [SerializeField] StringTextPair _scoreText;
    [SerializeField] LocalizedString _personalBestNoteStr;

    [SerializeField] Button _restartButton;
    [SerializeField] Button _mainMenuButton;
    [SerializeField] Image _failImage;
    [SerializeField] Text _failText;

    public GameMgr.GameFailCause GameFailCause { get; set; } = GameMgr.GameFailCause.DEATH;

    protected override void Start()
    {
        base.Start();

        
        _restartButton.onClick.AddListener(RestartGame);
        _mainMenuButton.onClick.AddListener(ToMainMenu);
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

    public override bool CanClose()
    {
        return false;
    }

    void ToMainMenu()
    {
        GameConsts.gameManager.ResumeGame();
        SceneManager.LoadScene(GameConsts.k_MainMenuSceneIndex);
    }

    void RestartGame()
    {
        GameConsts.gameManager.RestartGame();
    }
}
