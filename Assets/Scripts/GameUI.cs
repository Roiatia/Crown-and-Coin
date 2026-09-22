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

    [Header("Buttons")]
    [SerializeField] private Button buildFarmButton;
    [SerializeField] private Button buildWellButton;
    [SerializeField] private Button trainKnightButton;
    [SerializeField] private Button endDayButton;

    [Header("Day Summary")]
    [SerializeField] private GameObject daySummaryPanel;
    [SerializeField] private TMP_Text daySummaryText;

    private void OnEnable()
    {
        resourceManager.OnResourceChanged += HandleResourceChanged;
        kingdomManager.OnKingdomChanged += RefreshHud;
        gameManager.OnDayChanged += HandleDayChanged;
        gameManager.OnDaySummaryReady += HandleDaySummaryReady;
        gameManager.OnStateChanged += HandleStateChanged;

        buildFarmButton.onClick.AddListener(HandleBuildFarmClicked);
        buildWellButton.onClick.AddListener(HandleBuildWellClicked);
        trainKnightButton.onClick.AddListener(HandleTrainKnightClicked);
        endDayButton.onClick.AddListener(HandleEndDayClicked);
    }

    private void OnDisable()
    {
        resourceManager.OnResourceChanged -= HandleResourceChanged;
        kingdomManager.OnKingdomChanged -= RefreshHud;
        gameManager.OnDayChanged -= HandleDayChanged;
        gameManager.OnDaySummaryReady -= HandleDaySummaryReady;
        gameManager.OnStateChanged -= HandleStateChanged;

        buildFarmButton.onClick.RemoveListener(HandleBuildFarmClicked);
        buildWellButton.onClick.RemoveListener(HandleBuildWellClicked);
        trainKnightButton.onClick.RemoveListener(HandleTrainKnightClicked);
        endDayButton.onClick.RemoveListener(HandleEndDayClicked);
    }

    private void Start()
    {
        daySummaryPanel.SetActive(false);
        RefreshHud();
        HandleDayChanged(gameManager.CurrentDay, gameManager.MaxDays);
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
        dayText.text = $"Day: {currentDay}/{maxDays}";
    }

    private void HandleDaySummaryReady(string summary)
    {
        daySummaryText.text = summary;
        daySummaryPanel.SetActive(true);
    }

    private void HandleStateChanged(GameState state)
    {
        bool isPlaying = state == GameState.Playing;

        buildFarmButton.interactable = isPlaying;
        buildWellButton.interactable = isPlaying;
        trainKnightButton.interactable = isPlaying;
        endDayButton.interactable = isPlaying;

        if (isPlaying)
        {
            daySummaryPanel.SetActive(false);
        }
    }

    private void RefreshHud()
    {
        foodText.text = $"Food: {resourceManager.GetAmount(ResourceType.Food)}";
        waterText.text = $"Water: {resourceManager.GetAmount(ResourceType.Water)}";
        goldText.text = $"Gold: {resourceManager.GetAmount(ResourceType.Gold)}";

        citizensText.text = $"Citizens: {kingdomManager.Citizens}";
        happinessText.text = $"Happiness: {kingdomManager.Happiness}";
        knightsText.text = $"Knights: {kingdomManager.Knights}";
    }
}