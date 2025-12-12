using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI bestScoreText;

    [Header("Lives Icons")]
    [SerializeField] Transform livesContainer;
    [SerializeField] GameObject lifeIconPrefab;

    [Header("Bombs Icons")]
    [SerializeField] Transform bombsContainer;
    [SerializeField] GameObject bombIconPrefab;

    public void SetLives(int lives)
    {
        Debug.Log($"SetLives({lives}) - Container: {livesContainer}, Prefab: {lifeIconPrefab}");
        if (livesContainer == null || lifeIconPrefab == null) 
        {
            Debug.LogError("LivesContainer o LifeIconPrefab no asignados!");
            return;
        }
        SyncIcons(livesContainer, lifeIconPrefab, lives);
    }

    public void SetBombs(int bombs)
    {
        Debug.Log($"SetBombs({bombs}) - Container: {bombsContainer}, Prefab: {bombIconPrefab}");
        if (bombsContainer == null || bombIconPrefab == null) 
        {
            Debug.LogError("BombsContainer o BombIconPrefab no asignados!");
            return;
        }
        SyncIcons(bombsContainer, bombIconPrefab, bombs);
    }

    public void SetScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Puntos: {score}";
    }

    public void SetBestScore(int best)
    {
        if (bestScoreText != null)
            bestScoreText.text = $"Mejor: {best}";
    }

    // Ensure the container has exactly 'count' children using the given prefab
    void SyncIcons(Transform container, GameObject prefab, int count)
    {
        Debug.Log($"SyncIcons: Container has {container.childCount} children, need {count}");
        count = Mathf.Max(0, count);

        // Remove extra icons
        for (int i = container.childCount - 1; i >= count; i--)
        {
            Debug.Log($"Destroying child {i}");
            Destroy(container.GetChild(i).gameObject);
        }

        // Add missing icons
        int toAdd = count - container.childCount;
        Debug.Log($"Adding {toAdd} icons");
        for (int i = 0; i < toAdd; i++)
        {
            var icon = Instantiate(prefab, container);
            Debug.Log($"Instantiated icon {i}: {icon.name}");
        }
    }
}
