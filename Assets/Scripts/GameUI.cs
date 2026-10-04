using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private KingdomManager kingdomManager;
    [SerializeField] private GameManager gameManager;

    [Header("HUD Texts")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text waterText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text citizensText;
    [SerializeField] private TMP_Text happinessText;
    [SerializeField] private TMP_Text knightsText;
    [SerializeField] private TMP_Text Timer;

    [Header("Buttons")]
    [SerializeField] private Button buildFarmButton;
    [SerializeField] private Button buildWellButton;
    [SerializeField] private Button trainKnightButton;
    [SerializeField] private Button endDayButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitMenu;

    

    [Header("Day Summary")]
    [SerializeField] private GameObject daySummaryPanel;
    [SerializeField] private TMP_Text daySummaryText;

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;


    [Header("Pause Menu")]
    [SerializeField] private GameObject pausePanel;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;

    private void OnEnable()
    {
        resourceManager.OnResourceChanged += HandleResourceChanged;
        kingdomManager.OnKingdomChanged += RefreshHud;
        gameManager.OnDayChanged += HandleDayChanged;
        gameManager.OnDaySummaryReady += HandleDaySummaryReady;
        gameManager.OnStateChanged += HandleStateChanged;
        gameManager.OnTimerChanged += HandleTimerChanged;

        buildFarmButton.onClick.AddListener(HandleBuildFarmClicked);
        buildWellButton.onClick.AddListener(HandleBuildWellClicked);
        trainKnightButton.onClick.AddListener(HandleTrainKnightClicked);
        endDayButton.onClick.AddListener(HandleEndDayClicked);

        pauseButton.onClick.AddListener(HandlePauseClicked);
        resumeButton.onClick.AddListener(HandlePauseClicked);
        restartButton.onClick.AddListener(HandleRestartClicked);
        exitMenu.onClick.AddListener(HandleExitClicked);

        startButton.onClick.AddListener(HandleStartClicked);
    }

    private void OnDisable()
    {
        resourceManager.OnResourceChanged -= HandleResourceChanged;
        kingdomManager.OnKingdomChanged -= RefreshHud;
        gameManager.OnDayChanged -= HandleDayChanged;
        gameManager.OnDaySummaryReady -= HandleDaySummaryReady;
        gameManager.OnStateChanged -= HandleStateChanged;
        gameManager.OnTimerChanged -= HandleTimerChanged;

        buildFarmButton.onClick.RemoveListener(HandleBuildFarmClicked);
        buildWellButton.onClick.RemoveListener(HandleBuildWellClicked);
        trainKnightButton.onClick.RemoveListener(HandleTrainKnightClicked);
        endDayButton.onClick.RemoveListener(HandleEndDayClicked);


        pauseButton.onClick.RemoveListener(HandlePauseClicked);
        resumeButton.onClick.RemoveListener(HandlePauseClicked);
        restartButton.onClick.RemoveListener(HandleRestartClicked);
        exitMenu.onClick.RemoveListener(HandleExitClicked);

        startButton.onClick.RemoveListener(HandleStartClicked);

    }

    private void Start()
    {
        daySummaryPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(gameManager.CurrentState == GameState.MainMenu);
        pausePanel.SetActive(false);


        RefreshHud();
        HandleDayChanged(gameManager.CurrentDay, gameManager.MaxDays);
    }

    public void HandleStartClicked()
    {
        gameManager.StartGame();
    }

    private void HandlePauseClicked()
    {
        gameManager.TogglePause();
    }

   private void HandleExitClicked()
    {
        gameManager.ReturnToMenu();
    }

    private void HandleRestartClicked()
    {
        gameManager.RestartGame();
    }

    private void HandleBuildFarmClicked()
    {
        kingdomManager.BuildFarm();
    }

    private void HandleBuildWellClicked()
    {
        kingdomManager.BuildWell();
    }

    private void HandleTrainKnightClicked()
    {
        kingdomManager.TrainKnight();
    }

    private void HandleEndDayClicked()
    {
        gameManager.EndDay();
    }

    private void HandleResourceChanged(ResourceType type, int amount)
    {
        RefreshHud();
    }

    private void HandleDayChanged(int currentDay, int maxDays)
    {
        dayText.text = $"{currentDay}/{maxDays}";
    }

    private void HandleDaySummaryReady(string summary)
    {
        daySummaryText.text = summary;
        daySummaryPanel.SetActive(true);
    }

    private void HandleStateChanged(GameState state)
    {
        mainMenuPanel.SetActive(state == GameState.MainMenu);
        pausePanel.SetActive(state == GameState.Paused);

        bool isPlaying = state == GameState.Playing;

        buildFarmButton.interactable = isPlaying;
        buildWellButton.interactable = isPlaying;
        trainKnightButton.interactable = isPlaying;
        endDayButton.interactable = isPlaying;
        

        if (isPlaying)
        {
            daySummaryPanel.SetActive(false);
        }


        if (state == GameState.Win)
        {
            daySummaryPanel.SetActive(false);
            gameOverPanel.SetActive(true);
            gameOverText.text = "Victory!\nThe kingdom survived.";
        }

        if (state == GameState.Lose)
        {
            daySummaryPanel.SetActive(false);
            gameOverPanel.SetActive(true);
            gameOverText.text = "Game Over\nThe kingdom has fallen.";
        }
    }

    private void HandleTimerChanged(float timeLeft , float dayDurtation)
    {
        int seconds = Mathf.CeilToInt(timeLeft);
        Timer.text = seconds.ToString();

    }

    private void RefreshHud()
    {
        foodText.text = resourceManager.GetAmount(ResourceType.Food).ToString();
        waterText.text = resourceManager.GetAmount(ResourceType.Water).ToString();
        goldText.text = resourceManager.GetAmount(ResourceType.Gold).ToString();

        citizensText.text = kingdomManager.Citizens.ToString();
        happinessText.text = kingdomManager.Happiness.ToString();
        knightsText.text = kingdomManager.Knights.ToString();
    }
}