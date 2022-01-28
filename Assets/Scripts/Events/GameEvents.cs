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
    public enum Type { TOTAL_SCORE, GIFT_TARGET_SCORE };

    public Type type;
    public int score;
    public int targetScore;

    public GameScoreEventData(Type type, int score, int targetScore)
    {
        this.type = type;
        this.score = score;
        this.targetScore = targetScore;
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
        new InterfaceMethodPair { type = typeof(IGameScoreHandler), methodName = "OnGameScoreChange" }
    };
}