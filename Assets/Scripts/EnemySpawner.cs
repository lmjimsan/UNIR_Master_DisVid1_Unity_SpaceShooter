using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    public enum SpawnMode
    {
        Line,
        Points, 
    }
    [SerializeField] SpawnMode spawnMode;
    [SerializeField] Transform[] spawnPoints;

    [SerializeField] Transform spawnLineTop;
    [SerializeField] Transform spawnLineBottom;

    // Ya no spawn automático en Start, lo controlará WaveManager
    void Start()
    {
        // Vacío - el WaveManager controla cuándo spawnear
    }
    
    // Método público para spawnear una oleada con parámetros variables
    public void SpawnWave(int enemyCount, float enemySpeed, float shootInterval)
    {
        Debug.Log($"SpawnWave llamado - Count: {enemyCount}, Speed: {enemySpeed}, Interval: {shootInterval}, Mode: {spawnMode}");
        
        if (spawnMode == SpawnMode.Line) 
        {
            StartCoroutine(LineSpawning(enemyCount, enemySpeed, shootInterval));
        }
        else if (spawnMode == SpawnMode.Points) 
        {
            StartCoroutine(PointSpawning(enemyCount, enemySpeed, shootInterval));
        }
    }

    IEnumerator LineSpawning(int count, float speed, float shootInterval)
    {
        Vector3 lineTop = spawnLineTop.position;
        Vector3 lineBottom = spawnLineBottom.position; 

        for (int i = 0; i < count; i++) 
        { 
            float t = Random.Range(0f, 1f);
            Vector3 startPosition = Vector3.Lerp(lineTop, lineBottom, t);
            
            // Instancia y configura el enemigo
            GameObject enemy = Instantiate(enemyPrefab, startPosition, Quaternion.identity);
            ConfigureEnemy(enemy, speed, shootInterval);
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    IEnumerator PointSpawning(int count, float speed, float shootInterval)
    {
        int numPoints = spawnPoints.Length;
        int enemiesToSpawn = Mathf.Min(count, numPoints);
        
        for (int i = 0; i < enemiesToSpawn; i++) 
        { 
            Vector3 startPosition = spawnPoints[i].position;
            
            // Instancia y configura el enemigo
            GameObject enemy = Instantiate(enemyPrefab, startPosition, Quaternion.identity);
            ConfigureEnemy(enemy, speed, shootInterval);
            
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    void ConfigureEnemy(GameObject enemy, float speed, float shootInterval)
    {
        Debug.Log($"[EnemySpawner] ConfigureEnemy() - enemy activo: {enemy.activeSelf}, nombre: {enemy.name}");
        
        // Asigna el tag "Enemy" en caso de que el prefab no lo tenga
        enemy.tag = "Enemy";
        
        var enemyScript = enemy.GetComponent<EnemyLowDificult>();
        Debug.Log($"[EnemySpawner] enemyScript es {(enemyScript == null ? "NULL" : "VÁLIDO")}");
        if (enemyScript != null)
        {
            enemyScript.SetSpeed(speed);
            enemyScript.SetShootInterval(shootInterval);
            
            // Calcular puntos según oleada actual (más oleada = más puntos)
            // Obtener oleada del WaveManager
            var waveManager = FindFirstObjectByType<WaveManager>();
            if (waveManager != null)
            {
                int currentWave = waveManager.GetCurrentWave();
                int points = 10 + (currentWave - 1) * 5; // 10, 15, 20, 25... hasta 55 puntos en oleada 10
                enemyScript.SetPointsOnDeath(points);
            }
            
            Debug.Log($"Enemigo configurado - Velocidad: {speed}, Intervalo: {shootInterval}");
        }
    }
}
