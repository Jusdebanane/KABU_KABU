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
        _stamina = _data.max_stamina;
    }
    #region Collision_check
    [Header(" Collision checks")]
    [SerializeField] private Transform ground_check;
    [SerializeField] private Transform wall_check;
    [SerializeField] private LayerMask solid_mask;
    private bool grounded() { return Physics2D.OverlapCircle(ground_check.position, 0.2f, solid_mask); }
    private bool walled() { return Physics2D.OverlapCircle(wall_check.position, 0.2f, solid_mask) && !grounded(); }
    #endregion

    #region Ressources
    private int _kabu;
    [SerializeField] private float _stamina;

    private void Ressources()
    {
        if (grounded())
        {
            _kabu = _data.max_kabu;
            _stamina += _data.stamina_gain;
        }
        _stamina = Mathf.Clamp(_stamina, 0, 100); 
    }
    #endregion

    #region Input

    private Vector2 _direction;
    private bool _glide_input;
    
    public void input_direction(InputAction.CallbackContext _move) { _direction = _move.ReadValue<Vector2>(); }
    public void input_glide(InputAction.CallbackContext _glide) { _glide_input = _glide.ReadValueAsButton(); }
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
            var acceleration = grounded() ? _data.run_acceleration : _data.air_acceleration;
            _velocity.x = Mathf.MoveTowards(_body.velocity.x, _direction.x * _data.max_speed, acceleration * Time.fixedDeltaTime);
        }
        _body.velocity = new Vector2(_velocity.x, _body.velocity.y);
        if(_direction.x != 0) { _body.transform.localScale = new Vector3(_direction.x, 1, 1); }
    }
    #endregion

    #region Jump
    public void Jump(InputAction.CallbackContext _jump)
    {
        if (_jump.performed && grounded() && !walled() || _jump.performed && _kabu > 0 && !walled()) 
        {
            if (!grounded()) { _kabu--; }
            _body.velocity = new Vector2(_body.velocity.x, 0);
            _body.AddForce(Vector2.up * _data.jump_force, ForceMode2D.Impulse); 
        } 
        else if(_jump.performed && walled() && _stamina != 0) 
        {
            _body.velocity = new Vector2(_body.velocity.x, 0);
            _body.AddForce(new Vector2(-_direction.x * _data.jump_force/2, _data.jump_force), ForceMode2D.Impulse);
            _stamina -= _data.stamina_wj_loss;
        }
        if(_body.velocity.y > 0 && _jump.canceled) { _body.gravityScale = _data.base_gravity * _data.low_jump_gravity_multiplier; }
    }
    #endregion

    #region Fall & Glide
    private bool _gliding;

    public void Fall()
    {
        _gliding = _glide_input && !grounded() && _stamina != 0 && !walled();
        var max_fall_speed = _gliding || walled() && _direction.x != 0? _data.glide_max_fall_speed : _data.max_fall_speed;
        if(_body.velocity.y <= 0) { _body.gravityScale = _data.base_gravity * _data.fall_gravity_multiplier; }
        _body.velocity = new Vector2(_body.velocity.x, Mathf.Max(_body.velocity.y, -max_fall_speed));
        if (_gliding) { _stamina -= _data.stamina_loss; }
    }
    #endregion

    private void FixedUpdate()
    {
        Horizontal_Movement();
        Fall();
        Ressources();
    }
}
