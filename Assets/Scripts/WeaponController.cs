using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float range = 10f;
    Transform player;
    public GameObject projectilePrefab;
    Transform projectileSpawn;
    public float rateOfFire = 1;
    float timeSinceLastFire = 0;
    public float projectileForce = 20;
    public AudioSource bang;


    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        projectileSpawn = transform.Find("ProjectileSpawn").transform;
    }
    void Update()
    {
        Transform target = TagTargeter("Enemy");
        if (target != transform)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0;
            float rotationSpeed = 100f;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (timeSinceLastFire > rateOfFire)
            {
                GameObject projectile = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.identity);
                Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
                projectileRB.AddForce(projectileSpawn.transform.forward * projectileForce, ForceMode.VelocityChange);
                timeSinceLastFire = 0;
                bang.Play();
                Destroy(projectile, 5);
            }
            else
            {
                timeSinceLastFire += Time.deltaTime;
            }
        }

    }
    Transform TagTargeter(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        Transform closestTarget = transform;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            Vector3 difference = target.transform.position - player.position;
            float distance = difference.magnitude;

            if (distance < closestDistance && distance < range)
            {
                closestTarget = target.transform;
                closestDistance = distance;
            }
        }
        return closestTarget;
    }

    Transform LegeacyTargeter()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, range);
        Transform target = transform;
        float targetDistance = Mathf.Infinity;

        foreach (Collider collider in collidersInRange)
        {
            GameObject model = collider.gameObject;

            if (model.transform.parent != null)
            {
                GameObject enemy = model.transform.parent.gameObject;
                if (enemy.CompareTag("Enemy"))
                {
                    Vector3 diference = player.position - enemy.transform.position;
                    float distance = diference.magnitude;
                    if (distance < targetDistance)
                    {
                        target = enemy.transform;
                        targetDistance = distance;
                    }
                }
            }


        }
        return target;
    }
}