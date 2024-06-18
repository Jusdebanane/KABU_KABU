using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Movement : MonoBehaviour
{
    private P_Data _data;
    private Rigidbody2D _body;
    private void Awake()
    {
        _data = GetComponent<P_Data>();
        _body = GetComponent<Rigidbody2D>();
    }
    #region Collision_check
    [Header(" Collision checks")]
    [SerializeField] private Transform ground_check;
    [SerializeField] private LayerMask ground_mask;
    private bool grounded() { return Physics2D.OverlapCircle(ground_check.position, 0.2f, ground_mask); }
    #endregion
    #region Input
    private Vector2 _direction;
    public void input_direction(InputAction.CallbackContext _move) { _direction = _move.ReadValue<Vector2>(); }
    private bool gliding;
    public void is_gliding(InputAction.CallbackContext _glide) { if (_data.can_glide && !grounded()) { gliding = _glide.performed; } }
    #endregion
    #region Run
    public Vector2 _velocity;
    private void Horizontal_Movement()
    {
        if(_direction.x == 0)
        {
            var deceleration = grounded() ? _data.run_deceleration : _data.air_deceleration;
            _velocity.x = Mathf.MoveTowards(_body.velocity.x, 0, deceleration * Time.fixedDeltaTime); 
        }
        else
        {
            _velocity.x = Mathf.MoveTowards(_body.velocity.x, _direction.x * _data.max_speed, _data.acceleration * Time.fixedDeltaTime);
        }
        _body.velocity = new Vector2(_velocity.x, _body.velocity.y);
    }
    #endregion
    #region Jump
    private int _kabu;
    public void Jump(InputAction.CallbackContext _jump)
    {
        if (_jump.performed && grounded() || _jump.performed && _kabu > 0) 
        {
            if (!grounded()) { _kabu--; }
            _body.velocity = new Vector2(_body.velocity.x, 0);
            _body.AddForce(Vector2.up * _data.jump_force, ForceMode2D.Impulse); 
        }
        if(_body.velocity.y > 0 && _jump.canceled) { _body.gravityScale = _data.base_gravity * _data.low_jump_gravity_multiplier; }
    }
    #endregion
    #region Fall
    private void Fall()
    {
        var max_fall_speed = gliding? _data.glide_max_fall_speed : _data.max_fall_speed;
        if(_body.velocity.y <= 0) { _body.gravityScale = _data.base_gravity * _data.fall_gravity_multiplier; }
        _body.velocity = new Vector2(_body.velocity.x, Mathf.Max(_body.velocity.y, -max_fall_speed));
    }
    #endregion
    private void FixedUpdate()
    {
        Horizontal_Movement();
        Fall();
        if(grounded()) { _kabu = _data.max_kabu; }
    }
}
