using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerSpaceShip : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float acceleration = 10f;   
    [SerializeField] float rawMoveThresholdForBraking = 0.1f;
    [SerializeField] float brakingFactor = 6f;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;

    [Header("Controls")]
    public InputActionReference move;
    public InputActionReference shoot;
    public InputActionReference bomb;
    public StartGameScreen startGameScreen;

    [Header("Explosion")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosionClip;
    [SerializeField] float sfxVolume = 0.8f;
 
    Vector2 rawMove;
    Vector2 currentVelocity = Vector2.zero;
    public Vector3 spawnPosition;
    bool isDead = false;
    bool isInvulnerable = false;
    public SpriteRenderer spriteRenderer;
    public Collider2D col;
 
    void Start()
    {
        spawnPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        
        if (startGameScreen == null)
        {
            startGameScreen = FindFirstObjectByType<StartGameScreen>();
        }
    }
 
    // Enable and disable input actions
    private void OnEnable()
    {
        RegisterInputActions();
    }

    void Update()
    {
        if(rawMove.magnitude < rawMoveThresholdForBraking)
        {
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, brakingFactor * Time.deltaTime);
        }
        else
        {
            currentVelocity += rawMove * acceleration * Time.deltaTime;
        }

        float linearVelocity = currentVelocity.magnitude;
        linearVelocity = Mathf.Clamp(linearVelocity, 0, maxSpeed);
        currentVelocity = currentVelocity.normalized * linearVelocity;
        
        float NewDespX = currentVelocity.x * Time.deltaTime;
        float NewDespY = currentVelocity.y * Time.deltaTime;
        float NewPosX = transform.position.x + NewDespX;
        float NewPosY = transform.position.y + NewDespY;
        if(NewPosX < -8.5f || NewPosX > 8.5f) NewDespX=0;
        if(NewPosY < -4f || NewPosY > 4f) NewDespY=0;

        transform.position += new Vector3(NewDespX, NewDespY, 0);
    }

    private void OnDisable()
    {
        UnregisterInputActions();
    }

    public void RegisterInputActions()
    {
        if (move != null && move.action != null)
        {
            move.action.started -= OnMove;
            move.action.performed -= OnMove;
            move.action.canceled -= OnMove;
            move.action.started += OnMove;
            move.action.performed += OnMove;
            move.action.canceled += OnMove;
            move.action.Enable();
        }

        if (shoot != null && shoot.action != null)
        {
            shoot.action.started -= OnShoot;
            shoot.action.started += OnShoot;
            shoot.action.Enable();
        }

        if (bomb != null && bomb.action != null)
        {
            bomb.action.performed -= OnBomb;
            bomb.action.performed += OnBomb;
            bomb.action.Enable();
        }
    }

    public void UnregisterInputActions()
    {
        if (move != null && move.action != null)
        {
            move.action.started -= OnMove;
            move.action.performed -= OnMove;
            move.action.canceled -= OnMove;
        }

        if (shoot != null && shoot.action != null)
        {
            shoot.action.started -= OnShoot;
        }
        
        if (bomb != null && bomb.action != null)
        {
            bomb.action.performed -= OnBomb;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        rawMove = context.ReadValue<Vector2>();
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (projectilePrefab != null)
        {
            Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        }
    }
    
    // Handle bomb input
    private void OnBomb(InputAction.CallbackContext context)
    {
        if (GameSession.Instance != null && GameSession.Instance.UseBomb())
        {
            if (startGameScreen != null)
            {
                startGameScreen.UseBombEffect();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead || isInvulnerable) return;
        
        if(!collision.gameObject.CompareTag("PlayerProjectile"))
        {
            // Si es enemigo, dar puntos
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemyScript = collision.gameObject.GetComponent<EnemyLowDificult>();
                if (enemyScript != null)
                {
                    GameSession.Instance?.AddScore(enemyScript.GetPointsOnDeath());
                }
            }
            
            Destroy(collision.gameObject);
            
            // Ocultar nave inmediatamente
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (col != null) col.enabled = false;
            
            DoExplosion();
            
            GameSession.Instance?.LoseLife(1);
            int lives = GameSession.Instance?.GetLives() ?? 0;
            
            if (lives > 0)
            {
                isDead = true; // Temporalmente muerto
                StartCoroutine(RespawnCoroutine());
            }
            else
            {
                isDead = true;
                Destroy(gameObject);
            }
        }
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // Pequeño delay
        Respawn();
    }

    public void Respawn()
    {
        Debug.Log("Respawneando nueva nave");
        
        GameObject prefab = startGameScreen.playerPrefab;
        if (prefab == null)
        {
            Debug.LogError("PlayerPrefab no asignado en StartGameScreen!");
            return;
        }
        
        // Crear nave SIN desactivar (como hace EnemySpawner)
        GameObject newShip = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        PlayerSpaceShip newScript = newShip.GetComponent<PlayerSpaceShip>();
        
        if (newScript != null)
        {
            // Asignar referencias
            newScript.move = this.move;
            newScript.shoot = this.shoot;
            newScript.bomb = this.bomb;
            newScript.startGameScreen = this.startGameScreen;
            
            // Ya está activo, Start() se ejecutó
            // Solo iniciamos invulnerabilidad
            newScript.StartCoroutine(newScript.InvulnerabilityTimer());
            Debug.Log("Nueva nave lista");
        }
        
        if (startGameScreen != null)
        {
            startGameScreen.player = newShip;
        }
        
        Destroy(gameObject);
    }
    
    IEnumerator InvulnerabilityTimer()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(2f);
        isInvulnerable = false;
    }

    // Solo hace la explosión visual, sin destruir
    private void DoExplosion()
    {
        var pos = transform.position;

        // Spawn VFX
        if (explosionPrefab != null)
        {
            var vfx = Instantiate(explosionPrefab, pos, Quaternion.identity);
            // Auto-destroy VFX if it has a ParticleSystem
            var ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Object.Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax + 0.2f);
            }
            else
            {
                // Fallback: destroy after 2 seconds
                Object.Destroy(vfx, 0.2f);
            }
        }

        // Play SFX
        if (explosionClip != null)
        {
            AudioSource.PlayClipAtPoint(explosionClip, pos, sfxVolume);
        }
    }
}
