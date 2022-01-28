using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoMainMenu : MonoBehaviour
{
    [SerializeField] Button _startGameButton;
    [SerializeField] Button _quitGameButton;

    private void Start()
    {
        _startGameButton.onClick.AddListener(()=> { SceneManager.LoadScene(GameConsts.k_MainSceneIndex); });
        _quitGameButton.onClick.AddListener(() => { Application.Quit();  });
    }
}