using UnityEngine;
using System.Collections;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    [RequireComponent(typeof(Collider2D))]
    public class DirectionalCameraTrigger : MonoBehaviour
    {
        [Header("Camera Point Arrays")]
        [Tooltip("Horizontal room points (X-axis tracking)")]
        public Transform[] Array1;

        [Tooltip("Vertical section points (Y-axis tracking)")]
        public Transform[] Array2;

        public enum ArrayChoice { Array1, Array2 }
        public enum StepDirection { Forward = 1, Backward = -1 }

        [Header("Trigger Configuration")]
        public ArrayChoice TargetArray = ArrayChoice.Array1;
        public StepDirection Step = StepDirection.Forward;

        [Header("Shared Global Indices & Timers")]
        public static int Array1Index = 0;
        public static int Array2Index = 0;
        private static float _globalNextTriggerTime = 0f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Array1Index = 0;
            Array2Index = 0;
            _globalNextTriggerTime = 0f;
        }

        [Header("Global Cooldown Settings")]
        public float CooldownDuration = 0.25f;

        [Header("Smooth Transition")]
        public bool SmoothTransition = false;
        public float SmoothSpeed = 5f;

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
            if (!collision.CompareTag("Player")) return;

            if (Time.time < _globalNextTriggerTime)
            {
                if (ShowDebugLogs) Debug.Log($"[CameraTrigger] Blocked by global cooldown.");
                return;
            }

            if (Camera.main == null)
            {
                Debug.LogWarning("[CameraTrigger] ERROR: Camera.main is NULL!");
                return;
            }

            _globalNextTriggerTime = Time.time + CooldownDuration;

            Transform[] activeArray = (TargetArray == ArrayChoice.Array1) ? Array1 : Array2;
            string arrayName = (TargetArray == ArrayChoice.Array1) ? "Array 1" : "Array 2";

            if (activeArray == null || activeArray.Length == 0)
            {
                Debug.LogWarning($"[CameraTrigger] {arrayName} is empty or unassigned!");
                return;
            }

            Vector3 targetPosition = Vector3.zero;
            Transform targetPoint = null;
            int targetIndex = 0;

            if (TargetArray == ArrayChoice.Array1)
            {
                // --- ARRAY 1 (HORIZONTAL) LOGIC ---
                int currentIndex = Mathf.Clamp(Array1Index, 0, Array1.Length - 1);

                targetIndex = (Step == StepDirection.Forward) ? currentIndex + 1 : currentIndex - 1;

                // Safe clamping instead of wrapping to prevent snapping to Room 4
                targetIndex = Mathf.Clamp(targetIndex, 0, Array1.Length - 1);

                Array1Index = targetIndex;
                targetPoint = Array1[targetIndex];

                if (targetPoint == null)
                {
                    Debug.LogWarning($"[CameraTrigger] Point at index [{targetIndex}] in Array 1 is null!");
                    return;
                }

                // HYBRID POSITIONING: Grab X from Array 1, but retain Y from the active Array 2 floor
                float currentVerticalY = (Array2 != null && Array2.Length > 0)
                    ? Array2[Mathf.Clamp(Array2Index, 0, Array2.Length - 1)].position.y
                    : Camera.main.transform.position.y;

                targetPosition = new Vector3(targetPoint.position.x, currentVerticalY, Camera.main.transform.position.z);
            }
            else
            {
                // --- ARRAY 2 (VERTICAL) LOGIC ---
                int currentIndex = Mathf.Clamp(Array2Index, 0, Array2.Length - 1);

                targetIndex = (Step == StepDirection.Forward) ? currentIndex + 1 : currentIndex - 1;

                // Safe clamping
                targetIndex = Mathf.Clamp(targetIndex, 0, Array2.Length - 1);

                Array2Index = targetIndex;
                targetPoint = Array2[targetIndex];

                if (targetPoint == null)
                {
                    Debug.LogWarning($"[CameraTrigger] Point at index [{targetIndex}] in Array 2 is null!");
                    return;
                }

                // HYBRID POSITIONING: Grab X from Array 1 room, and Y from Array 2 target point
                float currentHorizontalX = (Array1 != null && Array1.Length > 0)
                    ? Array1[Mathf.Clamp(Array1Index, 0, Array1.Length - 1)].position.x
                    : Camera.main.transform.position.x;

                targetPosition = new Vector3(currentHorizontalX, targetPoint.position.y, Camera.main.transform.position.z);
            }

            targetPosition.z = Camera.main.transform.position.z;

            if (ShowDebugLogs)
                Debug.Log($"[CameraTrigger] Hit {arrayName} | Step: {Step} | Moved to Index: [{targetIndex}] | Target Point: {targetPoint.name}");

            if (SmoothTransition)
            {
                StopAllCoroutines();
                StartCoroutine(PanCamera(targetPosition));
            }
            else
            {
                Camera.main.transform.position = targetPosition;
            }
        }

        private IEnumerator PanCamera(Vector3 targetPosition)
        {
            while (Camera.main != null && Vector3.Distance(Camera.main.transform.position, targetPosition) > 0.01f)
            {
                Camera.main.transform.position = Vector3.Lerp(
                    Camera.main.transform.position,
                    targetPosition,
                    SmoothSpeed * Time.deltaTime
                );
                yield return null;
            }
            if (Camera.main != null)
            {
                Camera.main.transform.position = targetPosition;
            }
        }
    }
}