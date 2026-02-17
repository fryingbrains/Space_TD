using System.Collections.Generic;
using UnityEngine;

public class Creep : MonoBehaviour
{
    public List<Vector3> waypoints = new();
    public GameManager gameManager;


    private int waypointIndex = 1;
    private int _health;
    private int _speed;
    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }
    public int Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }
    void Start()
    {
        gameManager = Camera.main.GetComponent<GameManager>();
        Debug.Log("Creep spawned");
        SetCreep();
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
    public void SetCreep()
    {
        Health = gameManager.currentWave + 4;
        Speed = 1;
    }
    public void Update()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
            gameManager.creepsAlive--;
            gameManager.creepsText.text = "Creeps Left: " + gameManager.creepsAlive.ToString();
            if (gameManager.creepsAlive <= 0)
            {
                gameManager.EndWave();
            }
        }
        //Debug.Log($"Current position: {transform.position}\nCurrent waypoint: {waypoints[waypointIndex]}");
        transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex], Time.deltaTime * Speed);
        if (Vector3.Distance(transform.position, waypoints[waypointIndex]) < 0.1f)
        {
            waypointIndex++;
            if (waypointIndex > waypoints.Count - 1)
            {
                gameManager.GameHealth--;
                gameManager.livesText.text = "Lives Left: " + gameManager.GameHealth.ToString();
                gameManager.creepsAlive--;
                gameManager.creepsText.text = "Creeps Left: " + gameManager.creepsAlive.ToString();
                Destroy(gameObject);
            }
        }
    }
}
