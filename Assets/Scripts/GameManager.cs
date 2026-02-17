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
    public bool inWave;
    public List<Vector3> waypoints = new();
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI creepsText;
    public int GameHealth = 10;
    public int currentWave = 0;
    public int creepsAlive = 0;
    public Button startButton;
    public GameObject creepPrefab;
    private readonly WaitForSeconds onesec = new WaitForSeconds(1);

    public GameObject UI;

    void Start()
    {
        startButton.onClick.AddListener(StartWave);
        livesText.text = "Lives Left: " + 10;
    }

    public void StartWave()
    {
        Debug.Log("Starting wave");
        currentWave++;
        waveText.text = "Wave: " + currentWave.ToString();
        inWave = true;
        UI.SetActive(false);
        StartCoroutine(CreepSpawner());
    }
    IEnumerator CreepSpawner()
    {
        creepsText.text = "Creeps Left: " + (currentWave + 10).ToString();
        for (int i = 0; i < (currentWave + 10); i++)
        {
            creepsAlive++;
            GameObject go = Instantiate(creepPrefab, new Vector3(1.5f, 11, 0), Quaternion.identity);
            go.GetComponent<Creep>().waypoints = waypoints;
            yield return onesec;
        }
    }
    public void EndWave()
    {
        inWave = false;
        UI.SetActive(true);
    }
}
