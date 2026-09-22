using UnityEngine;

[CreateAssetMenu(fileName = "KingdomBalanceData", menuName = "Scriptable Objects/KingdomBalanceData")]
public class KingdomBalanceData : ScriptableObject
{
    [Header("Game")]
    public int maxDays = 7;
    public float dayDuration = 30f;
    public float raidChance = 0.35f;

    [Header("Daily Production")]
    public int foodPerFarm = 8;
    public int waterPerWell = 8;
    public int foodConsumedPerCitizen = 1;
    public int waterConsumedPerCitizen = 1;
    public int goldTaxPerCitizen = 2;

    [Header("Costs")]
    public int farmCostGold = 8;
    public int wellCostGold = 8;
    public int knightCostGold = 10;
    public int knightCostFood = 2;

    [Header("Raid")]
    public int raidFoodLoss = 5;
    public int raidGoldLoss = 5;
    public int raidHappinessLossWithKnights = 5;
    public int raidHappinessLossWithoutKnights = 20;
    public int raidCitizenLossWithoutKnights = 2;

    [Header("Happiness")]
    public int missingFoodPenalty = 15;
    public int missingWaterPenalty = 15;
    public int dailySuccessBonus = 5;
}
