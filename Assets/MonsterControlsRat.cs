using Assets.PixelFantasy.Common.Scripts;
using UnityEngine;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    [RequireComponent(typeof(Monster))]
    [RequireComponent(typeof(MonsterControllerRat))]
    [RequireComponent(typeof(MonsterAnimation))]
    public class MonsterControlsRat : MonoBehaviour
    {
        private Monster _monster;
        private MonsterControllerRat _controller;
        private MonsterAnimation _animation;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;

        [Header("Key Bindings Configuration")]
        [Tooltip("Assign your KeyBinder component here, or leave blank to find it automatically.")]
        [SerializeField] private KeyBinder keyConfig;

        [Header("Death Visual Effects")]
        public float FlashSpeed = 15f;
        public Color FlashColor = Color.red;

        private Color _originalColor;
        private bool _wasDeadLastFrame = false;

        public void Start()
        {
            _monster = GetComponent<Monster>();
            _controller = GetComponent<MonsterControllerRat>();
            _animation = GetComponent<MonsterAnimation>();
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }

            if (keyConfig == null)
            {
                keyConfig = FindObjectOfType<KeyBinder>();
            }
        }

        public void Update()
        {
            if (keyConfig == null) return;

            // Manual Reset: Pressing configured Respawn key instantly teleports to checkpoint
            if (Input.GetKeyDown(keyConfig.respawnR))
            {
                if (DeathTrigger.ActiveSpawnPoint != null)
                {
                    transform.position = DeathTrigger.ActiveSpawnPoint.position;
                    DeathTrigger.IsPlayerDead = false;
                    ResetAfterRespawn();
                    return;
                }
            }

            // Detect automatic transition from Dead to Alive
            if (_wasDeadLastFrame && !DeathTrigger.IsPlayerDead)
            {
                ResetAfterRespawn();
            }

            if (DeathTrigger.IsPlayerDead)
            {
                _wasDeadLastFrame = true;
                _animation.SetState(MonsterState.Die);
                if (_controller != null) _controller.Input = Vector2.zero;

                if (_rb != null)
                {
                    _rb.linearVelocity = Vector2.zero;
                    _rb.angularVelocity = 0f;
                    _rb.isKinematic = true;
                }

                if (_spriteRenderer != null)
                {
                    float lerp = Mathf.PingPong(Time.time * FlashSpeed, 1f);
                    _spriteRenderer.color = Color.Lerp(_originalColor, FlashColor, lerp);
                }

                return;
            }

            Move();
            Attack();
        }

        private void ResetAfterRespawn()
        {
            _wasDeadLastFrame = false;

            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0f;
            }
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor;
            }
            _animation.SetState(MonsterState.Idle);
        }

        private void Move()
        {
            Vector2 rawInput = Vector2.zero;

            if (Input.GetKey(keyConfig.moveLeft)) { rawInput.x = -1; }
            else if (Input.GetKey(keyConfig.moveRight)) { rawInput.x = 1; }

            if (Input.GetKey(keyConfig.moveUp)) { rawInput.y = 1; }
            else if (Input.GetKey(keyConfig.moveDown)) { rawInput.y = -1; }

            _controller.Input = rawInput;
        }

        private void Attack()
        {
            if (Input.GetKeyDown(keyConfig.attackA)) { _animation.Attack(); }
            if (Input.GetKeyDown(keyConfig.fireF)) { _animation.Fire(); }
        }
    }
}