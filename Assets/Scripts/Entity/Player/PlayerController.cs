using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Utilities;

namespace Entity.Player {

    public class PlayerController : MonoBehaviour {

        static class PlayerAnimations {
            public static int Idle = Animator.StringToHash("Player_Idle");
            public static int Walk = Animator.StringToHash("Player_Walk");
            public static int Fall = Animator.StringToHash("Player_Fall");
            public static int Jump = Animator.StringToHash("Player_Jump");
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

        [SerializeField] private bool _isDashing = false;

        [SerializeField] private bool _coyoteTimeTrigger = false;
        float input;
        private int currentAnimation;

        [SerializeField] private CountDownTimer _coyoteTimer = new CountDownTimer(0.25f);
        [SerializeField] private CountDownTimer _dashTimer = new CountDownTimer(0f);

        private Rigidbody2D _rb2D;
        private SpriteRenderer _renderer;
        private Animator _animator;

        void Start() {
            _rb2D = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _coyoteTimer.OnTimerStart += () => _canJump = true;
            _coyoteTimer.OnTimerStop += () => _canJump = false;
            _dashTimer.OnTimerStart += () => { _canDash = false; _isDashing = true; };
            _dashTimer.OnTimerStop += () => { _canDash = true; _isDashing = false; };
        }

        void FixedUpdate() {
            _isGrounded = GetGrounded();
            if (_isGrounded) {
                _coyoteTimeTrigger = true;
                _isJumping = false;
            }
            if (_isGrounded) {
                _coyoteTimer.Stop();
            }
            if (!_isGrounded && _coyoteTimeTrigger) {
                _coyoteTimer.Reset();
                _coyoteTimer.Start();
                _coyoteTimeTrigger = false;
            }
            _coyoteTimer.Update(Time.fixedDeltaTime);
            _dashTimer.Update(Time.fixedDeltaTime);
            _canJump = _isGrounded || _coyoteTimer.IsRunning;
            input = Input.GetAxisRaw("Horizontal");
            if (Input.GetKey(KeyCode.Space) && _canJump && !_isJumping) {
                _rb2D.velocity += Vector2.up * _jumpForce;
                _canJump = false;
                _isJumping = true;
            }
            if (Input.GetKey(KeyCode.LeftShift) && _canDash && input != 0.0f) {
                _dashTimer.Reset(_dashTime);
                _rb2D.velocity = Vector2.right * input * _dashForce;
                _isJumping = false;
            }
            if (!_isDashing) {
                if (_isGrounded) {
                    _rb2D.velocity = new Vector2(input * _speed, _rb2D.velocity.y);
                } else {
                    _rb2D.velocity = new Vector2(input * _speed, _rb2D.velocity.y);
                }
                if (_rb2D.velocity.y < 0 && !_isGrounded) {
                    _rb2D.gravityScale = 2f;
                } else {
                    _rb2D.gravityScale = 1f;
                }
            } else {
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, _rb2D.velocity.y);
                _rb2D.gravityScale = 0.25f;
            }


            UpdateAnimations();
        }

        private bool GetGrounded() {
            RaycastHit2D hit = Physics2D.BoxCast(_rb2D.position, _renderer.size, 0f, Vector2.down, 0.04f, _ground);
            if (hit) {
                return hit.normal.y >= 0.75f;
            } else {
                return false;
            }
        }


        private void UpdateAnimations() {
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
    }
}