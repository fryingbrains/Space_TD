using System.Collections.Generic;
using UnityEngine;

public class Creep : MonoBehaviour
{
    public List<Vector3> waypoints = new();
    public GameManager gameManager;


    private int waypointIndex = 1;
    private float _health;
    private float _speed;
    public float Health
    {
        get { return _health; }
        set { _health = value; }
    }
    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }
    void Start()
    {
        gameManager = Camera.main.GetComponent<GameManager>();
    }
    public void TakeDamage(float damage)
    {
        Health -= damage;
    }
    public void SetCreep(int _health, int _speed, List<Vector3> _waypoints)
    {
        Health = _health;
        waypoints = _waypoints;
        Speed = _speed;
    }
    public void Update()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
            gameManager.playerGold += 1;
            gameManager.creepsAlive--;
            gameManager.UpdateUI();
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
                gameManager.livesLeft--;

                gameManager.creepsAlive--;
                if (gameManager.creepsAlive <= 0)
                {
                    gameManager.EndWave();
                }
                gameManager.UpdateUI();
                if (gameManager.livesLeft <= 0)
                {
                    gameManager.livesText.text = "GAME OVER!";
                }
                Destroy(gameObject);
            }
        }
    }
}
