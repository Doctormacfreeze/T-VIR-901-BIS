using UnityEngine;

public class KnifeDummy : MonoBehaviour
{
    [Header("Knife Dummy Settings")]
    public int maxHealth = 1000; // Très haute santé
    public int currentHealth;
    public bool isImmortal = true; // Immortel par défaut

    [Header("Knife Damage")]
    public int knifeDamage = 25; // Dégâts par coup de couteau

    [Header("Damage Tracking")]
    public int totalDamageTaken = 0;
    public int hitCount = 0;

    [Header("Effects")]
    public ParticleSystem hitEffect;
    public AudioClip knifeHitSound;
    public Renderer dummyRenderer;
    public Color hitColor = Color.red;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;

        if (dummyRenderer != null)
            originalColor = dummyRenderer.material.color;

        Debug.Log($"🔪 KnifeDummy initialisé: {gameObject.name}");
    }

    // Méthode pour les coups de couteau
    public void OnKnifeHit(Vector3 hitPoint, Vector3 hitNormal, int damage)
    {
        Debug.Log($"🔪 KnifeDummy touché! Dégâts: {damage}");

        // Appliquer les dégâts même si immortel
        if (isImmortal)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
        }

        // Tracker les statistiques
        totalDamageTaken += damage;
        hitCount++;

        PlayHitEffects(hitPoint, hitNormal);

        Debug.Log($"🔪 Santé: {currentHealth}/{maxHealth} | Total dégâts: {totalDamageTaken} | Coups: {hitCount}");
    }

    private void PlayHitEffects(Vector3 position, Vector3 normal)
    {
        // Effet de particules
        if (hitEffect != null)
        {
            hitEffect.transform.position = position;
            hitEffect.transform.rotation = Quaternion.LookRotation(normal);
            hitEffect.Play();
        }

        // Son pour couteau
        if (knifeHitSound != null)
        {
            AudioSource.PlayClipAtPoint(knifeHitSound, position);
        }

        // Feedback visuel
        if (dummyRenderer != null)
        {
            dummyRenderer.material.color = hitColor;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    private void ResetColor()
    {
        if (dummyRenderer != null)
            dummyRenderer.material.color = originalColor;
    }

    [ContextMenu("Reset Knife Dummy")]
    public void ResetDummy()
    {
        currentHealth = maxHealth;
        totalDamageTaken = 0;
        hitCount = 0;

        if (dummyRenderer != null)
            dummyRenderer.material.color = originalColor;

        Debug.Log("🔄 KnifeDummy reset!");
    }

    [ContextMenu("Afficher Stats")]
    public void DisplayStats()
    {
        Debug.Log($"=== STATS KNIFE DUMMY ===");
        Debug.Log($"Santé: {currentHealth}/{maxHealth}");
        Debug.Log($"Dégâts totaux: {totalDamageTaken}");
        Debug.Log($"Coups reçus: {hitCount}");
        Debug.Log($"Dégâts moyens par coup: {(hitCount > 0 ? totalDamageTaken / hitCount : 0)}");
    }
}