using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public abstract class GameEventData
{

}

public interface ITestEvent
{
    void OnEvent(GameEventData data);
}

public interface ILevelStageHandler
{
    void OnGameStageEnter(LevelStageEventData eventData);
}

public interface IGameScoreHandler
{
    void OnGameScoreChange(GameScoreEventData eventData);
}

public interface IWagonCollisionHandler
{
    void OnWagonCollide(WagonCollisionEventData eventData);
}

public interface IBonusStateHanlder
{
    void OnBonusStateChange(BonusStateEventData eventData);
}

public interface IBuffStateHandler
{
    void OnBuffStateChange(BuffStateEventData eventData);
}

public class LevelStageEventData : GameEventData
{
    public float speedMultiplier;
    public float extraSpeedIncrease;

    public LevelStageEventData(float speedMultiplier, float extraSpeedIncrease)
    {
        this.speedMultiplier = speedMultiplier;
        this.extraSpeedIncrease = extraSpeedIncrease;
    }
}

public class GameScoreEventData : GameEventData
{
    public enum Type 
    {
        TOTAL_SCORE_CHANGE,
        GIFT_TARGET_SCORE_CHANGE,
        TOTAL_PLAYER_SCORE_CHANGE
    };

    public Type type;
    public int[] values;

    public GameScoreEventData(Type type, params int[] args)
    {
        this.type = type;
        values = args;
    }
}
public class WagonCollisionEventData : GameEventData
{
    public int partCount;
    public WagonCollisionEventData(int partCount)
    {
        this.partCount = partCount;
    }
}
public class BonusStateEventData : GameEventData
{
    public string bonusName;
    public bool hasBonus;
    public int multiplier;

    public BonusStateEventData(string bonusName, bool hasBonus, int multiplier)
    {
        this.bonusName = bonusName;
        this.hasBonus = hasBonus;
        this.multiplier = multiplier;
    }
}
public class BuffStateEventData : GameEventData
{
    public bool buffEnabled;
    public PlayerBuffDesc desc;

    public BuffStateEventData(bool buffEnabled, PlayerBuffDesc desc)
    {
        this.buffEnabled = buffEnabled;
        this.desc = desc;
    }
}

public struct InterfaceMethodPair
{
    public Type type;
    public string methodName;
}

public static class EventStub
{
    public static InterfaceMethodPair[] EventTypes =
    {
        new InterfaceMethodPair { type = typeof(ITestEvent), methodName = "OnEvent" },
        new InterfaceMethodPair { type = typeof(ILevelStageHandler), methodName = "OnGameStageEnter" },
        new InterfaceMethodPair { type = typeof(IGameScoreHandler), methodName = "OnGameScoreChange" },
        new InterfaceMethodPair { type = typeof(IWagonCollisionHandler), methodName = "OnWagonCollide" },
        new InterfaceMethodPair { type = typeof(IBonusStateHanlder), methodName = "OnBonusStateChange" },
        new InterfaceMethodPair { type = typeof(IBuffStateHandler), methodName = "OnBuffStateChange" },
    };
}