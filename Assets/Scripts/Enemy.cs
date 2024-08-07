using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField]
    [Tooltip("Prefab for the VFX played when the enemy dies.")]
    GameObject deathFX;

    [SerializeField]
    [Tooltip("Prefab for the VFX played when the enemy is hit.")]
    GameObject hitVFX;

    [Header("Combat Settings")]
    [SerializeField]
    [Tooltip("Score gained by the player when this enemy is killed.")]
    int scorePerKill = 15;

    [SerializeField]
    [Tooltip("Number of hits this enemy can sustain before being killed.")]
    int hitPoints = 2;

    [SerializeField]
    [Tooltip("Offset position for the hit VFX relative to the enemy's position.")]
    Vector3 hitVFXOffset = Vector3.zero;

    ScoreBoard scoreBoard;
    GameObject VFXParent;

    void Start()
    {
        // Cache the reference to the ScoreBoard to avoid repeated FindObjectOfType calls
        scoreBoard = FindObjectOfType<ScoreBoard>();
        VFXParent = GameObject.FindWithTag("SpawnAtRuntime");
        AddRigidbody();
    }

    void AddRigidbody()
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
    }


    void OnParticleCollision(GameObject other)
    {
        ProcessHit();
        if (hitPoints < 1)
        {
            KillEnemy();
            scoreBoard.IncreaseScore(scorePerKill);
        }
    }

    void ProcessHit()
    {
        GameObject vfx = Instantiate(hitVFX, transform.position + hitVFXOffset, Quaternion.identity);
        vfx.transform.parent = VFXParent.transform;
        hitPoints--;
    }

    void KillEnemy()
    {
        GameObject vfx = Instantiate(deathFX, transform.position, Quaternion.identity);
        vfx.transform.parent = VFXParent.transform;
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    // Visualization for hitVFX position adjustment in the Unity Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 vfxPosition = transform.position + hitVFXOffset;
        Gizmos.DrawSphere(vfxPosition, 0.1f);

        Vector3 textPosition = vfxPosition + Vector3.up * 0.5f;
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.red;
        Handles.Label(textPosition, "Hit VFX Position", style);
    }
#endif
}
