using UnityEngine;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    [RequireComponent(typeof(Collider2D))]
    public class CharacterUnlockTrigger : MonoBehaviour
    {
        [Header("Unlock Settings")]
        [Tooltip("Drag the character object (e.g. Spider) that this trigger unlocks.")]
        public GameObject CharacterToUnlock;

        private void Awake()
        {
            if (TryGetComponent<Collider2D>(out var col))
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (CharacterSwitcher.Instance != null && CharacterToUnlock != null)
                {
                    // Unlock the character in the switcher and swap to them
                    CharacterSwitcher.Instance.UnlockAndSwitchToCharacter(CharacterToUnlock);
                }

                // Permanently remove/disable this trigger's collider so it can never be hit again
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}