using System;
using UnityEngine;

public class KingdomManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private ResourceManager resourceManager;

    [Header("Balance")]
    [SerializeField] private KingdomBalanceData balanceData;

    [Header("Population")]
    [SerializeField] private int citizens = 10;
    [SerializeField] private int happiness = 70;
    [SerializeField] private int knights = 0;

    [Header("Buildings")]
    [SerializeField] private int farms = 1;
    [SerializeField] private int wells = 1;

    public int Citizens => citizens;
    public int Happiness => happiness;
    public int Knights => knights;
    public int Farms => farms;
    public int Wells => wells;

    public event Action OnKingdomChanged;

    public bool BuildFarm()
    {
        if (!resourceManager.Spend(ResourceType.Gold, balanceData.farmCostGold))
            return false;

        farms++;
        OnKingdomChanged?.Invoke();
        return true;
    }

    public bool BuildWell()
    {
        if (!resourceManager.Spend(ResourceType.Gold, balanceData.wellCostGold))
            return false;

        wells++;
        OnKingdomChanged?.Invoke();
        return true;
    }

    public bool TrainKnight()
    {
        if (!resourceManager.HasEnough(ResourceType.Gold, balanceData.knightCostGold))
            return false;

        if (!resourceManager.HasEnough(ResourceType.Food, balanceData.knightCostFood))
            return false;

        resourceManager.Spend(ResourceType.Gold, balanceData.knightCostGold);
        resourceManager.Spend(ResourceType.Food, balanceData.knightCostFood);

        knights++;

        OnKingdomChanged?.Invoke();
        return true;
    }

    public string ProcessDay()
    {
        int producedFood = farms * balanceData.foodPerFarm;
        int producedWater = wells * balanceData.waterPerWell;
        int consumedFood = citizens * balanceData.foodConsumedPerCitizen;
        int consumedWater = citizens * balanceData.waterConsumedPerCitizen;
        int earnedGold = citizens * balanceData.goldTaxPerCitizen;

        resourceManager.AddResource(ResourceType.Food, producedFood);
        resourceManager.AddResource(ResourceType.Water, producedWater);
        resourceManager.AddResource(ResourceType.Gold, earnedGold);

        bool hadEnoughFood = resourceManager.Spend(ResourceType.Food, consumedFood);
        bool hadEnoughWater = resourceManager.Spend(ResourceType.Water, consumedWater);

        if (!hadEnoughFood)
        {
            happiness -= balanceData.missingFoodPenalty;
            citizens--;
        }

        if (!hadEnoughWater)
        {
            happiness -= balanceData.missingWaterPenalty;
            citizens--;
        }

        if (hadEnoughFood && hadEnoughWater)
        {
            happiness += balanceData.dailySuccessBonus;
        }

        happiness = Mathf.Clamp(happiness, 0, 100);
        citizens = Mathf.Max(citizens, 0);

        OnKingdomChanged?.Invoke();

        return $"Food: +{producedFood} -{consumedFood}\n" +
               $"Water: +{producedWater} -{consumedWater}\n" +
               $"Gold Tax: +{earnedGold}\n" +
               $"Citizens: {citizens}\n" +
               $"Happiness: {happiness}";
    }

    public void ApplyRaid()
    {
        resourceManager.Spend(ResourceType.Food, balanceData.raidFoodLoss);
        resourceManager.Spend(ResourceType.Gold, balanceData.raidGoldLoss);

        if (knights > 0)
        {
            knights--;
            happiness -= balanceData.raidHappinessLossWithKnights;
        }
        else
        {
            citizens -= balanceData.raidCitizenLossWithoutKnights;
            happiness -= balanceData.raidHappinessLossWithoutKnights;
        }

        happiness = Mathf.Clamp(happiness, 0, 100);
        citizens = Mathf.Max(citizens, 0);

        OnKingdomChanged?.Invoke();
    }

    public bool IsGameLost()
    {
        return citizens <= 0 || happiness <= 0;
    }
}