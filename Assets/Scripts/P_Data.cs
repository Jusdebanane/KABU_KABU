using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Data : MonoBehaviour
{
    [Header("STAMINA")]
    public float max_stamina;
    public float stamina_loss;
    public float stamina_wj_loss;
    public float stamina_gain;

    [Header("GRAVITY")]
    public float base_gravity;
    public float fall_gravity_multiplier;
    public float low_jump_gravity_multiplier;

    [Header("FALL")]
    public bool can_glide;
    public float max_fall_speed;
    public float glide_max_fall_speed;

    [Header("HORIZONTAL MOVEMENT")]
    public float max_speed;
    public float run_acceleration;
    public float air_acceleration;
    public float run_deceleration;
    public float air_deceleration;

    [Header("JUMP")]
    public float jump_force;
    public int max_kabu;


}
