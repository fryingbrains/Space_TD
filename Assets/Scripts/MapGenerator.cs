using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;

    public TileBase pathTile;
    public TileBase buildTile;
    public TileBase blockedTile;
    private Vector3Int? selectedCell = null;
    private Dictionary<Vector3Int, bool> tileOccupied = new Dictionary<Vector3Int, bool>();
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
        if (Mouse.current.leftButton.wasPressedThisFrame)
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
        Debug.Log($"Selected cell: {cellPos}");
        Debug.Log($"Tile at position: {tilemap.GetTile(cellPos)}");
        Debug.Log($"Tile color after set: {tilemap.GetColor(cellPos)}");
        Debug.Log($"Tilemap material: {tilemap.GetComponent<TilemapRenderer>().material.name}");
    }
    void GenerateMap()
    {
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                char tileChar = map[y][x];

                Vector3Int position = new Vector3Int(x - 5, -(y - 8), 0);

                switch (tileChar)
                {
                    case 'P':
                        tileOccupied[position] = true;  // path, unbuildable
                        tilemap.SetTile(position, pathTile);
                        break;

                    case 'B':
                        tileOccupied[position] = false;  // buildable
                        tilemap.SetTile(position, buildTile);
                        break;

                    case 'X':
                        tileOccupied[position] = true;  // blocked, unbuildable
                        tilemap.SetTile(position, blockedTile);
                        break;

                }
            }
        }
    }
    void PlaceTower()
    {
        if (!selectedCell.HasValue) return;

        Vector3 worldPos = tilemap.CellToWorld(selectedCell.Value);
        worldPos.x += 0.5f;
        worldPos.y += 1;
        worldPos.z = -1f; // in front of the tilemap
        Instantiate(towerPrefab, worldPos, Quaternion.identity);

        tileOccupied[selectedCell.Value] = true;
        tilemap.SetColor(selectedCell.Value, Color.white);
        selectedCell = null;
        buildButton.interactable = false;
    }
}
