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
    
    [SerializeField] private Button endDayButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitMenu;


    [SerializeField] private GameObject waterProductionButton;
    [SerializeField] private GameObject foodProductionButton;
    [SerializeField] private GameObject knightTrainingButton;


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

    [SerializeField] private TMP_Text gameOverStats;

    private void OnEnable()
    {
        resourceManager.OnResourceChanged += HandleResourceChanged;
        kingdomManager.OnKingdomChanged += RefreshHud;
        gameManager.OnDayChanged += HandleDayChanged;
        gameManager.OnDaySummaryReady += HandleDaySummaryReady;
        gameManager.OnStateChanged += HandleStateChanged;
        gameManager.OnTimerChanged += HandleTimerChanged;

        
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
        gameOverPanel.SetActive(state == GameState.Win || state == GameState.Lose);

        waterProductionButton.SetActive(state == GameState.Playing);
        foodProductionButton.SetActive(state == GameState.Playing);
        knightTrainingButton.SetActive(state == GameState.Playing);

        bool isPlaying = state == GameState.Playing;

      
        endDayButton.interactable = isPlaying;
        

        if (isPlaying)
        {
            daySummaryPanel.SetActive(false);
        }


      

        if(state == GameState.Win)
        {
            daySummaryPanel.SetActive(false);
            gameOverPanel.SetActive(true);

            gameOverText.text = "Huzzah! Victory is ours!";
            UpdateGameOverStats();
        }

        if(state == GameState.Lose)
        {
            daySummaryPanel.SetActive(false);
            gameOverPanel.SetActive(true);

            gameOverText.text = "Alas! All is lost!";
            UpdateGameOverStats();
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

    private void UpdateGameOverStats()
    {
        gameOverStats.text =
            $"Days survived: {gameManager.CurrentDay} / {gameManager.MaxDays}\n" +
            $"Food consumed: {kingdomManager.TotalFoodConsumed}\n" +
            $"Water consumed: {kingdomManager.TotalWaterConsumed}\n" +
            $"Gold collected: {kingdomManager.TotalGoldCollected}\n" +
            $"Knights trained: {kingdomManager.TotalKnightsTrained}\n" +
            $"New citizens: {kingdomManager.TotalNewCitizens}\n" +
            $"Raids: {kingdomManager.TotalRaids}\n\n" +
            $"Final citizens: {kingdomManager.Citizens}\n" +
            $"Final happiness: {kingdomManager.Happiness}";
    }
}