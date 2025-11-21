using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Magazine : MonoBehaviour
{
    [Header("Magazine Settings")]
    public int ammoCapacity = 10;
    public bool isInfinite = false; // Si true, recharge infiniment

    [Header("Visual Feedback")]
    public Renderer magazineRenderer;
    public Color usedColor = Color.gray;
    private Color originalColor;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isUsed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (magazineRenderer != null)
            originalColor = magazineRenderer.material.color;
    }

    public void OnUsed()
    {
        if (isInfinite) return; // Si infini, ne pas se désactiver

        isUsed = true;

        // Feedback visuel
        if (magazineRenderer != null)
            magazineRenderer.material.color = usedColor;

        // Désactiver l'interaction temporairement
        if (grabInteractable != null)
            grabInteractable.enabled = false;

        // Réactiver après un délai (pour réutilisation)
        Invoke(nameof(ResetMagazine), 3f);
    }

    private void ResetMagazine()
    {
        isUsed = false;

        // Réactiver l'apparence
        if (magazineRenderer != null)
            magazineRenderer.material.color = originalColor;

        // Réactiver l'interaction
        if (grabInteractable != null)
            grabInteractable.enabled = true;
    }

    // Pour reset manuellement
    public void Reset()
    {
        CancelInvoke(nameof(ResetMagazine));
        isUsed = false;

        if (magazineRenderer != null)
            magazineRenderer.material.color = originalColor;

        if (grabInteractable != null)
            grabInteractable.enabled = true;
    }
}