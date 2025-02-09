using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Utilities;

namespace Entity.Player {

    public class PlayerController : MonoBehaviour {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private bool _isGrounded = false;
        [SerializeField] private bool _canJump = false;
        [SerializeField] private LayerMask _ground;
        [SerializeField] private bool _isJumping = false;

        [SerializeField] private bool _coyoteTimeTrigger = false;

        [SerializeField] private CountDownTimer _coyoteTimer = new CountDownTimer(0.25f);

        private Rigidbody2D _rb2D;
        private SpriteRenderer _renderer;

        void Start() {
            _rb2D = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _coyoteTimer.OnTimerStart += () => _canJump = true;
            _coyoteTimer.OnTimerStop += () => _canJump = false;
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
            if (_coyoteTimer.IsFinished) {
                _coyoteTimer.Stop();
            }
            _canJump = _isGrounded || _coyoteTimer.IsRunning;
            float input = Input.GetAxisRaw("Horizontal");
            if (Input.GetKey(KeyCode.Space) && _canJump && !_isJumping) {
                _rb2D.velocity += Vector2.up * _jumpForce;
                _canJump = false;
                _isJumping = true;
                Debug.Log("Jump");
            }
            if (_isGrounded) {
                _rb2D.velocity = new Vector2(input * _speed, _rb2D.velocity.y);
            } else {
                _rb2D.velocity = new Vector2(input * _speed * 0.5f, _rb2D.velocity.y);
            }
        }

        private bool GetGrounded() {
            return Physics2D.BoxCast(_rb2D.position, _renderer.size, 0f, Vector2.down, 0.04f, _ground);
        }
    }
}