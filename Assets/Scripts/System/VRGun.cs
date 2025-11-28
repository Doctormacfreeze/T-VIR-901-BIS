using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class XRGun : MonoBehaviour
{

    private bool canchangeObj = false;
    private bool canchangeObj2 = false;
    private bool canchangeObj3 = false;
    private DisplayObjScript dislayObj;
    [Header("Input Actions")]
    public InputActionReference fireAction;

    [Header("Gun Settings")]
    public Transform muzzleTransform;
    public float range = 50f;
    public LayerMask hitLayers;
    public float fireRate = 0.5f;
    private float lastFireTime = 0f;

    [Header("Ammo System")]
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadDistance = 0.3f;

    [Header("Muzzle Flash")]
    public ParticleSystem muzzleFlash;

    [Header("Animation & Audio")]
    public Animator gunAnimator;
    public AudioSource fireAudio;
    public AudioSource reloadAudio;
    public AudioSource emptySoundAudio; // ✅ AUDIOSOURCE POUR LE SON À VIDE

    [Header("Laser Reference")]
    public LineRenderer laserLine;
    public float laserDuration = 0.05f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isHeld = false;
    private Magazine nearbyMagazine;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        currentAmmo = maxAmmo;

        if (laserLine != null)
            laserLine.enabled = false;
    }



    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        fireAction.action.performed += OnFirePerformed;
        fireAction.action.Enable();
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);

        fireAction.action.performed -= OnFirePerformed;
        fireAction.action.Disable();
    }

    private void Update()
    {
        if (nearbyMagazine != null && IsMagazineCloseEnough(nearbyMagazine.transform))
        {
            Reload();
        }
    }

    private bool IsMagazineCloseEnough(Transform magazineTransform)
    {
        float distance = Vector3.Distance(transform.position, magazineTransform.position);
        return distance <= reloadDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        Magazine magazine = other.GetComponent<Magazine>();
        if (magazine != null)
        {
            nearbyMagazine = magazine;
            Debug.Log("Chargeur détecté à proximité");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Magazine magazine = other.GetComponent<Magazine>();
        if (magazine != null && magazine == nearbyMagazine)
        {
            nearbyMagazine = null;
            Debug.Log("Chargeur éloigné");
        }
    }

    private void PlayMuzzleFlash()
    {
        if (muzzleFlash == null) return;
        muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        muzzleFlash.Play();
    }

    private void OnGrab(SelectEnterEventArgs args){
        isHeld = true;
        if(canchangeObj == false && isHeld == true){
            canchangeObj = true;
            ObjectiveManager.Instance.CurrentObjectiveCompleted();
        }
    }
    private void OnRelease(SelectExitEventArgs args) => isHeld = false;

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        if (!isHeld) return;

        if (Time.time - lastFireTime < fireRate)
            return;

        if (currentAmmo <= 0)
        {
            // ✅ UTILISER L'AUDIOSOURCE EXISTANT
            PlayEmptySound();
            if(canchangeObj2 == false){
            canchangeObj2 = true;
            ObjectiveManager.Instance.CurrentObjectiveCompleted();
        }
            return;
        }

        lastFireTime = Time.time;
        Fire();
    }

    // ✅ METHODE SIMPLIFIÉE AVEC AUDIOSOURCE
    private void PlayEmptySound()
    {
        if (emptySoundAudio != null)
        {
            emptySoundAudio.Play();
        }
        else if (fireAudio != null)
        {
            // Fallback: utiliser le fireAudio mais différent
            fireAudio.pitch = 0.7f; // Son plus grave
            fireAudio.Play();
            fireAudio.pitch = 1f; // Remettre la pitch normale
        }

        Debug.Log("Click ! Plus de munitions");
    }

    private void Fire()
    {
        // Dépenser une munition
        currentAmmo--;
        UpdateAmmoUI();

        // Effets visuels et sonores
        PlayMuzzleFlash();

        if (fireAudio != null)
            fireAudio.Play();

        if (gunAnimator != null)
            gunAnimator.SetTrigger("Fire");

        // Calcul du tir
        Vector3 origin = muzzleTransform.position;
        Vector3 direction = muzzleTransform.forward;

        RaycastHit hit;
        bool hasHit = Physics.Raycast(origin, direction, out hit, range, hitLayers);

        Vector3 hitPoint = hasHit ? hit.point : origin + direction * range;

        // Afficher le laser
        if (laserLine != null)
            StartCoroutine(ShowLaser(origin, hitPoint));

        // Gérer l'impact
        if (hasHit)
        {
            HandleHit(hit);
        }

        Debug.Log($"Munitions restantes: {currentAmmo}/{maxAmmo}");
    }

    private void HandleHit(RaycastHit hit)
    {
        TargetDummy dummy = hit.collider.GetComponentInParent<TargetDummy>();
        if (dummy != null)
        {
            dummy.OnShot(hit.point, hit.normal, hit.collider);
        }
    }

    private IEnumerator ShowLaser(Vector3 start, Vector3 end)
    {
        if (laserLine == null) yield break;

        Vector3 localStart = transform.InverseTransformPoint(start);
        Vector3 localEnd = transform.InverseTransformPoint(end);

        laserLine.positionCount = 2;
        laserLine.SetPosition(0, localStart);
        laserLine.SetPosition(1, localEnd);
        laserLine.enabled = true;

        yield return new WaitForSeconds(laserDuration);

        laserLine.enabled = false;
    }

    private void Reload()
    {
        if (nearbyMagazine == null) return;

        // Recharger les munitions
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        // Effet sonore
        if (reloadAudio != null)
            reloadAudio.Play();

        // Utiliser le chargeur
        nearbyMagazine.OnUsed();
        nearbyMagazine = null;

        Debug.Log($"Rechargé ! Munitions: {currentAmmo}/{maxAmmo}");
    }

    private void UpdateAmmoUI()
    {
        if(canchangeObj3 == false){
            canchangeObj3 = true;
            ObjectiveManager.Instance.CurrentObjectiveCompleted();
        }
    }
}