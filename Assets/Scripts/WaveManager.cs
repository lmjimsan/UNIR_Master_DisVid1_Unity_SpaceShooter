using UnityEngine;
using System.Collections;

/// <summary>
/// WaveManager: Controla el sistema de oleadas progresivas.
/// Características:
/// - 10 oleadas con aumento gradual de dificultad
/// - Escalado de enemigos: velocidad y frecuencia de disparo
/// - Cada oleada añade más enemigos (5 → 23 en la oleada 10)
/// - Transición a pantalla de Victoria al completar todas
/// 
/// FÓRMULAS DE ESCALADO:
/// - Enemigos: 5 + (ola-1)*2 = 5, 7, 9, 11...23
/// - Velocidad: 5 + (ola-1)*0.5 = 5, 5.5, 6, 6.5...9.5
/// - Intervalo disparo: max(0.5, 2 - (ola-1)*0.1) = 2, 1.9, 1.8...1.1
/// </summary>
public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] int totalWaves = 10;
    [SerializeField] float timeBetweenWaves = 10f;
    [SerializeField] int baseEnemiesPerWave = 5;
    [SerializeField] int enemiesIncreasePerWave = 2;
    
    [Header("Enemy Scaling")]
    [SerializeField] float baseEnemySpeed = 5f;
    [SerializeField] float speedIncreasePerWave = 0.5f;
    [SerializeField] float baseShootInterval = 2f;
    [SerializeField] float shootIntervalDecreasePerWave = 0.1f;
    [SerializeField] float minShootInterval = 0.5f;
    
    [Header("References")]
    [SerializeField] EnemySpawner spawner;
    [SerializeField] StartGameScreen startGameScreen;
    
    int currentWave = 0;
    bool waveSystemActive = false;
    
    void Start()
    {
        // No iniciamos automáticamente, esperamos a que el juego empiece
    }
    
    public void StartWaveSystem()
    {
        Debug.Log($"StartWaveSystem llamado - waveSystemActive: {waveSystemActive}");
        
        if (!waveSystemActive)
        {
            waveSystemActive = true;
            currentWave = 0;
            Debug.Log("Iniciando coroutine WaveLoop");
            StartCoroutine(WaveLoop());
        }
        else
        {
            Debug.LogWarning("WaveSystem ya estaba activo!");
        }
    }
    
    public void StopWaveSystem()
    {
        waveSystemActive = false;
        StopAllCoroutines();
    }
    
    IEnumerator WaveLoop()
    {
        while (currentWave < totalWaves && waveSystemActive)
        {
            currentWave++;
            Debug.Log($"=== OLEADA {currentWave}/{totalWaves} ===");
            
            // Calcula parámetros de esta oleada
            int enemyCount = baseEnemiesPerWave + (currentWave - 1) * enemiesIncreasePerWave;
            float enemySpeed = baseEnemySpeed + (currentWave - 1) * speedIncreasePerWave;
            float shootInterval = Mathf.Max(
                minShootInterval, 
                baseShootInterval - (currentWave - 1) * shootIntervalDecreasePerWave
            );
            
            Debug.Log($"Enemigos: {enemyCount}, Velocidad: {enemySpeed}, Intervalo disparo: {shootInterval}");
            
            // Genera la oleada
            if (spawner != null)
            {
                Debug.Log("Llamando a spawner.SpawnWave()");
                spawner.SpawnWave(enemyCount, enemySpeed, shootInterval);
            }
            else
            {
                Debug.LogError("Spawner es NULL en WaveManager!");
            }
            
            // Espera antes de la siguiente oleada
            yield return new WaitForSeconds(timeBetweenWaves);
        }
        
        // Todas las oleadas completadas
        if (waveSystemActive)
        {
            Debug.Log("¡TODAS LAS OLEADAS COMPLETADAS!");
            yield return new WaitForSeconds(2f); // Pequeña pausa
            
            // Muestra pantalla de victoria (o Game Over por ahora)
            if (startGameScreen != null)
            {
                startGameScreen.ShowVictory();
            }
        }
    }
    
    public int GetCurrentWave() => currentWave;
    public int GetTotalWaves() => totalWaves;
}
