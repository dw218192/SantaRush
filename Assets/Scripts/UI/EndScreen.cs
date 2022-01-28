using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : SingletonGameMenu<EndScreen>
{
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
        _scoreText.text = "分数: " + GameConsts.gameManager.Score.ToString();
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
