
using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class Tower : MonoBehaviour
{
    //private int _damage;
    //private int _range;
    // public int Damage
    // {
    //     get { return _damage; }
    //     set { _damage = value; }
    // }
    // public int Range
    // {
    //     get { return _range; }
    //     set { _range = value; }
    // }
    public GameObject projectilePrefab;
    private float fireRate = 1f;        // shots per second
    private float range = 3f;           // detection range
    private float currentDamage = 1;

    private Creep currentTarget;
    private float nextFireTime;
    private List<Creep> creepsInRange = new List<Creep>();
    private GameManager gameManager;
    public int rangeUpgradeCost = 5;
    public int fireRateUpgradeCost = 5;
    public int damageUpgradeCost = 5;
    public int ElementalUpgradeCost = 55;

    private TowerUpgradeUI towerUpgradeUI;

    void Start()
    {
        gameManager = Camera.main.GetComponent<GameManager>();
        towerUpgradeUI = FindObjectOfType<TowerUpgradeUI>();
    }

    public void OnTowerClicked()
    {
        towerUpgradeUI.ShowUpgradeMenu(this);
    }

    public void UpgradeRange()
    {
        gameManager.playerGold -= rangeUpgradeCost;
        rangeUpgradeCost += 5;
        range += range / 4;
        GetComponent<CircleCollider2D>().radius = range;
    }
    public void UpgradeFireRate()
    {
        gameManager.playerGold -= fireRateUpgradeCost;
        fireRateUpgradeCost += 5;
        fireRate += fireRate / 4f;
    }

    public void UpgradeDamage()
    {
        gameManager.playerGold -= damageUpgradeCost;
        damageUpgradeCost += 5;
        currentDamage += currentDamage / 4f;
    }

    void Update()
    {
        // Clean up dead/null creeps from our list
        creepsInRange.RemoveAll(c => c == null);

        // If current target is dead or out of range, find a new one
        if (currentTarget == null || !creepsInRange.Contains(currentTarget))
        {
            currentTarget = GetClosestCreep();
        }

        // Shoot at target
        if (currentTarget != null && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Triggered");
        Creep creep = other.GetComponent<Creep>();
        if (creep != null)
        {
            creepsInRange.Add(creep);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Creep creep = other.GetComponent<Creep>();
        if (creep != null)
        {
            creepsInRange.Remove(creep);
            if (currentTarget == creep)
                currentTarget = null;
        }
    }

    Creep GetClosestCreep()
    {
        if (creepsInRange.Count == 0) return null;

        Creep closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Creep creep in creepsInRange)
        {
            float dist = Vector3.Distance(transform.position, creep.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = creep;
            }
        }

        return closest;
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Assuming your projectile has a script that needs the target
        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
            proj.SetProjectile(currentTarget, currentDamage);
    }
}
