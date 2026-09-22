using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private List<ResourceAmount> startingResources = new List<ResourceAmount>();

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    public event Action<ResourceType, int> OnResourceChanged;

    private void Awake()
    {
        InitializeResources();
    }

    private void InitializeResources()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
        }

        foreach (ResourceAmount resource in startingResources)
        {
            resources[resource.type] = resource.amount;
        }
    }

    public int GetAmount(ResourceType type)
    {
        return resources[type];
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (amount <= 0)
            return;

        resources[type] += amount;
        OnResourceChanged?.Invoke(type, resources[type]);
    }

    public bool HasEnough(ResourceType type, int amount)
    {
        if (amount < 0)
            return false;

        return resources[type] >= amount;
    }

    public bool Spend(ResourceType type, int amount)
    {
        if (!HasEnough(type, amount))
            return false;

        resources[type] -= amount;
        OnResourceChanged?.Invoke(type, resources[type]);

        return true;
    }
}

[Serializable]
public class ResourceAmount
{
    public ResourceType type;
    public int amount;
}