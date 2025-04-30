using UnityEngine;
using UnityEngine.AI; // Solo si usas NavMeshAgent

public class EnemyFollow : MonoBehaviour
{
    public float detectionRange = 15f;
    public float baseSpeed = 2f;
    public float speedIncreaseRate = 0.5f; // Aumento de velocidad por minuto
    public Transform player;

    private float currentSpeed;
    private float timer = 0f;

    // Si usas NavMeshAgent
    private NavMeshAgent agent;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        currentSpeed = baseSpeed;

        // Solo si usas NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = currentSpeed;
        }
    }

    void Update()
    {
        // Aumenta la dificultad con el tiempo
        timer += Time.deltaTime;
        currentSpeed = baseSpeed + (timer / 60f) * speedIncreaseRate;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            FollowPlayer();
        }
        else
        {
            StopMovement();
        }
    }

    void FollowPlayer()
    {
        if (agent != null)
        {
            agent.speed = currentSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            // Movimiento sin NavMeshAgent (dirección directa)
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * currentSpeed * Time.deltaTime;
            transform.LookAt(player);
        }
    }

    void StopMovement()
    {
        if (agent != null)
        {
            agent.SetDestination(transform.position);
        }
    }
}
