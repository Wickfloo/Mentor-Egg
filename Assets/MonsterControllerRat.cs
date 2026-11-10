using System.Linq;
using UnityEngine;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(MonsterAnimation))]
    public class MonsterControllerRat : MonoBehaviour
    {
        public Vector2 Input;
        public bool IsGrounded;
        public bool IsInWater;
        public bool IsStandingOnWater;
        public bool IsEjecting;
        public int EjectionCount = 0; // Tracks consecutive ejections without touching ground
        public bool HasStalled = false; // Tracks if a shallow/stalled jump happened in this air combo

        public float Acceleration = 40;
        public float MaxSpeed = 8;
        public float JumpForce = 1000;
        public float Gravity = 70;

        public float ApexY;

        private Collider2D _collider;
        private Rigidbody2D _rigidbody;
        private MonsterAnimation _animation;

        private bool _jump;

        public void Start()
        {
            _collider = GetComponent<Collider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animation = GetComponent<MonsterAnimation>();
            ApexY = transform.position.y;
        }

        public void FixedUpdate()
        {
            var state = _animation.GetState();
            if (state == MonsterState.Die) return;

            if (IsInWater && !IsStandingOnWater) return;

            var velocity = _rigidbody.linearVelocity;

            if (IsGrounded || IsStandingOnWater)
            {
                ApexY = transform.position.y;
                EjectionCount = 0;
                HasStalled = false; // Reset stall state when grounded
                _rigidbody.gravityScale = 0f;
                _jump = false;
                IsEjecting = false;

                if (Input.x == 0)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, 0, Acceleration * 3 * Time.fixedDeltaTime);
                    _animation.Ready();
                }
                else
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, Input.x * MaxSpeed, Acceleration * Time.fixedDeltaTime);
                    _animation.Run();
                    Turn(velocity.x);
                }

                if (Input.y > 0 && !_jump)
                {
                    _jump = true;
                    IsGrounded = false;
                    IsStandingOnWater = false;
                    IsInWater = false;
                    IsEjecting = false;
                    _rigidbody.gravityScale = 1f;

                    float scaleFactor = Mathf.Abs(transform.localScale.x);
                    _rigidbody.AddForce(Vector2.up * (JumpForce * scaleFactor));
                    _animation.Jump();
                }
            }
            else
            {
                ApexY = Mathf.Max(ApexY, transform.position.y);

                _rigidbody.gravityScale = 1f;

                if (IsEjecting)
                {
                    IsEjecting = false;
                }
                else
                {
                    velocity.y -= Gravity * Time.fixedDeltaTime;
                }

                if (velocity.y < 0)
                {
                    _jump = true;
                    _animation.Fall();
                }

                if (Input.x != 0)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, Input.x * MaxSpeed, Acceleration * Time.fixedDeltaTime);
                    Turn(velocity.x);
                }
            }

            _rigidbody.linearVelocity = velocity;
        }

        private void Turn(float direction)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Sign(direction) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        private Collider2D _ground;

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.contacts.All(i => i.point.y <= _collider.bounds.min.y + 0.1f))
            {
                IsGrounded = true;
                _ground = collision.collider;
                EjectionCount = 0;
                HasStalled = false; // Reset stall state when grounded

                if (_jump)
                {
                    _jump = false;
                    _animation.Land();
                }
            }
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if (IsGrounded && collision.collider == _ground)
            {
                IsGrounded = false;
            }
        }
    }
}