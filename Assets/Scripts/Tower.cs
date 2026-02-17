
using UnityEngine;
using System.Collections.Generic;

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
    public float fireRate = 1f;        // shots per second
    public float range = 3.3f;           // detection range

    private Creep currentTarget;
    private float nextFireTime;
    private List<Creep> creepsInRange = new List<Creep>();

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
        Debug.Log("Triggered");
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
            proj.SetTarget(currentTarget);
    }
}
