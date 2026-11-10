using UnityEngine;
using System.Collections;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    [RequireComponent(typeof(Collider2D))]
    public class WaterDeathTrigger : MonoBehaviour
    {
        [Header("Respawn Settings")]
        [Tooltip("Optional: Drag a specific Checkpoint/Spawn Transform here. If left empty, it will use the globally saved ActiveSpawnPoint.")]
        public Transform SpawnPoint;

        [Tooltip("Time to wait while the death animation/flash plays before respawning.")]
        public float RespawnDelay = 1.5f;

        [Header("Scale Settings")]
        [Tooltip("Should entering this water as a non-rat change the monster's scale upon respawn?")]
        public bool ApplyCustomScale = false;

        [Tooltip("The target scale to apply to the monster when respawning.")]
        public Vector3 TargetScale = new Vector3(1f, 1f, 1f);

        [Header("Debugging")]
        public bool ShowDebugLogs = true;

        private static bool _isRespawning = false;

        private void Awake()
        {
            if (TryGetComponent<Collider2D>(out var col))
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isRespawning) return;

            // Ensure we are checking the main character root object
            if (collision.GetComponent<MonsterAnimation>() == null &&
                collision.GetComponentInParent<MonsterAnimation>() == null &&
                !collision.name.Contains("Rat") && !collision.name.Contains("Spider"))
            {
                return;
            }

            // Check if the player currently has the MonsterControllerRat component attached
            MonsterControllerRat ratController = collision.GetComponentInParent<MonsterControllerRat>();
            bool isRatActive = (ratController != null);

            if (!isRatActive)
            {
                if (ShowDebugLogs)
                    Debug.Log("[WaterDeathTrigger] Player is NOT the rat. Triggering death and respawn sequence.");

                // Update global active spawn point if a specific one was assigned here
                if (SpawnPoint != null)
                {
                    DeathTrigger.ActiveSpawnPoint = SpawnPoint;
                }

                // Update scale data if enabled on this water hazard
                if (ApplyCustomScale)
                {
                    DeathTrigger.ActiveTargetScale = TargetScale;
                    collision.transform.root.localScale = TargetScale;
                }

                DeathTrigger.IsPlayerDead = true;
                GameObject monsterRoot = collision.transform.root.gameObject;
                StartCoroutine(RespawnAfterDelay(monsterRoot));
            }
            else
            {
                if (ShowDebugLogs)
                    Debug.Log("[WaterDeathTrigger] Player IS the rat. Safe to swim!");
            }
        }

        private IEnumerator RespawnAfterDelay(GameObject monster)
        {
            _isRespawning = true;
            yield return new WaitForSeconds(RespawnDelay);

            if (DeathTrigger.ActiveSpawnPoint != null)
            {
                monster.transform.position = DeathTrigger.ActiveSpawnPoint.position;
            }
            else
            {
                Debug.LogWarning("[WaterDeathTrigger] Warning: DeathTrigger.ActiveSpawnPoint is NULL!");
            }

            // Apply the active checkpoint or hazard's scale on respawn
            monster.transform.localScale = DeathTrigger.ActiveTargetScale;

            DeathTrigger.IsPlayerDead = false;
            _isRespawning = false;
        }
    }
}