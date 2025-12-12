using UnityEngine;
using System.Collections;

public class EnemyLowDificult : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float destroyPosition = 10f;
    [SerializeField] float turnPosition = -8f;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab; 
    [SerializeField] float shootInterval = 2f;

    [Header("Explosion")]
    [SerializeField] GameObject explosionPrefab; // Optional: particle system prefab
    [SerializeField] AudioClip explosionClip;    // Optional: audio clip
    [SerializeField] float destroyTimeDuration = 0.2f;
    [SerializeField] float sfxVolume = 0.8f;
    [SerializeField] bool destroyObjectAfterExplode = true;

    [Header("Scoring")]
    [SerializeField] int pointsOnDeath = 10;

    private Coroutine shootCoroutine;

    public int GetPointsOnDeath() => pointsOnDeath;
    
    // Métodos para configurar parámetros desde el spawner
    public void SetSpeed(float newSpeed) 
    { 
        speed = newSpeed;
        Debug.Log($"Enemigo velocidad ajustada a: {speed}");
    }
    
    public void SetPointsOnDeath(int newPoints)
    {
        pointsOnDeath = newPoints;
        Debug.Log($"Enemigo puntos ajustados a: {pointsOnDeath}");
    }
    
    public void SetShootInterval(float newInterval) 
    { 
        shootInterval = newInterval;
        Debug.Log($"[EnemyLowDificult] SetShootInterval() llamado con {newInterval}");
        if (shootCoroutine != null)
        {
            Debug.Log($"[EnemyLowDificult] Deteniendo coroutine anterior y creando uno nuevo");
            StopCoroutine(shootCoroutine);
            shootCoroutine = StartCoroutine(SpawnearDisparos());
        }
    }

    Vector3 linearVelocity = Vector3.left;
    int sentido = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"[EnemyLowDificult] Start() - projectilePrefab es {(projectilePrefab == null ? "NULL" : "VÁLIDO")}, shootInterval={shootInterval}");
        shootCoroutine = StartCoroutine(SpawnearDisparos());
        Debug.Log($"[EnemyLowDificult] Coroutine SpawnearDisparos iniciado");
    }

    IEnumerator SpawnearDisparos()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval); // Espera el intervalo definido entre disparos
            if (projectilePrefab == null)
            {
                Debug.LogWarning($"[EnemyLowDificult] ¡projectilePrefab ES NULL! No puedo disparar");
            }
            else
            {
                Debug.Log($"[EnemyLowDificult] Disparando enemigo en posición {transform.position}");
                Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(linearVelocity * speed * Time.deltaTime);
        if((sentido != 1) && (transform.position.x < turnPosition))
        {
            // Cambia de sentido al llegar a la posición de giro y indica que ya ha girado
            linearVelocity = Vector3.right;
            sentido = 1;
        }
        else if(transform.position.x > destroyPosition)
        {
            // Sólo se destruye, no explota, ya que ha salido de la pantalla
            Destroy(gameObject);
        }
    }

    // Handle collision with player projectiles
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("PlayerProjectile"))
        {
            // Desactiva el collider para evitar múltiples colisiones
            var myCollider = GetComponent<Collider2D>();
            if (myCollider != null)
                myCollider.enabled = false;
            
            // Destruye también el proyectil del jugador
            Destroy(collision.gameObject);
            
            // Explota antes de destruirse, ya que ha sido impactado por un poryectil del jugador
            ExplosionAndDestroy();
        }
    }

    // Explosion effect and destroy the enemy
    public void ExplosionAndDestroy()
    {
        // Add points to GameSession
        GameSession.Instance?.AddScore(pointsOnDeath);

        // Spawn VFX
        if (explosionPrefab != null)
        {
            var vfx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            // Auto-destroy VFX if it has a ParticleSystem
            var ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Object.Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax + destroyTimeDuration);
            }
            else
            {
                // Fallback: destroy after 2 seconds
                Object.Destroy(vfx, destroyTimeDuration);
            }
        }

        // Play SFX
        if (explosionClip != null)
        {
            AudioSource.PlayClipAtPoint(explosionClip, transform.position, sfxVolume);
        }

        if (destroyObjectAfterExplode)
        {
            Destroy(gameObject);
        }
    }
}
