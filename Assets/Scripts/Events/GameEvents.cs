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

public class LevelStageEventData : GameEventData
{
    public float speedMultiplier;
    public LevelStageEventData(float speedMultiplier)
    {
        this.speedMultiplier = speedMultiplier;
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
    public bool hasBonus;
    public int multiplier;

    public BonusStateEventData(bool hasBonus, int multiplier)
    {
        this.hasBonus = hasBonus;
        this.multiplier = multiplier;
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
        new InterfaceMethodPair { type = typeof(IBonusStateHanlder), methodName = "OnBonusStateChange" }
    };
}