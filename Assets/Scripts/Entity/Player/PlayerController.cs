using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

using LevelEditor;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;

using Utilities;

namespace Entity.Player {

    public class PlayerController : MonoBehaviour {

        static class PlayerAnimations {
            public static int Idle = Animator.StringToHash("Player_Idle");
            public static int Walk = Animator.StringToHash("Player_Walk");
            public static int Fall = Animator.StringToHash("Player_Fall");
            public static int Jump = Animator.StringToHash("Player_Jump");
            public static int Dash = Animator.StringToHash("Player_Dash");
            public static int Death = Animator.StringToHash("Player_Death");
        }

        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _jumpForce = 7.5f;
        [SerializeField] private float _dashForce = 10f;
        [SerializeField] private float _dashTime = 0.5f;
        [SerializeField] private bool _isGrounded = false;
        [SerializeField] private bool _canJump = false;
        [SerializeField] private bool _canDash = true;
        [SerializeField] private LayerMask _ground;
        [SerializeField] private bool _isJumping = false;
        [SerializeField] private bool _isPlaying = false;


        //Gravity 
        [SerializeField] private float originalGravity;
        private float fallGravity;
        [SerializeField] private float maxFallGravity;
        [SerializeField] private float fallGravityIncrement;

        //Coyote time
        [SerializeField] private float timeSinceLeftGround = 0.0f;
        [SerializeField] private float coyoteTimeDuration = 0.2f;

        [SerializeField] private bool _isDashing = false;
        [SerializeField] private GameObject dashEffect;
        private GameObject effectObject;

        [SerializeField] private bool _coyoteTimeTrigger = false;

        private bool jumpPressed = false;
        private bool jumpReleased = false;

        float input;
        private int currentAnimation;
        float _deathAnimationLength;
        public float DeathTime => _deathAnimationLength;

        [SerializeField] private CountDownTimer _dashTimer = new CountDownTimer(0f);

        private Rigidbody2D _rb2D;
        private SpriteRenderer _renderer;
        private Animator _animator;
        private BoxCollider2D _collider;

        [Header("Resume previous state")]
        private Vector2 _fallbackPosition;
        private Vector2 _previousVelocity = Vector2.zero;
        private float _previousGravity = 1.0f;

        public Action OnDeath;
        [SerializeField] private bool isDead;

        void Start() {
            _fallbackPosition = transform.position;
            _rb2D = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _collider = GetComponent<BoxCollider2D>();
            _dashTimer.OnTimerStart += () => { _canDash = false; _isDashing = true; };
            _dashTimer.OnTimerStop += () => { _canDash = true; _isDashing = false; };
            _deathAnimationLength = _animator.GetRuntimeClip(PlayerAnimations.Death).length;
            originalGravity = _rb2D.gravityScale;
            fallGravity = _rb2D.gravityScale * 2f;
            OnPlay(PlayState.Exit);
        }

        void Update() {
            if (!_isPlaying) { return; }
            if (_canJump && !_isJumping) {
                if (Input.GetButtonDown("Jump")) {
                    jumpPressed = true;
                }
                if (Input.GetButtonUp("Jump")) {
                    jumpReleased = true;
                }
            }
        }

        void FixedUpdate() {
            if (isDead) { return; }
            if (!_isPlaying) { return; }
            _isGrounded = GetGrounded();


            if (_isGrounded) {
                _isJumping = false;
            }
        
            _dashTimer.Update(Time.fixedDeltaTime);
 
            input = Input.GetAxisRaw("Horizontal");

            _canJump = _isGrounded || _coyoteTimeTrigger;
            if (_canJump && !_isJumping) {
                if (jumpPressed) {
                    _rb2D.velocity = new Vector2(_rb2D.velocity.x, _jumpForce);
                    jumpPressed = false;
                    timeSinceLeftGround = coyoteTimeDuration + 1;
                }
                if (jumpReleased && _rb2D.velocity.y > 0) {
                    _rb2D.velocity = new Vector2(_rb2D.velocity.x, _rb2D.velocity.y / 2);
                    _isJumping = true;
                    jumpReleased = false;
                }
            }

            if (Input.GetKey(KeyCode.LeftShift) && _canDash && input != 0.0f) {
                _dashTimer.Reset(_dashTime);
                 effectObject = Instantiate(dashEffect, this.transform);
                _rb2D.velocity = Vector2.right * input * _dashForce;
                _isJumping = false;
            }
            if (!_isDashing) {
                if (_isGrounded) {
                    _rb2D.velocity = new Vector2(input * _speed, _rb2D.velocity.y);
                } else {
                    _rb2D.velocity = new Vector2(input * _speed, _rb2D.velocity.y);
                }
                UpdateGravity();
            } else {
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, _rb2D.velocity.y);
                _rb2D.gravityScale = 0.25f;
            }

