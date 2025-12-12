using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    // Configuración inicial (asignables desde Inspector)
    [SerializeField] int startingLives = 3;              // Vidas iniciales
    [SerializeField] int startingBombs = 2;              // Bombas iniciales
    [SerializeField] int maxBombs = 3;                   // Límite máximo de bombas
    [SerializeField] HUDController hud;                  // Referencia al HUD para actualizar pantalla
    [SerializeField] StartGameScreen startGameScreen;    // Referencia al menú para mostrar Game Over

    // Variables que se guardan durante todo el juego
    int lives;           // Vidas restantes del jugador
    int score;           // Puntuación actual
    int bestScore;       // Mejor puntuación de la sesión actual
    int bombs;           // Bombas disponibles (máximo 2)

    // Devuelve las variables
    public int GetLives() { return lives; }
    public int GetScore() { return score; }         
    public int GetBestScore() { return bestScore; }
    public int GetBombs() { return bombs; }

    void Awake()
    {
        // Singleton sencillo por escena; no persistimos entre escenas para evitar referencias nulas
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    // Inicializa el juego: carga vidas, score y bombas
    void Start()
    {
        ResetSession();
    }

    // Reinicia la sesión conservando el bestScore logrado hasta ahora
    public void ResetSession()
    {
        lives = startingLives;
        score = 0;
        bombs = Mathf.Clamp(startingBombs, 0, maxBombs);
        UpdateHUD();
    }

    // Suma puntos. Si supera el récord de la sesión, lo actualiza
    public void AddScore(int amount)
    {
        score += amount;
        if (score > bestScore)
        {
            bestScore = score;
        }
        UpdateHUD();
    }

    // Resta vidas. Si llega a 0, dispara Game Over
    public void LoseLife(int amount = 1)
    {
        lives = Mathf.Max(0, lives - amount);
        UpdateHUD();
        
        Debug.Log($"[GameSession] LoseLife - Vidas restantes: {lives}");
        
        if (lives <= 0)
        {
            TriggerGameOver();
        }
        else
        {
            // Aún quedan vidas - pedir respawn
            Debug.Log($"[GameSession] Quedan {lives} vidas - solicitando respawn");
            if (startGameScreen != null)
            {
                startGameScreen.RespawnPlayer();
            }
        }
    }

    // Gasta una bomba si hay disponible. Devuelve true si pudo usarla
    public bool UseBomb()
    {
        if (bombs <= 0) return false;
        bombs--;
        UpdateHUD();
        return true;
    }

    // Suma bombas respetando el límite máximo
    public void AddBomb(int amount = 1)
    {
        bombs = Mathf.Clamp(bombs + amount, 0, maxBombs);
        UpdateHUD();
    }

    // Envía los valores actuales al HUD para que se muestren en pantalla
    void UpdateHUD()
    {
        if (hud == null) return;
        hud.SetLives(lives);
        hud.SetBombs(bombs);
        hud.SetScore(score);
        hud.SetBestScore(bestScore);
    }

    // Finaliza el juego: muestra pantalla Game Over
    void TriggerGameOver()
    {
        Debug.Log("GameSession(TriggerGameOver): Game Over!");
        if (startGameScreen != null)
        {
            Debug.Log("GameSession(TriggerGameOver): Llamando a startGameOver.ShowGameOver()");
            startGameScreen.ShowGameOver();
        }
    }
}
