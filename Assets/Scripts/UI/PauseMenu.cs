using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : SingletonGameMenu<PauseMenu>
{
    [SerializeField]
    Button _resumeButton;
    [SerializeField]
    Button _restartButton;
    [SerializeField]
    Button _tutorialButton;
    [SerializeField]
    Button _quitGameButton;

    [SerializeField]
    StringTextPair _scoreText;
    [SerializeField]
    StringTextPair _resumeText;
    [SerializeField]
    StringTextPair _restartText;
    [SerializeField]
    StringTextPair _tutorialText;
    [SerializeField]
    StringTextPair _quitGameText;
    
    protected override void Start()
    {
        base.Start();

        _resumeButton.onClick.AddListener(OnBackPressed);
        _restartButton.onClick.AddListener(GameConsts.gameManager.RestartGame);
        _tutorialButton.onClick.AddListener(() => { GameConsts.uiMgr.OpenMenu(TutorialMenu.Instance); });
        _quitGameButton.onClick.AddListener(GameConsts.gameManager.QuitGame);
    }

    public override void OnEnterMenu()
    {
        base.OnEnterMenu();
        _scoreText.Set(": ", GameConsts.gameManager.PlayerScore.ToString());
    }

    public override void OnLeaveMenu()
    {
        base.OnLeaveMenu();
        GameConsts.gameManager.ResumeGame();
    }
}
