using UnityEngine;

public class Target : MonoBehaviour
{
    [HideInInspector] public TargetSpawner.SpawnPoint spawnPoint;

    [Header("Movimento")]
    [HideInInspector] public bool moveHorizontal = false;
    [HideInInspector] public bool moveVertical = false;
    [HideInInspector] public float moveSpeed = 3f;
    [HideInInspector] public float moveRange = 5f;

    [Header("Status")]
    [HideInInspector] public int health = 1;
    [HideInInspector] public int pointsValue = 1;

    private Vector3 startPosition;
    private float directionX = 1f;
    private float directionY = 1f;
    private bool destroyed = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        MoveTarget();
    }

    void MoveTarget()
    {
        Vector3 newPosition = transform.position;

        if (moveHorizontal)
        {
            newPosition.x += directionX * moveSpeed * Time.deltaTime;

            if (Mathf.Abs(newPosition.x - startPosition.x) >= moveRange)
            {
                directionX *= -1f;
            }
        }

        if (moveVertical)
        {
            newPosition.y += directionY * moveSpeed * Time.deltaTime;

            if (Mathf.Abs(newPosition.y - startPosition.y) >= moveRange)
            {
                directionY *= -1f;
            }
        }

        transform.position = newPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeHit(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeHit(collision.gameObject);
        }
    }

    void TakeHit(GameObject bullet)
    {
        if (destroyed)
        {
            return;
        }

        health--;

        if (bullet != null)
        {
            Destroy(bullet);
        }

        if (health <= 0)
        {
            destroyed = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(pointsValue);
            }

            Destroy(gameObject);
        }
    }
}