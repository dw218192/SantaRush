using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [SerializeField] InputAction _pauseGame = null;

    private void OnEnable()
    {
        _pauseGame.Enable();
        _pauseGame.performed += PauseGame;
    }

    private void OnDisable()
    {
        _pauseGame.Disable();
        _pauseGame.performed -= PauseGame;
    }

    void PauseGame(InputAction.CallbackContext context)
    {
        if(GameConsts.gameManager.State == GameMgr.GameState.PAUSED)
        {
            IGameMenu menu = GameConsts.uiMgr.GetActiveMenu();
            if(menu != null && (Object)menu != EndScreen.Instance)
                GameConsts.uiMgr.CloseCurrentMenu();
        }
        else
        {
            GameConsts.gameManager.PauseGame();
        }
    }
}
