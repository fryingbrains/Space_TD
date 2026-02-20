using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class MapGenerator : MonoBehaviour
{
    public GameManager gameManager;
    public List<Vector3Int> bufferList = new();
    public Tilemap tilemap;
    public TileBase pathTile;
    public TileBase buildTile;
    public TileBase blockedTile;
    private Vector3Int? selectedCell = null;
    private Dictionary<Vector3Int, bool> tileOccupied = new();
    public Button buildButton;
    public GameObject towerPrefab;
    private TowerUpgradeUI towerUpgradeUI;
    private string[] map =
    {
        "BBBBBBPBBB",
        "BBBBPPPBBB",
        "BBBBPBBBBB",
        "BBBBPPPBBB",
        "BBBBBBPBBB",
        "BBBBBBPBBB",
        "BBBBBBPBBB",
        "BPPPPPPBBB",
        "BPBBBBBPPP",
        "BPPPPBPPBP",
        "BBBBPPPBBP",
        "BBBBBBBBBP",
        "BBPPPPPPPP",
        "BPPBBBBBBB",
        "BPBPPPPPBB",
        "BPPPBBBPPP",
        "BBBBPPPBBP",
        "BBPPPBPPPP",
        "BBPBBBBBBB",
        "BBPBBBBBBB",
        "BBPBBBBBBB"
    };

    void Start()
    {
        towerUpgradeUI = FindObjectOfType<TowerUpgradeUI>();
        buildButton.onClick.AddListener(PlaceTower);
        GenerateMap();
    }
    public void ClearTileSelection()
    {
        if (selectedCell.HasValue)
        {
            tilemap.SetColor(selectedCell.Value, Color.white);
            selectedCell = null;
        }
        buildButton.interactable = false;
    }
    void Update()
    {
        bool clickDetected = false;
        Vector2 inputPos = Vector2.zero;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            clickDetected = true;
            inputPos = Mouse.current.position.ReadValue();
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            clickDetected = true;
            inputPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (clickDetected)
        {
            Debug.Log("Click detected");

            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Blocked by UI");
                return;
            }

            if (towerUpgradeUI.WasButtonClickedThisFrame())
            {
                Debug.Log("Blocked by button click");
                return;
            }

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(inputPos);

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f);
            Debug.Log($"Raycast hit: {hit.collider?.name ?? "nothing"}");

            if (hit.collider != null && hit.collider is BoxCollider2D)
            {
                Debug.Log("Hit box collider");
                Tower tower = hit.collider.GetComponent<Tower>();
                if (tower != null)
                {
                    Debug.Log("Opening tower menu");
                    tower.OnTowerClicked();
                    return;
                }
            }
            else
            {
                towerUpgradeUI.CloseMenu();
            }

            Vector3Int cellPos = tilemap.WorldToCell(worldPos);
            SelectTile(cellPos);
        }
    }

    void SelectTile(Vector3Int cellPos)
    {
        buildButton.interactable = false;
        if (!tileOccupied.ContainsKey(cellPos)) { buildButton.interactable = false; return; }
        if (tileOccupied[cellPos]) { buildButton.interactable = false; return; }
        if (gameManager.playerGold >= 25)
        {
            buildButton.interactable = true;
            gameManager.buildButtonText.text = "BUILD";
        }
        else
        {
            gameManager.buildButtonText.text = "NO MONEY?!";
        }
        if (selectedCell.HasValue)
            tilemap.SetColor(selectedCell.Value, Color.white);

        selectedCell = cellPos;
        tilemap.SetColor(cellPos, Color.darkOrange);

        // Debug
        // Debug.Log($"Selected cell: {cellPos}");
        // Debug.Log($"Tile at position: {tilemap.GetTile(cellPos)}");
        // Debug.Log($"Tile color after set: {tilemap.GetColor(cellPos)}");
        // Debug.Log($"Tilemap material: {tilemap.GetComponent<TilemapRenderer>().material.name}");
    }
    void GenerateMap()
    {
        Vector3Int? startPos = null;

        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                char tileChar = map[y][x];
                Vector3Int position = new(x - 5, -(y - 10), 0);

                switch (tileChar)
                {
                    case 'P':
                        tileOccupied[position] = true;
                        tilemap.SetTile(position, pathTile);

                        // Find the starting tile (top-most path tile)
                        if (startPos == null || position.y > startPos.Value.y)
                            startPos = position;
                        break;

                    case 'B':
                        tileOccupied[position] = false;
                        tilemap.SetTile(position, buildTile);
                        break;

                    case 'X':
                        tileOccupied[position] = true;
                        tilemap.SetTile(position, blockedTile);
                        break;
                }
            }
        }
        if (startPos.HasValue)
            TracePath(startPos.Value);
    }

    void TracePath(Vector3Int current)
    {
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        while (true)
        {
            Vector3 worldPos = tilemap.CellToWorld(current);
            worldPos += new Vector3(0.55f, 0.5f, 0f); // center of the tile
            gameManager.waypoints.Add(worldPos);
            visited.Add(current);

            // Check all 4 neighbors
            Vector3Int[] neighbors = {
            current + Vector3Int.up,
            current + Vector3Int.down,
            current + Vector3Int.left,
            current + Vector3Int.right
        };

            Vector3Int? next = null;
            foreach (var neighbor in neighbors)
            {
                // If it's a path tile and we haven't visited it yet
                if (tileOccupied.ContainsKey(neighbor) &&
                    tileOccupied[neighbor] &&
                    tilemap.GetTile(neighbor) == pathTile &&
                    !visited.Contains(neighbor))
                {
                    next = neighbor;
                    break;
                }
            }

            if (!next.HasValue) break; // reached the end
            current = next.Value;
        }
    }
    void PlaceTower()
    {
        if (!selectedCell.HasValue) return;

        Vector3 worldPos = tilemap.CellToWorld(selectedCell.Value);

        worldPos.x += 0.5f; //temporary tower positioning
        worldPos.y += 1;
        worldPos.z = -1f; // in front of the tilemap

        Instantiate(towerPrefab, worldPos, Quaternion.identity);

        tileOccupied[selectedCell.Value] = true;
        tilemap.SetColor(selectedCell.Value, Color.white);
        selectedCell = null;
        buildButton.interactable = false;
        gameManager.playerGold -= 25;
        gameManager.UpdateUI();//.goldText.text = "Gold: " + gameManager.playerGold.ToString();
    }
}
