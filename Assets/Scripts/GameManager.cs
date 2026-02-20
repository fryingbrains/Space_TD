using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
public class GameManager : MonoBehaviour
{
    //public bool inWave;
    public List<Vector3> waypoints = new();
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI creepsText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI buildButtonText;
    public int playerGold = 45;
    public int currentWave = 0;
    public int creepsAlive = 0;
    public int livesLeft = 20;
    public Button startButton;
    public GameObject creepPrefab;
    public Button faster;
    public Button slower;
    private readonly WaitForSeconds onesec = new WaitForSeconds(1);

    public GameObject UI;


    void Start()
    {
        startButton.onClick.AddListener(StartWave);
        faster.onClick.AddListener(IncreaseSpeed);
        slower.onClick.AddListener(DecreaseSpeed);
        UpdateUI();
    }
    void IncreaseSpeed()
    {
        Time.timeScale *= 2;
    }
    void DecreaseSpeed()
    {
        Time.timeScale /= 2;
    }

    public void StartWave()
    {
        //Debug.Log("Starting wave");
        FindObjectOfType<MapGenerator>().ClearTileSelection();
        currentWave++;
        //inWave = true;
        UpdateUI();
        UI.SetActive(false);
        StartCoroutine(CreepSpawner());
    }
    IEnumerator CreepSpawner()
    {
        creepsText.text = "Creeps Left: " + (currentWave + 16);
        for (int i = 0; i < (currentWave + 16); i++)
        {
            creepsAlive++;
            GameObject go = Instantiate(creepPrefab, new Vector3(1.5f, 11, 0), Quaternion.identity);
            int health = currentWave + 1;
            if (currentWave % 5 == 0 && i == 10)
            {
                go.transform.localScale *= 2f;
                health *= 3;
            }
            go.GetComponent<Creep>().SetCreep(health, 1, waypoints);
            yield return onesec;

        }
    }
    public void EndWave()
    {
        //inWave = false;
        UI.SetActive(true);
    }
    public void UpdateUI()
    {
        goldText.text = "Gold: " + playerGold;
        waveText.text = "Wave: " + currentWave;
        creepsText.text = "Creeps Left: " + creepsAlive;
        livesText.text = "Lives Left: " + livesLeft;
        //More to come
    }
}
