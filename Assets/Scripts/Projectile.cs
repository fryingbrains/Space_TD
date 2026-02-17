using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    private Creep target;

    public void SetTarget(Creep targetCreep)
    {
        target = targetCreep;
    }

    void Update()
    {
        // If target died or is null, destroy projectile
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move toward target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotate to face target (optional, for arrow sprites)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Check if we hit the target
        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        target.TakeDamage(damage);
        Destroy(gameObject);
    }
}