            UpdateCoyoteTime();
            UpdateAnimations();
        }

        private void UpdateCoyoteTime() {
            if (!_isGrounded) {
                timeSinceLeftGround += Time.deltaTime;

                if (timeSinceLeftGround <= coyoteTimeDuration) {
                    _coyoteTimeTrigger = true;
                } else {
                    _coyoteTimeTrigger = false;
                }
            } else {
                timeSinceLeftGround = 0.0f;
                _coyoteTimeTrigger = false;
            }
        }

        private bool GetGrounded() {
            return Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, Vector2.down, .4f, _ground);
        }

        private void UpdateGravity() {
            if (_isGrounded) {
                _rb2D.gravityScale = originalGravity;
                fallGravity = originalGravity * 2f;
            }
            if (_rb2D.velocity.y > 0.1f) {
                _rb2D.gravityScale = originalGravity;

            } else if (_rb2D.velocity.y < -0.05f) {
                _rb2D.gravityScale = fallGravity;

                if (fallGravity < maxFallGravity) {
                    fallGravity += fallGravityIncrement;
                }
            }
        }

        private void UpdateAnimations() {
            if (currentAnimation == PlayerAnimations.Death) { return; }

            if (_isDashing) {
                PlayAnimation(PlayerAnimations.Dash);
                _renderer.flipX = _rb2D.velocity.x > 0;
                if (_renderer.flipX) {
                    effectObject.transform.localScale = new Vector3(-1, 1, 1);
                }
                return;
            }

            if (input < 0f) {
                _renderer.flipX = false;
            } else if (input > 0f) {
                _renderer.flipX = true;
            }

            if (_rb2D.velocity.y > 0.1f) {

                PlayAnimation(PlayerAnimations.Jump);
            } else if (_rb2D.velocity.y < -0.1f) {
                PlayAnimation(PlayerAnimations.Fall);
            } else if (_rb2D.velocity.x > 0.1f || _rb2D.velocity.x < -0.1f) {
                PlayAnimation(PlayerAnimations.Walk);
            } else {
                PlayAnimation(PlayerAnimations.Idle);
            }
        }

        private void PlayAnimation(int animation) {
            if (currentAnimation == animation) { return; }
            currentAnimation = animation;
            _animator.Play(animation);
        }

        public void OnPlay(PlayState state) {
            switch (state) {
                case PlayState.Begin:
                    PlayerReset();
                    break;

                case PlayState.Continue:
                    _isPlaying = true;
                    _rb2D.gravityScale = _previousGravity;
                    _rb2D.velocity = _previousVelocity;
                    break;

                case PlayState.Exit:
                    _isPlaying = false;
                    _previousGravity = _rb2D.gravityScale;
                    _previousVelocity = _rb2D.velocity;
                    break;
            }
        }

        public void Death() {
            PlayAnimation(PlayerAnimations.Death);
            isDead = true;
            OnDeath?.Invoke();
        }

        public void PlayerReset() {
            _isPlaying = true;
            isDead = false;
            PlayAnimation(PlayerAnimations.Idle);
            transform.position = FindFirstObjectByType<SpawnPoint>().OrNull()?.transform.position ?? _fallbackPosition;
            _rb2D.velocity = Vector2.zero;
            _rb2D.gravityScale = 1.0f;
        }


        //private void OnCollisionEnter2D(Collision2D collision) {
        //    print("collision");
        //    if (collision.gameObject.layer == 9) {
        //        if (_isDashing) {
        //            print("enemy death");
        //        } else {
        //            OnDeath();
        //        }
        //    }

        //}

        public bool IsVulnerable() {
            return (!_isDashing);
        }

    }
}