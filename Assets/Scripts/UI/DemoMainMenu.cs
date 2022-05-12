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

    [SerializeField] Button _startGameButton;
    [SerializeField] Button _quitGameButton;
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
        _languageDropdown.value = 0;
        _languageDropdown.RefreshShownValue();
        OnLanguageChanged(0);

        _startGameButton.onClick.AddListener(()=> { SceneManager.LoadScene(GameConsts.k_MainSceneIndex); });
        _quitGameButton.onClick.AddListener(() => { Application.Quit();  });
    }

    void OnLanguageChanged(int val)
    {
        GameConsts.curLanguage = (Language)val;
    }
}