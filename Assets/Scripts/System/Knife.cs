using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class Knife : MonoBehaviour
{
    [Header("Knife Settings")]
    public int damage = 25;
    public float attackCooldown = 0.3f;

    [Header("Effects")]
    public ParticleSystem hitEffect;

    private bool canAttack = true;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🎯 === DÉBUT OnTriggerEnter ===");
        Debug.Log($"🎯 Couteau a touché: {other.gameObject.name}");
        Debug.Log($"🎯 Layer: {other.gameObject.layer} (LayerMask: {LayerMask.LayerToName(other.gameObject.layer)})");
        Debug.Log($"🎯 Tag: {other.gameObject.tag}");
        Debug.Log($"🎯 IsTrigger: {other.isTrigger}");
        Debug.Log($"🎯 Position couteau: {transform.position}");

        if (!canAttack)
        {
            Debug.Log("❌ Attaque en cooldown - Ignoré");
            return;
        }

        ProcessHit(other);
        Debug.Log($"🎯 === FIN OnTriggerEnter ===\n");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"💥 === DÉBUT OnCollisionEnter ===");
        Debug.Log($"💥 Collision avec: {collision.gameObject.name}");
        Debug.Log($"💥 Force: {collision.impulse.magnitude}");

        if (!canAttack)
        {
            Debug.Log("❌ Attaque en cooldown - Ignoré");
            return;
        }

        ProcessHit(collision.collider);
        Debug.Log($"💥 === FIN OnCollisionEnter ===\n");
    }

    private void ProcessHit(Collider other)
    {
        Debug.Log($"🔍 Recherche de KnifeDummy sur: {other.gameObject.name}");

        // Chercher le KnifeDummy dans le parent
        KnifeDummy knifeDummy = other.GetComponentInParent<KnifeDummy>();
        if (knifeDummy != null)
        {
            Debug.Log($"✅ KNIFEDUMMY TROUVÉ: {knifeDummy.gameObject.name}");

            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = (transform.position - hitPoint).normalized;

            Debug.Log($"📍 Point d'impact: {hitPoint}");
            Debug.Log($"📐 Normal: {hitNormal}");

            knifeDummy.OnKnifeHit(hitPoint, hitNormal, damage);

            // Effet visuel
            if (hitEffect != null)
            {
                hitEffect.transform.position = hitPoint;
                hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
                hitEffect.Play();
                Debug.Log($"✨ Effet de hit joué");
            }
            else
            {
                Debug.Log($"❌ Aucun effet de hit assigné");
            }

            StartCoroutine(AttackCooldown());
        }
        else
        {
            Debug.Log($"❌ Aucun KnifeDummy trouvé sur {other.gameObject.name}");

            // Chercher dans les enfants pour debug
            KnifeDummy[] childDummys = other.GetComponentsInChildren<KnifeDummy>();
            if (childDummys.Length > 0)
            {
                Debug.Log($"🔍 KnifeDummy trouvé dans les enfants: {childDummys[0].gameObject.name}");
            }

            // Chercher un TargetDummy pour voir si on touche le mauvais type
            TargetDummy targetDummy = other.GetComponentInParent<TargetDummy>();
            if (targetDummy != null)
            {
                Debug.Log($"🎯 TargetDummy trouvé (mais pas KnifeDummy): {targetDummy.gameObject.name}");
            }

            // Lister tous les composants pour debug
            Debug.Log($"📋 Composants sur {other.gameObject.name}:");
            Component[] components = other.GetComponents<Component>();
            foreach (Component comp in components)
            {
                Debug.Log($"   - {comp.GetType().Name}");
            }

            // Vérifier la hiérarchie
            if (other.transform.parent != null)
            {
                Debug.Log($"🏗️ Parent: {other.transform.parent.name}");
                KnifeDummy parentKnifeDummy = other.transform.parent.GetComponent<KnifeDummy>();
                if (parentKnifeDummy != null)
                {
                    Debug.Log($"✅ KnifeDummy trouvé sur le parent!");
                }
            }
        }
    }

    private System.Collections.IEnumerator AttackCooldown()
    {
        Debug.Log($"⏰ Début du cooldown: {attackCooldown}s");
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        Debug.Log($"✅ Cooldown terminé - Prêt à attaquer");
    }

    // Méthode pour tester si le couteau fonctionne
    [ContextMenu("Test Couteau Manuellement")]
    public void TestKnifeManually()
    {
        Debug.Log($"🧪 === TEST MANUEL DU COUTEAU ===");

        // Vérifier si le couteau a un collider
        Collider knifeCollider = GetComponent<Collider>();
        if (knifeCollider != null)
        {
            Debug.Log($"✅ Couteau a un collider: {knifeCollider.GetType().Name}");
            Debug.Log($"✅ IsTrigger: {knifeCollider.isTrigger}");
            Debug.Log($"✅ Enabled: {knifeCollider.enabled}");
        }
        else
        {
            Debug.Log($"❌ Couteau n'a pas de collider!");
        }

        // Chercher tous les KnifeDummy dans la scène
        KnifeDummy[] allKnifeDummys = FindObjectsOfType<KnifeDummy>();
        Debug.Log($"🔍 {allKnifeDummys.Length} KnifeDummy trouvés dans la scène:");
        foreach (KnifeDummy dummy in allKnifeDummys)
        {
            Debug.Log($"   - {dummy.gameObject.name} (position: {dummy.transform.position})");
        }

        Debug.Log($"🧪 === FIN TEST MANUEL ===\n");
    }

    private void Start()
    {
        Debug.Log($"🔪 Couteau initialisé: {gameObject.name}");
        Debug.Log($"🔪 Position: {transform.position}");
        Debug.Log($"🔪 CanAttack: {canAttack}");
    }

    private void Update()
    {
        // Debug visuel - dessiner un ray en avant du couteau
        Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.red);
    }
}