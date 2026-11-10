using UnityEngine;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    public class GameInitializer : MonoBehaviour
    {
        [Tooltip("Drag your Room1Spawn transform here")]
        public Transform Room1Spawn;

        private void Awake()
        {
            if (Room1Spawn != null)
            {
                // Set the default active spawn point to Room 1 at game start
                DeathTrigger.ActiveSpawnPoint = Room1Spawn;
            }

            // Ensure camera indices start at Room 1 / Story 1
            DirectionalCameraTrigger.Array1Index = 0;
            DirectionalCameraTrigger.Array2Index = 0;
        }
    }
}