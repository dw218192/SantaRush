using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : SingletonGameMenu<EndScreen>
{
    [SerializeField] Text _highestScoreText;
    [SerializeField] Text _scoreText;


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
            _highestScoreText.text = $"个人最佳: {sessionScore}";
            _scoreText.text = $"分数: {sessionScore} (新最高分!!)";
        }
        else
        {
            _highestScoreText.text = $"个人最佳: {highestScore}";
            _scoreText.text = $"分数: {sessionScore}";
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
