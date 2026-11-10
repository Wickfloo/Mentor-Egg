using Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts;
using System.Collections;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public static bool IsPlayerDead = false;
    public static Transform ActiveSpawnPoint;
    public static Vector3 ActiveTargetScale = new Vector3(1f, 1f, 1f); // Tracks scale for current checkpoint

    [Header("Respawn Settings")]
    [Tooltip("Drag your Checkpoint/Spawn Transform here in the Inspector.")]
    public Transform SpawnPoint;

    [Tooltip("Time to wait while the death animation/flash plays before respawning.")]
    public float RespawnDelay = 1.5f;

    [Header("Scale Settings")]
    [Tooltip("Should this checkpoint/hazard change the monster's scale?")]
    public bool ApplyCustomScale = false;

    [Tooltip("The target scale to apply to the monster.")]
    public Vector3 TargetScale = new Vector3(1f, 1f, 1f);

    private static bool _isRespawning = false;

    private void Start()
    {
        IsPlayerDead = false;
        _isRespawning = false;

        if (ActiveSpawnPoint == null && SpawnPoint != null)
        {
            ActiveSpawnPoint = SpawnPoint;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isRespawning) return;

        if (collision.GetComponent<MonsterAnimation>() != null ||
            collision.name.Contains("Spider") ||
            collision.name.Contains("Rat") ||
            collision.GetComponentInParent<MonsterAnimation>() != null)
        {
            if (SpawnPoint != null)
            {
                ActiveSpawnPoint = SpawnPoint;
            }

            // Update scale data if enabled on this trigger
            if (ApplyCustomScale)
            {
                ActiveTargetScale = TargetScale;
                collision.transform.root.localScale = TargetScale;
            }

            IsPlayerDead = true;
            GameObject monsterRoot = collision.transform.root.gameObject;
            StartCoroutine(RespawnAfterDelay(monsterRoot));
        }
    }

    private IEnumerator RespawnAfterDelay(GameObject monster)
    {
        _isRespawning = true;
        yield return new WaitForSeconds(RespawnDelay);

        if (ActiveSpawnPoint != null)
        {
            monster.transform.position = ActiveSpawnPoint.position;
        }

        // Apply the active checkpoint's scale on respawn
        monster.transform.localScale = ActiveTargetScale;

        IsPlayerDead = false;
        _isRespawning = false;
    }
}