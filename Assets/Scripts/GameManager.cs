using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static bool inWave;
    public List<Vector3> waypoints = new();
    public static int GameHealth = 10;
    public static int currentWave = 1;
    public Button startButton;
    public GameObject creepPrefab;
    private readonly WaitForSeconds onesec = new WaitForSeconds(1);

    public GameObject canvas;

    void Start()
    {
        startButton.onClick.AddListener(StartWave);
    }

    public void StartWave()
    {
        Debug.Log("Starting wave");
        inWave = true;
        canvas.SetActive(false);
        StartCoroutine(CreepSpawner());
    }
    IEnumerator CreepSpawner()
    {
        for (int i = 0; i < (currentWave + 10); i++)
        {
            GameObject go = Instantiate(creepPrefab, new Vector3(1.5f, 11, 0), Quaternion.identity);
            go.GetComponent<Creep>().waypoints = waypoints;
            yield return onesec;
        }
    }
}
