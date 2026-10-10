using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class FarmTileController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap farmTilemap;
    [SerializeField] private ResourceManager resourceManager;


    [SerializeField] private TileBase emptyFarmTile;
    [SerializeField] private TileBase growingFarmTile;
    [SerializeField] private TileBase readyFarmTile;


    [SerializeField] private float growingTime = 5f;
    [SerializeField] private int foodReward = 5;

    private readonly Dictionary<Vector3Int, FarmTileData> farmTiles = new Dictionary<Vector3Int, FarmTileData>();

    private enum FarmStates
    {
        Empty , 
        Growing , 
        Ready
    }

    private class FarmTileData
    {
        public FarmStates state;
        public float timer;
    }

    private void Update()
    {
        UpdateGrowingTiles();

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0f)
        );

        mouseWorldPosition.z = 0f;

        Vector3Int cellPosition = farmTilemap.WorldToCell(mouseWorldPosition);
        TileBase clickedTile = farmTilemap.GetTile(cellPosition);

        if (clickedTile == null)
            return;

        if (clickedTile != emptyFarmTile &&
            clickedTile != growingFarmTile &&
            clickedTile != readyFarmTile)
            return;

        if (!farmTiles.ContainsKey(cellPosition))
        {
            farmTiles[cellPosition] = new FarmTileData
            {
                state = FarmStates.Empty,
                timer = 0f
            };
        }

        FarmTileData tileData = farmTiles[cellPosition];

        if (tileData.state == FarmStates.Empty)
        {
            StartGrowing(cellPosition, tileData);
            return;
        }
        
        if (tileData.state == FarmStates.Ready)
        {
            Harvest(cellPosition, tileData);
        }
    }

    private void StartGrowing(Vector3Int cellPosition, FarmTileData tileData)
    {
        tileData.state = FarmStates.Growing;
        tileData.timer = growingTime;
        farmTilemap.SetTile(cellPosition, growingFarmTile);
    }

    private void Harvest(Vector3Int cellPosition, FarmTileData tileData)
    {
        resourceManager.AddResource(ResourceType.Food, foodReward);

        tileData.state = FarmStates.Empty;
        tileData.timer = 0f;
        farmTilemap.SetTile(cellPosition, emptyFarmTile);
    }

    private void UpdateGrowingTiles()
    {
        foreach (KeyValuePair<Vector3Int, FarmTileData> farmTile in farmTiles)
        {
            FarmTileData tileData = farmTile.Value;

            if (tileData.state != FarmStates.Growing)
                continue;

            tileData.timer -= Time.deltaTime;

            if (tileData.timer <= 0f)
            {
                tileData.state = FarmStates.Ready;
                farmTilemap.SetTile(farmTile.Key, readyFarmTile);
            }
        }
    }

}
