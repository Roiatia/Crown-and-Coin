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
    
    public int defenceThreshold = 3; //minimum knight for defence
    public int citizensLossWithoutKnights = 4;
    public int citizensLossWithKnights = 2;


    public int raidHappinessLossWithKnights = 5;
    public int raidHappinessLossWithoutKnights = 20;
    public int raidCitizenLossWithoutKnights = 2;

    public float raidWarningTime = 0.5f;


    [Header("Happiness")]
    public int missingFoodPenalty = 15;
    public int missingWaterPenalty = 15;
    public int dailySuccessBonus = 5;

     public int knightsPerClick = 3;

    public int dailyNewCitizens = 1;
    public int highHappinessBonusCitizens = 1;
    public int highHappinessThreshold = 80;
    public int minimumHappinessThreshold = 40;
}
