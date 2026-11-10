using UnityEngine;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    [RequireComponent(typeof(Collider2D))]
    public class ObjectSwapTrigger : MonoBehaviour
    {
        [Header("Swap Settings")]
        [Tooltip("Drag the existing Spider object from your Hierarchy into this slot.")]
        public GameObject TargetObjectInScene;

        [Tooltip("Only trigger if the colliding object has this tag (e.g., 'Player').")]
        public string RequiredTag = "Player";

        [Header("One-Time Use")]
        [Tooltip("If true, this trigger will disable itself after firing once.")]
        public bool TriggerOnce = true;

        [Header("Debugging")]
        public bool ShowDebugLogs = true;

        private void Awake()
        {
            if (TryGetComponent<Collider2D>(out var col))
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check tag
            if (!string.IsNullOrEmpty(RequiredTag) && !collision.CompareTag(RequiredTag))
            {
                return;
            }

            if (TargetObjectInScene != null)
            {
                // 1. Move the spider to the rat's exact position and rotation
                TargetObjectInScene.transform.position = collision.transform.position;
                TargetObjectInScene.transform.rotation = collision.transform.rotation;

                // 2. Turn the spider ON (active)
                TargetObjectInScene.SetActive(true);

                if (ShowDebugLogs) Debug.Log($"[SceneObjectSwapTrigger] Swapped rat for '{TargetObjectInScene.name}' at position {collision.transform.position}.", this);
            }
            else
            {
                Debug.LogWarning("[SceneObjectSwapTrigger] 'TargetObjectInScene' slot is empty!", this);
            }

            // 3. Turn the rat OFF (inactive)
            collision.gameObject.SetActive(false);

            // 4. Disable this trigger so it can never fire again
            if (TriggerOnce)
            {
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}