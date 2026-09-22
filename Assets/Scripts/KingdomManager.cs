using System;
using UnityEngine;

public class KingdomManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private ResourceManager resourceManager;

    [Header("Population")]
    [SerializeField] private int citizens = 10;
    [SerializeField] private int happiness = 70;
    [SerializeField] private int knights = 0;

    [Header("Buildings")]
    [SerializeField] private int farms = 1;
    [SerializeField] private int wells = 1;

    [Header("Daily Balance")]
    [SerializeField] private int foodPerFarm = 8;
    [SerializeField] private int waterPerWell = 8;
    [SerializeField] private int foodConsumedPerCitizen = 1;
    [SerializeField] private int waterConsumedPerCitizen = 1;
    [SerializeField] private int goldTaxPerCitizen = 2;

    [Header("Costs")]
    [SerializeField] private int farmCostGold = 8;
    [SerializeField] private int wellCostGold = 8;
    [SerializeField] private int knightCostGold = 10;
    [SerializeField] private int knightCostFood = 2;

    public int Citizens => citizens;
    public int Happiness => happiness;
    public int Knights => knights;
    public int Farms => farms;
    public int Wells => wells;

    public event Action OnKingdomChanged;

    public bool BuildFarm()
    {
        if (!resourceManager.Spend(ResourceType.Gold, farmCostGold))
            return false;

        farms++;
        OnKingdomChanged?.Invoke();
        return true;
    }

    public bool BuildWell()
    {
        if (!resourceManager.Spend(ResourceType.Gold, wellCostGold))
            return false;

        wells++;
        OnKingdomChanged?.Invoke();
        return true;
    }

    public bool TrainKnight()
    {
        if (!resourceManager.HasEnough(ResourceType.Gold, knightCostGold))
            return false;

        if (!resourceManager.HasEnough(ResourceType.Food, knightCostFood))
            return false;

        resourceManager.Spend(ResourceType.Gold, knightCostGold);
        resourceManager.Spend(ResourceType.Food, knightCostFood);

        knights++;
        

        OnKingdomChanged?.Invoke();
        return true;
    }

    public string ProcessDay()
    {
        int producedFood = farms * foodPerFarm;
        int producedWater = wells * waterPerWell;
        int consumedFood = citizens * foodConsumedPerCitizen;
        int consumedWater = citizens * waterConsumedPerCitizen;
        int earnedGold = citizens * goldTaxPerCitizen;

        resourceManager.AddResource(ResourceType.Food, producedFood);
        resourceManager.AddResource(ResourceType.Water, producedWater);
        resourceManager.AddResource(ResourceType.Gold, earnedGold);

        bool hadEnoughFood = resourceManager.Spend(ResourceType.Food, consumedFood);
        bool hadEnoughWater = resourceManager.Spend(ResourceType.Water, consumedWater);

        if (!hadEnoughFood)
        {
            happiness -= 15;
            citizens -= 1;
        }

        if (!hadEnoughWater)
        {
            happiness -= 15;
            citizens -= 1;
        }

        if (hadEnoughFood && hadEnoughWater)
        {
            happiness += 5;
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
        int foodLoss = 5;
        int goldLoss = 5;

        resourceManager.Spend(ResourceType.Food, foodLoss);
        resourceManager.Spend(ResourceType.Gold, goldLoss);

        if (knights > 0)
        {
            knights--;
            happiness -= 5;
        }
        else
        {
            citizens -= 2;
            happiness -= 20;
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