using UnityEngine;

public class Target : MonoBehaviour
{
    [HideInInspector] public TargetSpawner.SpawnPoint spawnPoint;

    [HideInInspector] public bool moveHorizontal = false;
    [HideInInspector] public bool moveVertical = false;
    [HideInInspector] public float moveSpeed = 3f;
    [HideInInspector] public float moveRange = 5f;

    [HideInInspector] public int health = 1;
    [HideInInspector] public int pointsValue = 1;

    private Vector3 startPosition;
    private float directionX = 1f;
    private float directionY = 1f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
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
            health--;

            Destroy(other.gameObject);

            if (health <= 0)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(pointsValue);
                }

                Destroy(gameObject);
            }
        }
    }
}