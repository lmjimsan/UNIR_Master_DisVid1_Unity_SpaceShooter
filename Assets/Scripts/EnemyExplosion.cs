using UnityEngine;

// Attach this to ships or enemies. Call Explode() when they die.
public class EnemyExplosion : MonoBehaviour
{
    [Header("Explosion VFX/SFX")]
    [SerializeField] GameObject explosionPrefab; // Optional: particle system prefab
    [SerializeField] AudioClip explosionClip;    // Optional: audio clip
    [SerializeField] float sfxVolume = 0.8f;
    [SerializeField] bool destroyObjectAfterExplode = true;

    public void FXExplosion()
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
                Object.Destroy(vfx, 2f);
            }
        }

        // Play SFX
        if (explosionClip != null)
        {
            AudioSource.PlayClipAtPoint(explosionClip, pos, sfxVolume);
        }

        if (destroyObjectAfterExplode)
        {
            Destroy(gameObject);
        }
    }
}