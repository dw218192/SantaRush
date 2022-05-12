using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : SingletonGameMenu<EndScreen>
{
    [SerializeField] StringTextPair _highestScoreText;
    [SerializeField] StringTextPair _scoreText;
    [SerializeField] LocalizedString _personalBestNoteStr;

    [SerializeField] Button _restartButton;
    [SerializeField] Button _quitGameButton;

    protected override void Start()
    {
        base.Start();

        _restartButton.onClick.AddListener(RestartGame);
        _quitGameButton.onClick.AddListener(QuitGame);
    }

    public override void OnEnterMenu()
    {
        base.OnEnterMenu();

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

    void QuitGame()
    {
        GameConsts.gameManager.QuitGame();
    }

    void RestartGame()
    {
        GameConsts.gameManager.RestartGame();
    }
}
