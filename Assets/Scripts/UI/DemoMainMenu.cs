using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoMainMenu : UIObject
{
    [SerializeField] StringTextPair _startGameText;
    [SerializeField] StringTextPair _quitGameText;
    [SerializeField] StringTextPair _gameTitleText;
    [SerializeField] StringTextPair _languageOptionText;
    [SerializeField] StringTextPair _tutorialText;

    [SerializeField] Button _startGameButton;
    [SerializeField] Button _quitGameButton;
    [SerializeField] Button _tutorialButton;

    [SerializeField] Dropdown _languageDropdown;

    protected override void Start()
    {
        base.Start();

        Language[] languages = (Language[])System.Enum.GetValues(typeof(Language));

        _languageDropdown.options.Clear();
        foreach (Language lan in languages)
        {
            _languageDropdown.options.Add(new Dropdown.OptionData(lan.ToString()));
        }

        _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        _languageDropdown.value = (int) GameConsts.curLanguage;
        _languageDropdown.RefreshShownValue();

        _startGameButton.onClick.AddListener(StartGame);
        _quitGameButton.onClick.AddListener(() => { Application.Quit();  });
        _tutorialButton.onClick.AddListener(() => { GameConsts.uiMgr.OpenMenu(TutorialMenu.Instance); });
    }

    void StartGame()
    {
        int tutorialViewed = PlayerPrefs.GetInt(GameConsts.k_PlayerPrefTutorialViewed, 0);
        if (tutorialViewed == 0)
        {
            GameConsts.uiMgr.OpenMenu(TutorialMenu.Instance);
            TutorialMenu.Instance.TutorialFInishEvent += () => { SceneManager.LoadScene(GameConsts.k_MainSceneIndex); };
            return;
        }
        
        SceneManager.LoadScene(GameConsts.k_MainSceneIndex);
    }

    void OnLanguageChanged(int val)
    {
        GameConsts.curLanguage = (Language)val;
    }
}