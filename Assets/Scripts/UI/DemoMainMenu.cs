using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoMainMenu : UIObject
{
    [SerializeField] Button _startGameButton;
    [SerializeField] Button _quitGameButton;

    protected override void Start()
    {
        base.Start();
        _startGameButton.onClick.AddListener(()=> { SceneManager.LoadScene(GameConsts.k_MainSceneIndex); });
        _quitGameButton.onClick.AddListener(() => { Application.Quit();  });
    }
}