using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FarmAreaProductionController : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private Tilemap farmTilemap;
    [SerializeField] private Button productionButton;
    [SerializeField] private TMP_Text buttonText;

    //Tiles
    [SerializeField] private TileBase emptyFarmTile;
    [SerializeField] private TileBase growingFarmTile;
    [SerializeField] private TileBase readyFarmTile;


//production vars
    [SerializeField] private int foodReward = 20;
    [SerializeField] private float growthDuration = 8f;
    [SerializeField] private float readyDisplayDuration = 1.5f;

    private readonly List<Vector3Int> activeFarmCells = new List<Vector3Int>();

    private float timer;
    private bool isGrowing;
    private bool isReadyDisplay;

    private void Awake()
    {
        if (productionButton == null)
            productionButton = GetComponent<Button>();

        UpdateVisuals();
    }

    private void Update()
    {
        if (!isGrowing && !isReadyDisplay)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f && isGrowing)
        {
            FinishGrowing();
            return;
        }

        if (timer <= 0f && isReadyDisplay)
        {
            ResetFarmTiles();
            return;
        }

        UpdateVisuals();
    }

    public void StartFoodProduction()
    {
        if (isGrowing || isReadyDisplay)
            return;

        activeFarmCells.Clear();

        BoundsInt bounds = farmTilemap.cellBounds;

        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            TileBase tile = farmTilemap.GetTile(cellPosition);

            if (tile != emptyFarmTile)
                continue;

            activeFarmCells.Add(cellPosition);
            farmTilemap.SetTile(cellPosition, growingFarmTile);
        }

        if (activeFarmCells.Count == 0)
            return;

        isGrowing = true;
        timer = growthDuration;

        UpdateVisuals();
    }

    private void FinishGrowing()
    {
        isGrowing = false;
        isReadyDisplay = true;
        timer = readyDisplayDuration;

        foreach (Vector3Int cellPosition in activeFarmCells)
        {
            farmTilemap.SetTile(cellPosition, readyFarmTile);
        }

        resourceManager.AddResource(ResourceType.Food, foodReward);

        UpdateVisuals();
    }

    private void ResetFarmTiles()
    {
        isReadyDisplay = false;

        foreach (Vector3Int cellPosition in activeFarmCells)
        {
            farmTilemap.SetTile(cellPosition, emptyFarmTile);
        }

        activeFarmCells.Clear();

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (productionButton != null)
            productionButton.interactable = !isGrowing && !isReadyDisplay;

        if (buttonText == null)
            return;

        if (isGrowing)
        {
            buttonText.text = Mathf.CeilToInt(timer).ToString();
            return;
        }

        if (isReadyDisplay)
        {
            buttonText.text = "Ready";
            return;
        }

        buttonText.text = "+Food";
    }
}