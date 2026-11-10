using System.Collections.Generic;
using UnityEngine;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    public class CharacterSwitcher : MonoBehaviour
    {
        public static CharacterSwitcher Instance { get; private set; }

        [System.Serializable]
        public class CharacterData
        {
            public GameObject CharacterObject;
            [Tooltip("Is this character unlocked for cycling yet?")]
            public bool IsUnlocked = false;
        }

        [Header("Character Roster")]
        public List<CharacterData> Roster = new List<CharacterData>();

        [Header("Starting Index")]
        public int CurrentIndex = 0;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (Roster.Count > 0 && CurrentIndex < Roster.Count)
            {
                Roster[CurrentIndex].IsUnlocked = true;
            }

            ValidateAndInitialize();
        }

        private void Update()
        {
            if (Roster == null || Roster.Count <= 1) return;

            // Q to cycle forward
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CycleCharacter(1);
            }
            // E to cycle backward
            else if (Input.GetKeyDown(KeyCode.E))
            {
                CycleCharacter(-1);
            }
        }

        private void CycleCharacter(int direction)
        {
            int startIndex = CurrentIndex;
            int nextIndex = CurrentIndex;

            do
            {
                nextIndex = (nextIndex + direction + Roster.Count) % Roster.Count;
                if (nextIndex == startIndex) break;
            } while (!Roster[nextIndex].IsUnlocked);

            if (nextIndex != CurrentIndex && Roster[nextIndex].IsUnlocked)
            {
                SwitchToCharacter(nextIndex);
            }
        }

        public void UnlockAndSwitchToCharacter(GameObject characterObj)
        {
            for (int i = 0; i < Roster.Count; i++)
            {
                if (Roster[i].CharacterObject == characterObj)
                {
                    Roster[i].IsUnlocked = true;
                    SwitchToCharacter(i);
                    break;
                }
            }
        }

        private void SwitchToCharacter(int targetIndex)
        {
            GameObject oldCharacter = Roster[CurrentIndex].CharacterObject;
            if (oldCharacter == null) return;

            // 1. Capture momentum (velocity) and transform from the old character before turning it off
            Vector2 inheritedVelocity = Vector2.zero;
            if (oldCharacter.TryGetComponent<Rigidbody2D>(out var oldRb))
            {
                inheritedVelocity = oldRb.linearVelocity;
            }

            Transform oldTransform = oldCharacter.transform;
            Vector3 position = oldTransform.position;
            Quaternion rotation = oldTransform.rotation;

            // 2. Deactivate current character
            oldCharacter.SetActive(false);

            // 3. Update index
            CurrentIndex = targetIndex;

            // 4. Activate new character, match position, and apply momentum
            GameObject newCharacter = Roster[CurrentIndex].CharacterObject;
            if (newCharacter != null)
            {
                newCharacter.transform.position = position;
                newCharacter.transform.rotation = rotation;
                newCharacter.SetActive(true);

                // Apply velocity to the new character's Rigidbody2D
                if (newCharacter.TryGetComponent<Rigidbody2D>(out var newRb))
                {
                    newRb.linearVelocity = inheritedVelocity;
                }

                Debug.Log($"[CharacterSwitcher] Switched to: {newCharacter.name} with inherited velocity: {inheritedVelocity}");
            }
        }

        private void ValidateAndInitialize()
        {
            for (int i = 0; i < Roster.Count; i++)
            {
                if (Roster[i].CharacterObject != null)
                {
                    bool shouldBeActive = (i == CurrentIndex) && Roster[i].IsUnlocked;
                    Roster[i].CharacterObject.SetActive(shouldBeActive);
                }
            }
        }
    }
}