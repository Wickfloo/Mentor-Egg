using System.Linq;
using UnityEngine;

namespace Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(MonsterAnimation))]
    public class MonsterControllerSpider : MonoBehaviour
    {
        public Vector2 Input;
        public bool IsGrounded;
        public bool IsInWater; // Allows WaterBlock to take control of physics

        public float MaxSpeed = 8;
        public float JumpForce = 850;
        public float Gravity = 70;

        [Header("Surface Detection")]
        public LayerMask GroundLayer;
        public float CheckDistance = 0.5f;

        private Collider2D _collider;
        private Rigidbody2D _rigidbody;
        private MonsterAnimation _animation;

        private bool _jump;
        private Vector2 _surfaceNormal = Vector2.up;
        private Collider2D _ground;

        public void Start()
        {
            _collider = GetComponent<Collider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animation = GetComponent<MonsterAnimation>();
            _rigidbody.gravityScale = 1;
        }

        public void FixedUpdate()
        {
            var state = _animation.GetState();
            if (state == MonsterState.Die) return;

            // Yield physics control to WaterBlock while submerged or floating
            if (IsInWater) return;

            var velocity = _rigidbody.linearVelocity;
            Vector2 surfaceTangent = new Vector2(_surfaceNormal.y, -_surfaceNormal.x);

            if (IsGrounded)
            {
                if (Input.x == 0)
                {
                    velocity = Vector2.zero;
                    _animation.Ready();
                }
                else
                {
                    velocity = surfaceTangent * (Mathf.Sign(Input.x) * MaxSpeed);
                    _animation.Run();
                    Turn(Input.x);
                }

                if (Input.y > 0 && !_jump)
                {
                    _jump = true;
                    IsGrounded = false;
                    _rigidbody.linearVelocity = Vector2.zero;

                    float scaleFactor = Mathf.Abs(transform.localScale.x);
                    bool isNaturalFloor = _surfaceNormal.y > 0.9f;
                    float horizontalBoost;
                    float verticalComponent;
                    float appliedJumpForce = JumpForce;

                    if (isNaturalFloor)
                    {
                        horizontalBoost = Input.x != 0 ? Mathf.Sign(Input.x) * 1.0f : 0f;
                        verticalComponent = 1.0f;
                    }
                    else
                    {
                        horizontalBoost = Input.x != 0 ? Mathf.Sign(Input.x) * 2.5f : 0f;
                        verticalComponent = 1.2f;
                        appliedJumpForce *= 1.3f;
                    }

                    Vector2 jumpDirection = (_surfaceNormal + new Vector2(horizontalBoost, verticalComponent)).normalized;
                    _rigidbody.AddForce(jumpDirection * (appliedJumpForce * scaleFactor));
                    _animation.Jump();
                }

                _rigidbody.AddForce(-_surfaceNormal * 150f);
            }
            else
            {
                _surfaceNormal = Vector2.Lerp(_surfaceNormal, Vector2.up, Time.fixedDeltaTime * 10f);

                velocity.y -= Gravity * Time.fixedDeltaTime;

                if (velocity.y < 0)
                {
                    _jump = true;
                    _animation.Fall();
                }

                if (Input.x != 0)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, Input.x * MaxSpeed, 40f * Time.fixedDeltaTime);
                    Turn(velocity.x);
                }
            }

            float targetAngle = Mathf.Atan2(_surfaceNormal.y, _surfaceNormal.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 25f);

            _rigidbody.linearVelocity = velocity;
        }

        private void Turn(float direction)
        {
            if (Mathf.Abs(direction) > 0.01f)
            {
                var scale = transform.localScale;
                scale.x = -Mathf.Sign(direction) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }

        public void OnCollisionEnter2D(Collision2D collision) => EvaluateClosestContact(collision);
        public void OnCollisionStay2D(Collision2D collision) => EvaluateClosestContact(collision);

        private void EvaluateClosestContact(Collision2D collision)
        {
            if ((GroundLayer.value & (1 << collision.collider.gameObject.layer)) > 0)
            {
                Vector2 closestNormal = _surfaceNormal;
                float minDistance = float.MaxValue;
                bool foundValidContact = false;

                foreach (var contact in collision.contacts)
                {
                    if (contact.normal.y > 0.9f || Mathf.Abs(contact.normal.y) < 0.1f)
                    {
                        float distance = Vector2.Distance(transform.position, contact.point);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestNormal = contact.normal;
                            foundValidContact = true;
                        }
                    }
                }

                if (foundValidContact)
                {
                    IsGrounded = true;
                    _ground = collision.collider;
                    _surfaceNormal = closestNormal;

                    if (_jump)
                    {
                        _jump = false;
                        _animation.Land();
                    }
                }
            }
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if (IsGrounded && collision.collider == _ground)
            {
                IsGrounded = false;
                _ground = null;
            }
        }
    }
}