using System;
using System.Collections.Specialized;
using Unity.Mathematics;
using Unity.VisualScripting;
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

    [Header("End Game Stats")]
    [SerializeField] int totalFoodConsumed;
    [SerializeField] int totalWaterConsumed;
    [SerializeField] int totalGoldCollected;
    [SerializeField] int totalKnightsTrained;
    [SerializeField] int totalNewCitizens;
    [SerializeField] int totalRaids;

    public int TotalFoodConsumed => totalFoodConsumed;
    public int TotalWaterConsumed => totalWaterConsumed;
    public int TotalGoldCollected => TotalGoldCollected;
    public int TotalKnightsTrained => TotalKnightsTrained;
    public int TotalNewCitizens => totalNewCitizens;
    public int TotalRaids => totalRaids;



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
        totalKnightsTrained++;

        OnKingdomChanged?.Invoke();
        return true;
    }

    public string ProcessDay()
    {
        int producedFood = 0;
        int producedWater = 0;
        int consumedFood = citizens * balanceData.foodConsumedPerCitizen;
        int consumedWater = citizens * balanceData.waterConsumedPerCitizen;
        int earnedGold = citizens * balanceData.goldTaxPerCitizen;

        resourceManager.AddResource(ResourceType.Food, producedFood);
        resourceManager.AddResource(ResourceType.Water, producedWater);
        resourceManager.AddResource(ResourceType.Gold, earnedGold);

        bool hadEnoughFood = resourceManager.Spend(ResourceType.Food, consumedFood);
        bool hadEnoughWater = resourceManager.Spend(ResourceType.Water, consumedWater);

        if (hadEnoughFood) totalFoodConsumed += consumedFood;
        if (hadEnoughWater) totalWaterConsumed += consumedWater;

        totalGoldCollected += earnedGold;

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

        int newCitizens = 0;

        if (happiness >= balanceData.minimumHappinessThreshold)
        {
            newCitizens = balanceData.dailyNewCitizens;

            if (happiness >= balanceData.highHappinessThreshold)
            {
                newCitizens += balanceData.highHappinessBonusCitizens;
            }

            citizens += newCitizens;
            totalNewCitizens += newCitizens;

            if (PlayerFeedbackUI.Instance != null)
            {
                PlayerFeedbackUI.Instance.ShowMessage("Huzzah! New citizens have arrived to join our glorious kingdom!");
            }

        }



        OnKingdomChanged?.Invoke();

        return $"Daily Summary\n\n" +
                $"Food consumed: -{consumedFood}\n" +
                $"Water consumed: -{consumedWater}\n" +
                $"Gold collected: +{earnedGold}\n" +
                $"New citizens: +{newCitizens}\n\n" +
                $"Citizens: {citizens}\n" +
                $"Happiness: {happiness}";
    }

    public String ApplyRaid()
    {
        totalRaids++; 

        resourceManager.Spend(ResourceType.Food, balanceData.raidFoodLoss);
        resourceManager.Spend(ResourceType.Gold, balanceData.raidGoldLoss);

        string raidResult;

        if (knights <= 0)
        {
            citizens -= balanceData.citizensLossWithoutKnights;
            happiness -= balanceData.raidHappinessLossWithoutKnights;

            raidResult = "The heathens struck under the cover of night !\n" +
                          "Alas ! not a single knight stood to defend the kingdom !\n" +
                          $"citizens: - {balanceData.citizensLossWithoutKnights}\n" +
                          $"Happiness :  - {balanceData.raidHappinessLossWithoutKnights}";
        }
        else if (knights < balanceData.defenceThreshold)
        {
            knights--;
            citizens -= balanceData.citizensLossWithKnights;
            happiness -= balanceData.raidHappinessLossWithKnights;

            raidResult = "The heathens struck under the cover of night!\n" +
                            "Alas! Our defenses were too weak to withstand their assault.\n" +
                            "Knights Lost: -1\n" +
                            $"Citizens Lost: -{balanceData.citizensLossWithKnights}\n" +
                            $"Happiness: -{balanceData.raidHappinessLossWithKnights}";
        }
        else
        {
            knights--;
            happiness -= balanceData.raidHappinessLossWithKnights;

            raidResult = "The heathens struck under the cover of night!\n" +
                            "Victory! Our brave knights stood strong and defended the kingdom!\n" +
                             "Knights Lost: -1\n" +
                               $"Happiness: -{balanceData.raidHappinessLossWithKnights}";
        }

        happiness = Mathf.Clamp(happiness, 0, 100);
        citizens = Mathf.Max(citizens, 0);

        OnKingdomChanged?.Invoke();

        return raidResult;



        //if (knights > 0)
        //{
        //    knights--;
        //    happiness -= balanceData.raidHappinessLossWithKnights;
        //}
        //else
        //{
        //    citizens -= balanceData.raidCitizenLossWithoutKnights;
        //    happiness -= balanceData.raidHappinessLossWithoutKnights;
        //}

        //happiness = Mathf.Clamp(happiness, 0, 100);
        //citizens = Mathf.Max(citizens, 0);

        //OnKingdomChanged?.Invoke();
    }

    public bool IsGameLost()
    {
        return citizens <= 0 || happiness <= 0;
    }
}