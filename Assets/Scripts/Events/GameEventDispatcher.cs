using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Game Event Dispatcher")]
public class GameEventDispatcher : ScriptableObject
{
    public void AddScoreGift(Hitbox hitbox)
    {
        var giftIns = hitbox.GetComponent<GiftInstance>();
        if (giftIns != null)
            GameConsts.gameManager.AddScore(giftIns.GiftType.val);
    }

    public void AddScore(int delta)
    {
        if (!GameConsts.gameManager)
            Debug.LogWarning("cannot find game manager", this);
        GameConsts.gameManager.AddScore(delta);
    }

    public void Fail(GameMgr.GameFailCause cause)
    {
        if (!GameConsts.gameManager)
            Debug.LogWarning("cannot find game manager", this);
        GameConsts.gameManager.FailGame(cause);
    }
}
