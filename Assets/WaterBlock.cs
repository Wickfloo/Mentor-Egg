using System.Collections.Generic;
using UnityEngine;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    [RequireComponent(typeof(Collider2D))]
    public class WaterBlock : MonoBehaviour
    {
        [Header("Water Physics")]
        [Range(0.1f, 0.95f)]
        public float MaxDepthRatio = 0.85f; // Increased default depth so it sinks further

        [Header("Submersion & Steering Window")]
        [Tooltip("Minimum time (in seconds) the rat must stay submerged before launching out, giving you time to steer.")]
        [Range(0.1f, 1.0f)]
        public float MinSubmergeTime = 0.2f;

        [Header("Ejection Height Settings")]
        [Tooltip("Percentage of the original entry apex height to conserve (1.0 = 100%, 0.6 = 60%).")]
        [Range(0.5f, 1.5f)]
        public float ApexRetentionMultiplier = 0.95f;

        [Tooltip("Minimum extra height added above the water surface during ejection.")]
        public float MinEjectionHeightBonus = 5f;

        [Header("Ejection Horizontal Settings")]
        [Tooltip("Base horizontal speed when launching out of water.")]
        public float BaseHorizontalSpeed = 6f;

        [Tooltip("How much player input can widen (further) or narrow (shorter) the parabola.")]
        [Range(0f, 0.8f)]
        public float InputInfluenceFactor = 0.4f;

        [Header("Effects & Audio")]
        public ParticleSystem SplashParticles;
        public AudioSource SplashAudio;

        private class WaterState
        {
            public float ApexY;
            public float TimeInWater;
        }

        private readonly Dictionary<Rigidbody2D, WaterState> _waterStates = new Dictionary<Rigidbody2D, WaterState>();

        private void Awake()
        {
            if (TryGetComponent<Collider2D>(out var col))
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var rb = collision.attachedRigidbody;
            if (rb == null) return;

            var rat = collision.GetComponent<MonsterControllerRat>();
            if (rat == null) return;

            TriggerSplashEffects(collision.transform.position);
            rb.gravityScale = 0f;
            rat.IsInWater = true;
            rat.IsEjecting = false;

            _waterStates[rb] = new WaterState
            {
                ApexY = Mathf.Max(rat.ApexY, collision.transform.position.y),
                TimeInWater = 0f
            };
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            var rb = collision.attachedRigidbody;
            if (rb == null || !_waterStates.TryGetValue(rb, out var state)) return;

            var rat = collision.GetComponent<MonsterControllerRat>();
            if (rat == null) return;

            var waterCollider = GetComponent<Collider2D>();
            float topY = waterCollider.bounds.max.y;
            float waterHeight = waterCollider.bounds.size.y;
            float currentDepth = topY - collision.transform.position.y;
            float maxDepth = waterHeight * MaxDepthRatio;

            state.TimeInWater += Time.fixedDeltaTime;
            Vector2 velocity = rb.linearVelocity;
            rat.IsInWater = true;
            rb.gravityScale = 0f;

            // Decelerate smoothly as it sinks into the water depth
            if (currentDepth < maxDepth)
            {
                float stretchProgress = currentDepth / maxDepth;
                velocity.y = Mathf.Lerp(velocity.y, 0f, stretchProgress * 10f * Time.fixedDeltaTime);
                velocity.x = Mathf.MoveTowards(velocity.x, 0f, 10f * Time.fixedDeltaTime);
                rb.linearVelocity = velocity;
            }

            // EJECTION CONDITIONS:
            // Must have spent at least 'MinSubmergeTime' underwater, AND either reached max depth or slowed down.
            bool hasCompletedSubmergeWindow = state.TimeInWater >= MinSubmergeTime;
            bool reachedMaxDepth = currentDepth >= maxDepth;
            bool slowedDown = Mathf.Abs(velocity.y) < 0.5f;

            if (hasCompletedSubmergeWindow && (reachedMaxDepth || slowedDown))
            {
                if (!reachedMaxDepth)
                {
                    Vector3 adjustedPos = collision.transform.position;
                    adjustedPos.y = topY - currentDepth;
                    collision.transform.position = adjustedPos;
                }

                rat.EjectionCount++;

                // Vertical Ejection Calculation
                float targetApexY = Mathf.Max(state.ApexY * ApexRetentionMultiplier, topY + MinEjectionHeightBonus);
                float distanceToTravel = Mathf.Max(targetApexY - collision.transform.position.y, 2f);
                float ejectionY = Mathf.Sqrt(2f * rat.Gravity * distanceToTravel);

                // Horizontal Direction & Parabola Shape Control (Influenced by current input window)
                float facingDir = Mathf.Sign(collision.transform.localScale.x != 0 ? collision.transform.localScale.x : 1f);
                float inputX = rat.Input.x;

                float horizontalScale = 1f + (inputX * facingDir * InputInfluenceFactor);
                horizontalScale = Mathf.Max(0.3f, horizontalScale);
                float targetX = facingDir * BaseHorizontalSpeed * horizontalScale;

                rb.gravityScale = 1f;
                rat.IsInWater = false;
                rat.IsEjecting = true;
                _waterStates.Remove(rb);

                rb.linearVelocity = new Vector2(targetX, ejectionY);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var rb = collision.attachedRigidbody;
            if (rb == null) return;

            var rat = collision.GetComponent<MonsterControllerRat>();
            if (rat != null)
            {
                rb.gravityScale = 1f;
                rat.IsEjecting = false;
                _waterStates.Remove(rb);
                rat.IsInWater = false;
            }
        }

        private void TriggerSplashEffects(Vector3 entryPosition)
        {
            if (SplashParticles != null)
            {
                SplashParticles.transform.position = entryPosition;
                SplashParticles.Play();
            }

            if (SplashAudio != null && !SplashAudio.isPlaying)
            {
                SplashAudio.Play();
            }
        }
    }
}