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
    private string[] map =
    {
        "BBBBBBPBBB",
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
        buildButton.onClick.AddListener(PlaceTower);
        GenerateMap();
    }


    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !GameManager.inWave)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; // ignore clicks on UI
            {
                Debug.Log("Clicked");
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Vector3Int cellPos = tilemap.WorldToCell(worldPos);
                SelectTile(cellPos);
            }
        }
    }

    void SelectTile(Vector3Int cellPos)
    {
        buildButton.interactable = false;
        if (!tileOccupied.ContainsKey(cellPos)) { buildButton.interactable = false; return; }
        if (tileOccupied[cellPos]) { buildButton.interactable = false; return; }
        buildButton.interactable = true;
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
                Vector3Int position = new(x - 5, -(y - 8), 0);

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
            worldPos += new Vector3(0.6f, 0.5f, 0f); // center of the tile
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
    }
}
