using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

// Pantalla de inicio y Game Over
public class StartGameScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Canvas screenCanvas;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] TextMeshProUGUI bestScoreText;

    [Header("Game Elements")]
    public GameObject player;
    public GameObject playerPrefab;
    [SerializeField] Vector3 playerSpawnPosition = new Vector3(-7f, 0f, 0f); // Posición inicial
    [SerializeField] Canvas hudCanvas; // Canvas completo del HUD
    [SerializeField] WaveManager waveManager; // Gestor de oleadas (controla los spawners)
    
    [Header("Backgrounds")]
    [SerializeField] GameObject startBackgroundImage; // Fondo pantalla inicio
    [SerializeField] GameObject gameSpaceBackground; // Fondo del juego (Space Background)

    [Header("Input")]
    [SerializeField] InputActionReference anyKey;
    [SerializeField] InputActionReference playerMove;
    [SerializeField] InputActionReference playerShoot;
    [SerializeField] InputActionReference playerBomb;

    bool gameStarted = false;

    void Start()
    {
        // Fuerza desactivación del HUD al iniciar el juego
        if (hudCanvas) 
        {
            hudCanvas.enabled = false;
            Debug.Log("StartGameScreen(Start): HUD Canvas desactivado en Start");
        }
        ShowStartScreen();
    }

    void OnEnable()
    {
        RegisterAnyKey();
    }
    
    void EnableInput()
    {
        // Ya no se usa, pero lo dejo por si acaso
    }

    void OnDisable()
    {
        UnregisterAnyKey();
    }

    void Update()
    {
        // Con el nuevo Input System, usa solo el InputAction (ya está en OnAnyKeyPressed)
        // Este Update solo se mantiene para debug si es necesario
    }

    void OnAnyKeyPressed(InputAction.CallbackContext context)
    {
        Debug.Log($"Tecla detectada - gameStarted: {gameStarted}");

        if (!gameStarted)
        {
            Debug.Log("Iniciando juego");
            StartGame();
        }
        else
        {
            Debug.Log("Reiniciando juego");
            RestartGame();
        }
    }

    void ShowStartScreen()
    {
        Time.timeScale = 1f; // no pausamos el tiempo; limpiamos objetos en lugar de congelarlos
        gameStarted = false;

        // Garantiza que no queden restos de juego visibles ni oleadas activas
        CleanupGameObjects();
        if (waveManager != null)
        {
            waveManager.StopWaveSystem();
        }

        // Asegura que el input de menú esté activo
        RegisterAnyKey();
        
        Debug.Log("Mostrando pantalla de inicio");
        
        if (screenCanvas) screenCanvas.enabled = true;
        if (titleText) 
        {
            titleText.text = "SPACE SHOOTER";
            titleText.alignment = TextAlignmentOptions.Center;
        }
        if (promptText) 
        {
            promptText.text = "Pulsa cualquier tecla para jugar";
            promptText.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
            promptText.alignment = TextAlignmentOptions.Center;
        }
        
        // Muestra mejor puntuación
        int best = GameSession.Instance?.GetBestScore() ?? 0;
        if (bestScoreText) bestScoreText.text = $"Mejor puntuación: {best}";
        
        // IMPORTANTE: Oculta elementos del juego
        if (player != null) 
        {
            player.SetActive(false);
            Debug.Log("Nave desactivada");
        }
        else
        {
            Debug.Log("No hay nave activa para desactivar (se spawneará al empezar)");
        }
        if (hudCanvas) 
        {
            hudCanvas.enabled = false;
            Debug.Log("Canvas HUD desactivado");
        }
        else
        {
            Debug.LogWarning("HUD Canvas no asignado en StartGameScreen!");
        }
        
        // Ya no hay spawners manuales, el WaveManager los controla
        
        // Cambiar fondos: muestra el de inicio, oculta el del juego
        if (startBackgroundImage) startBackgroundImage.SetActive(true);
        if (gameSpaceBackground) gameSpaceBackground.SetActive(false);
    }

    public void ShowGameOver()
    {
        Debug.Log("startGameScreen(ShowGameOver):Mostrando Game Over");
        
        // PRIMERO: Limpia todos los enemigos, proyectiles y efectos
        CleanupGameObjects();
        
        // Esperar 3 segundos antes de mostrar la pantalla de Game Over
        StartCoroutine(ShowGameOverDelayed());
    }
    
    IEnumerator ShowGameOverDelayed()
    {
        yield return new WaitForSeconds(3f);
        
        gameStarted = true; // Para que la siguiente tecla reinicie
        
        // DESPUÉS: Pausa el juego (ahora ya no hay objetos con delays activos)
        Time.timeScale = 0f;
        
        // Oculta elementos del juego
        if (hudCanvas) 
        {
            hudCanvas.enabled = false;
            Debug.Log("Canvas HUD desactivado en Game Over");
        }
        if (player != null)
        {
            Destroy(player); // Destruye para que se recree al reiniciar
            player = null;
            Debug.Log("Nave destruida en Game Over");
        }
        
        // Cambiar fondos
        if (gameSpaceBackground) gameSpaceBackground.SetActive(false);
        if (startBackgroundImage) startBackgroundImage.SetActive(true);

        // Rehabilita el input de menú
        RegisterAnyKey();
        
        if (screenCanvas) screenCanvas.enabled = true;
        if (titleText) 
        {
            titleText.text = "GAME OVER";
            titleText.alignment = TextAlignmentOptions.Center;
        }
        if (promptText) 
        {
            promptText.text = "Pulsa cualquier tecla para volver a jugar";
            promptText.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
            promptText.alignment = TextAlignmentOptions.Center;
        }
        
        // Muestra puntuación actual y mejor
        int score = GameSession.Instance?.GetScore() ?? 0;
        int best = GameSession.Instance?.GetBestScore() ?? 0;
        if (bestScoreText) bestScoreText.text = $"Puntuación: {score}\nMejor: {best}";
    }
    
    public void RespawnPlayer()
    {
        Debug.Log("[StartGameScreen] RespawnPlayer llamado");
        
        // Destruir nave anterior si existe
        if (player != null && player.scene.isLoaded)
        {
            Debug.Log("[StartGameScreen] Destruyendo nave anterior");
            Destroy(player);
            player = null;
        }
        
        // Spawn nueva nave desde prefab
        if (playerPrefab != null)
        {
            Debug.Log("[StartGameScreen] Spawneando nueva nave desde prefab");
            player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            player.SetActive(true);
            
            var playerScript = player.GetComponent<PlayerSpaceShip>();
            if (playerScript != null)
            {
                playerScript.startGameScreen = this;
                playerScript.move = playerMove;
                playerScript.shoot = playerShoot;
                playerScript.bomb = playerBomb;
                playerScript.RegisterInputActions();
                Debug.Log("[StartGameScreen] Nueva nave configurada y lista");
            }
        }
        else
        {
            Debug.LogError("[StartGameScreen] ¡playerPrefab es NULL! No se puede respawnear");
        }
    }
    
    void CleanupGameObjects()
    {
        // Destruye todas las naves enemigas SIN dar puntos
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }
        
        // Destruye proyectiles enemigos
        var projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        foreach (var proj in projectiles)
        {
            Destroy(proj);
        }
        
        // Destruye proyectiles del jugador
        var playerProjectiles = GameObject.FindGameObjectsWithTag("PlayerProjectile");
        foreach (var proj in playerProjectiles)
        {
            Destroy(proj);
        }
        
        // Destruye efectos visuales
        var effects = GameObject.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        foreach (var effect in effects)
        {
            Destroy(effect.gameObject);
        }
        
        Debug.Log($"Limpieza: {enemies.Length} naves, {projectiles.Length} proyectiles enemigos, {playerProjectiles.Length} proyectiles jugador, {effects.Length} efectos");
    }
    
    public void UseBombEffect()
    {
        Debug.Log("BOMBA ACTIVADA!");
        
        // Destruye todas las naves enemigas DÁNDOLES puntos
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int totalPoints = 0;
        
        foreach (var enemy in enemies)
        {
            var enemyScript = enemy.GetComponent<EnemyLowDificult>();
            if (enemyScript != null)
            {
                int points = enemyScript.GetPointsOnDeath();
                GameSession.Instance?.AddScore(points);
                totalPoints += points;
            }
            
            Destroy(enemy);
        }
        
        // Destruye proyectiles enemigos
        var projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        foreach (var proj in projectiles)
        {
            Destroy(proj);
        }
        
        Debug.Log($"Bomba: {enemies.Length} naves destruidas, {projectiles.Length} proyectiles, {totalPoints} puntos ganados");
        
        // TODO: Añadir VFX y SFX de bomba
    }

    void StartGame()
    {
        gameStarted = true;
        Debug.Log("StartGame - Activando elementos");
        
        // Deshabilita el input de menú para que no detecte teclas durante el juego
        UnregisterAnyKey();
        
        if (screenCanvas) screenCanvas.enabled = false;
        Time.timeScale = 1f;
        
        // Cambiar fondos: oculta el de inicio, muestra el del juego
        if (startBackgroundImage) startBackgroundImage.SetActive(false);
        if (gameSpaceBackground) gameSpaceBackground.SetActive(true);
        
        // Activa o spawnea la nave del jugador
        // CRÍTICO: Verificar que player no fue destruido en una partida anterior
        if (player != null && player.scene.isLoaded)
        {
            player.SetActive(true);
            
            var playerScript = player.GetComponent<PlayerSpaceShip>();
            if (playerScript != null)
            {
                playerScript.move = playerMove;
                playerScript.shoot = playerShoot;
                playerScript.bomb = playerBomb;
                playerScript.RegisterInputActions();
            }
            
            Debug.Log("Nave activada");
        }
        else if (playerPrefab != null)
        {
            player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            player.SetActive(true);
            
            var playerScript = player.GetComponent<PlayerSpaceShip>();
            if (playerScript != null)
            {
                playerScript.startGameScreen = this;
                playerScript.move = playerMove;
                playerScript.shoot = playerShoot;
                playerScript.bomb = playerBomb;
                playerScript.RegisterInputActions();
            }
            Debug.Log("Nave spawneada");
        }
        else
        {
            Debug.LogError("No hay player válido!");
        }
        
        if (hudCanvas) 
        {
            hudCanvas.enabled = true;
            Debug.Log("Canvas HUD activado");
        }
        else
        {
            Debug.LogError("HUD Canvas no asignado en StartGameScreen!");
        }
        
        // Inicia el sistema de oleadas (ya no necesitamos activar spawners manualmente)
        if (waveManager != null)
        {
            waveManager.StartWaveSystem();
            Debug.Log("Sistema de oleadas iniciado");
        }
        else
        {
            Debug.LogError("WaveManager no asignado en StartGameScreen!");
        }
        
        GameSession.Instance?.ResetSession();
    }
    
    public void ShowVictory()
    {
        Debug.Log("¡VICTORIA! Todas las oleadas completadas");
        
        // Limpia objetos restantes
        CleanupGameObjects();
        
        // Esperar 3 segundos antes de mostrar la pantalla de Victoria
        StartCoroutine(ShowVictoryDelayed());
    }
    
    IEnumerator ShowVictoryDelayed()
    {
        yield return new WaitForSeconds(3f);
        
        gameStarted = true;
        
        Time.timeScale = 0f;
        
        // Oculta elementos del juego
        if (hudCanvas) hudCanvas.enabled = false;
        if (player != null)
        {
            Destroy(player); // Destruye para que se recree al reiniciar
            player = null;
            Debug.Log("Nave destruida en Victoria");
        }
        
        // Cambiar fondos
        if (gameSpaceBackground) gameSpaceBackground.SetActive(false);
        if (startBackgroundImage) startBackgroundImage.SetActive(true);
        
        if (screenCanvas) screenCanvas.enabled = true;
        if (titleText) 
        {
            titleText.text = "¡VICTORIA!";
            titleText.alignment = TextAlignmentOptions.Center;
        }
        if (promptText) 
        {
            promptText.text = "Has completado todas las oleadas\nPulsa cualquier tecla para volver a jugar";
            promptText.textWrappingMode = TMPro.TextWrappingModes.Normal; // Permitir salto en este caso
            promptText.alignment = TextAlignmentOptions.Center;
        }
        
        // Muestra puntuación final
        int score = GameSession.Instance?.GetScore() ?? 0;
        int best = GameSession.Instance?.GetBestScore() ?? 0;
        if (bestScoreText) bestScoreText.text = $"Puntuación final: {score}\nMejor: {best}";

        // Rehabilita el input de menú
        RegisterAnyKey();
    }

    void RestartGame()
    {
        // Recarga la escena completa para reiniciar todo
        Debug.Log("Reiniciando escena...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    // --- Helpers de Input de menú ---
    void RegisterAnyKey()
    {
        if (anyKey != null)
        {
            anyKey.action.started -= OnAnyKeyPressed; // evitar doble suscripción
            anyKey.action.started += OnAnyKeyPressed;
            anyKey.action.Enable();
            Debug.Log("AnyKey habilitado (menú)");
        }
        else
        {
            Debug.LogWarning("AnyKey InputActionReference no asignado en StartGameScreen");
        }
    }

    void UnregisterAnyKey()
    {
        if (anyKey != null)
        {
            anyKey.action.started -= OnAnyKeyPressed;
            anyKey.action.Disable();
            Debug.Log("AnyKey deshabilitado (menú)");
        }
    }
}
