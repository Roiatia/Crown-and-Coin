using System;
using UnityEngine;

public enum GameState
{
    Playing,
    DaySummary,
    Paused,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private KingdomManager kingdomManager;

    [Header("Game Settings")]
    [SerializeField] private int maxDays = 7;
    [SerializeField] private int currentDay = 1;
    [SerializeField] private float dayDuration = 30f;
    [SerializeField] private float raidChance = 0.35f;

    private float dayTimer;

    public int CurrentDay => currentDay;
    public int MaxDays => maxDays;
   
    public GameState CurrentState { get; private set; } = GameState.Playing;

    public event Action<GameState> OnStateChanged;
    public event Action<int, int> OnDayChanged;

    public event Action<string> OnDaySummaryReady;

    private void Start()
    {
        dayTimer = dayDuration;
        OnDayChanged?.Invoke(currentDay, maxDays);
    
        ChangeState(GameState.Playing);
    }

    private void Update()
    {
        if (CurrentState != GameState.Playing)
            return;

        dayTimer -= Time.deltaTime;
     

        if (dayTimer <= 0f)
        {
            EndDay();
        }
    }

    public void EndDay()
    {
        if (CurrentState != GameState.Playing)
            return;

        dayTimer = 0f;

        string summary = kingdomManager.ProcessDay();

        bool raidHappened = false;

        if (currentDay > 1 && UnityEngine.Random.value <= raidChance)
        {
            kingdomManager.ApplyRaid();
            raidHappened = true;
        }

        if (raidHappened)
            summary += "\n\nRaid happened during the night!";
        else
            summary += "\n\nNo raid tonight.";

        OnDaySummaryReady?.Invoke(summary);

        if (kingdomManager.IsGameLost())
        {
            ChangeState(GameState.Lose);
            return;
        }

        if (currentDay >= maxDays)
        {
            ChangeState(GameState.Win);
            return;
        }

        ChangeState(GameState.DaySummary);
    }

    public void StartNextDay()
    {
        if (CurrentState != GameState.DaySummary)
            return;

        currentDay++;
        dayTimer = dayDuration;

        OnDayChanged?.Invoke(currentDay, maxDays);

        ChangeState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            return;
        }

        if (CurrentState == GameState.Paused)
        {
            Time.timeScale = 1f;
            ChangeState(GameState.Playing);
        }
    }

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }
}