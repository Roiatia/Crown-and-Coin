using System;
using UnityEngine;
using UnityEngine.SceneManagement; 

public enum GameState
{
    MainMenu,
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

    [Header("Balance")]
    [SerializeField] private KingdomBalanceData balanceData;

    [Header("Runtime")]
    [SerializeField] private int currentDay = 1;

    private float dayTimer;
    private bool raidHappenedThisGame;
    public int CurrentDay => currentDay;
    public int MaxDays => balanceData.maxDays;
    public float DayTimer => dayTimer;
    public float DayDuration => balanceData.dayDuration;
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    public event Action<GameState> OnStateChanged;
    public event Action<int, int> OnDayChanged;
    public event Action<float, float> OnTimerChanged;
    public event Action<string> OnDaySummaryReady;

    private void Start()
    {
        dayTimer = balanceData.dayDuration;
        OnDayChanged?.Invoke(currentDay, balanceData.maxDays);
        OnTimerChanged?.Invoke(dayTimer, balanceData.dayDuration);
        Time.timeScale = 0f;
        ChangeState(GameState.MainMenu);
    }

    private void Update()
    {
        if (CurrentState != GameState.Playing)
            return;

        dayTimer -= Time.deltaTime;
        OnTimerChanged?.Invoke(dayTimer, balanceData.dayDuration);

        if (dayTimer <= 0f)
        {
            EndDay();
        }
    }


    public void StartGame()
    {
        if(CurrentState != GameState.MainMenu)
        {
            return;
        }

        Time.timeScale = 1f;
        ChangeState(GameState.Playing);
    }


    public void EndDay()
    {
        if (CurrentState != GameState.Playing)
            return;

        dayTimer = 0f;
        OnTimerChanged?.Invoke(dayTimer, balanceData.dayDuration);

        string summary = kingdomManager.ProcessDay();

        bool raidHappened = ShouldRaidHappenToday();

        if (raidHappened)
        {
            kingdomManager.ApplyRaid();
            raidHappenedThisGame = true;
            summary += "\n\nRaid happened during the night!";
        }
        else
        {
            summary += "\n\nNo raid tonight.";
        }

        OnDaySummaryReady?.Invoke(summary);

        if (kingdomManager.IsGameLost())
        {
            ChangeState(GameState.Lose);
            return;
        }

        if (currentDay >= balanceData.maxDays)
        {
            ChangeState(GameState.Win);
            return;
        }

        ChangeState(GameState.DaySummary);
    }


    private bool ShouldRaidHappenToday()
    {
        if (currentDay <= 1)
            return false;

        bool isLastDay = currentDay >= balanceData.maxDays;

        if (isLastDay && !raidHappenedThisGame)
            return true;

        return UnityEngine.Random.value <= balanceData.raidChance;
    }


    public void StartNextDay()
    {
        if (CurrentState != GameState.DaySummary)
            return;

        currentDay++;
        dayTimer = balanceData.dayDuration;

        OnDayChanged?.Invoke(currentDay, balanceData.maxDays);
        OnTimerChanged?.Invoke(dayTimer, balanceData.dayDuration);

        ChangeState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            Time.timeScale = 0f;
            ChangeState(GameState.Paused);
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

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 0f;
        ChangeState(GameState.MainMenu);
    }
}
