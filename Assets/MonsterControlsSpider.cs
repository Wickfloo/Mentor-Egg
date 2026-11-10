using Assets.PixelFantasy.Common.Scripts;
using UnityEngine;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    [RequireComponent(typeof(Monster))]
    [RequireComponent(typeof(MonsterControllerSpider))]
    [RequireComponent(typeof(MonsterAnimation))]
    public class MonsterControlsSpider : MonoBehaviour
    {
        private Monster _monster;
        private MonsterControllerSpider _controller;
        private MonsterAnimation _animation;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;

        [Header("Death Visual Effects")]
        public float FlashSpeed = 15f;
        public Color FlashColor = Color.red;

        private Color _originalColor;
        private bool _wasDeadLastFrame = false;

        public void Start()
        {
            _monster = GetComponent<Monster>();
            _controller = GetComponent<MonsterControllerSpider>();
            _animation = GetComponent<MonsterAnimation>();
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }
        }

        public void Update()
        {
            // Manual Reset: Pressing R instantly teleports to the last active checkpoint
            if (Input.GetKeyDown(KeyCode.R))
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

            if (Input.GetKeyDown(KeyCode.I)) { _animation.SetState(MonsterState.Idle); }
            if (Input.GetKeyDown(KeyCode.D)) { _animation.SetState(MonsterState.Die); }
            if (Input.GetKeyUp(KeyCode.H)) { _animation.Hit(); }
            if (Input.GetKeyUp(KeyCode.L)) { EffectManager.Instance.Blink(_monster); }
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

            if (Input.GetKey(KeyCode.LeftArrow)) { rawInput.x = -1; }
            else if (Input.GetKey(KeyCode.RightArrow)) { rawInput.x = 1; }

            if (Input.GetKey(KeyCode.UpArrow)) { rawInput.y = 1; }
            else if (Input.GetKey(KeyCode.DownArrow)) { rawInput.y = -1; }

            _controller.Input = rawInput;
        }

        private void Attack()
        {
            if (Input.GetKeyDown(KeyCode.A)) { _animation.Attack(); }
            if (Input.GetKeyDown(KeyCode.F)) { _animation.Fire(); }
        }
    }
}