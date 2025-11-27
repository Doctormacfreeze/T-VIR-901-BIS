using UnityEngine;
using System;

public class TargetDummy : MonoBehaviour
{
    [Header("Dummy Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Damage Zones")]
    public int headDamage = 100; // One shot
    public int bodyDamage = 50;  // 2 tirs pour tuer

    [Header("Zone References")]
    public Collider headCollider;
    public Collider bodyCollider;

    [Header("Effects")]
    public ParticleSystem hitEffect;
    public AudioClip hitSound;
    public AudioClip headshotSound; // Son spécial pour headshot
    public Renderer dummyRenderer;
    public Color hitColor = Color.red;
    public Color headshotColor = Color.yellow; // Couleur spéciale pour headshot
    private Color originalColor;

    [Header("Animation")]
    public Animator dummyAnimator;

    // Événement quand le dummy meurt
    public event Action<TargetDummy> OnDummyDied;

    private void Start()
    {
        currentHealth = maxHealth;

        if (dummyRenderer != null)
            originalColor = dummyRenderer.material.color;

        Debug.Log($"🎯 TargetDummy initialisé: {gameObject.name}");
    }

    // Méthode modifiée pour prendre en compte la zone touchée
    public void OnShot(Vector3 hitPoint, Vector3 hitNormal, Collider hitCollider)
    {
        int damage = GetDamageFromCollider(hitCollider);
        string zoneName = GetZoneName(hitCollider);

        Debug.Log($"🎯 Dummy touché ({zoneName})! Dégâts: {damage}, Vie restante: {currentHealth}");

        TakeDamage(damage, zoneName);
        PlayHitEffects(hitPoint, hitNormal, zoneName);
    }

    private int GetDamageFromCollider(Collider hitCollider)
    {
        if (hitCollider == headCollider)
        {
            return headDamage; // One shot
        }
        else // Corps ou autre
        {
            return bodyDamage; // 2 tirs
        }
    }

    private string GetZoneName(Collider hitCollider)
    {
        if (hitCollider == headCollider) return "TÊTE";
        return "CORPS";
    }

    private void TakeDamage(int damage, string zoneName)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // Feedback spécial pour headshot
        if (zoneName == "TÊTE")
        {
            ShowHeadshotFeedback();
        }
        else
        {
            ShowDamageFeedback();
        }

        if (currentHealth <= 0)
        {
            OnDestroyed(zoneName);
        }
    }

    private void PlayHitEffects(Vector3 position, Vector3 normal, string zoneName)
    {
        // EFFET DE PARTICULES - CODE AMÉLIORÉ
        if (hitEffect != null)
        {
            // Arrêter et nettoyer avant de rejouer (important !)
            hitEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // Repositionner
            hitEffect.transform.position = position;
            hitEffect.transform.rotation = Quaternion.LookRotation(normal);

            // Rejouer l'effet
            hitEffect.Play();

            Debug.Log($"💥 Particle System joué à: {position}");
        }
        else
        {
            Debug.LogWarning("⚠️ Hit Effect Particle System non assigné!");
        }

        // Son spécial pour headshot
        if (zoneName == "TÊTE" && headshotSound != null)
        {
            AudioSource.PlayClipAtPoint(headshotSound, position);
            Debug.Log($"🔊 Headshot son joué");
        }
        else if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, position);
            Debug.Log($"🔊 Hit son joué");
        }
    }

    private void ShowDamageFeedback()
    {
        if (dummyRenderer != null)
        {
            dummyRenderer.material.color = hitColor;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    private void ShowHeadshotFeedback()
    {
        if (dummyRenderer != null)
        {
            dummyRenderer.material.color = headshotColor;
            Invoke(nameof(ResetColor), 0.3f); // Durée un peu plus longue pour headshot
        }

        Debug.Log("🎯 HEADSHOT !");
    }

    private void ResetColor()
    {
        if (dummyRenderer != null)
            dummyRenderer.material.color = originalColor;
    }

    private void OnDestroyed(string zoneName)
    {
        string deathMessage = zoneName == "TÊTE" ? "🎯 HEADSHOT ! Dummy détruit !" : "💀 Dummy détruit !";
        Debug.Log(deathMessage);

        if (dummyAnimator != null)
            dummyAnimator.SetTrigger("Destroy");

        // Désactiver les composants
        if (dummyRenderer != null) dummyRenderer.enabled = false;
        DisableAllColliders();

        // Appeler l'événement de mort
        OnDummyDied?.Invoke(this);

        // Détruire l'objet après un délai
        Destroy(gameObject, 2f);
    }

    private void DisableAllColliders()
    {
        // Désactiver tous les colliders
        if (headCollider != null) headCollider.enabled = false;
        if (bodyCollider != null) bodyCollider.enabled = false;
    }

    // Pour réinitialiser le dummy
    public void Respawn()
    {
        currentHealth = maxHealth;

        if (dummyRenderer != null)
        {
            dummyRenderer.enabled = true;
            dummyRenderer.material.color = originalColor;
        }

        EnableAllColliders();

        if (dummyAnimator != null)
            dummyAnimator.SetTrigger("Respawn");

        Debug.Log("🔄 TargetDummy respawn!");
    }

    private void EnableAllColliders()
    {
        // Réactiver tous les colliders
        if (headCollider != null) headCollider.enabled = true;
        if (bodyCollider != null) bodyCollider.enabled = true;
    }

    // Méthode de debug pour vérifier l'état du Particle System
    [ContextMenu("Debug Particle System")]
    public void DebugParticleSystem()
    {
        if (hitEffect != null)
        {
            Debug.Log($"=== DEBUG PARTICLE SYSTEM ===");
            Debug.Log($"IsPlaying: {hitEffect.isPlaying}");
            Debug.Log($"IsAlive: {hitEffect.IsAlive()}");
            Debug.Log($"Particle Count: {hitEffect.particleCount}");
            Debug.Log($"Time: {hitEffect.time}");
            Debug.Log($"Position: {hitEffect.transform.position}");
        }
        else
        {
            Debug.LogWarning("Particle System non assigné!");
        }
    }
}