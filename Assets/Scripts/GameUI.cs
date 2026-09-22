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
    [SerializeField] private Button buildFarm;
    [SerializeField] private Button buildWell;
    [SerializeField] private Button trainKnights;
    [SerializeField] private Button endDay;

    private void OnEnable()
    {
        resourceManager.OnResourceChanged += HandleResourceChanged;
        kingdomManager.OnKingdomChanged += RefreshHud;
        gameManager.OnDayChanged += HandleDayChanged;

        buildFarm.onClick.AddListener(HandleBuildFarmClicked);
        buildWell.onClick.AddListener(HandleBuildWellClicked);
        trainKnights.onClick.AddListener(HandleTrainKnightClicked);
        endDay.onClick.AddListener(HandleEndDayClicked);
    }

    private void OnDisable()
    {
        resourceManager.OnResourceChanged -= HandleResourceChanged;
        kingdomManager.OnKingdomChanged -= RefreshHud;
        gameManager.OnDayChanged -= HandleDayChanged;

        buildFarm.onClick.RemoveListener(HandleBuildFarmClicked);
        buildWell.onClick.RemoveListener(HandleBuildWellClicked);
        trainKnights.onClick.RemoveListener(HandleTrainKnightClicked);
        endDay.onClick.RemoveListener(HandleEndDayClicked);
    }

    private void Start()
    {
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