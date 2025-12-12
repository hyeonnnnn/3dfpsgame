using UnityEngine;

public struct Damage
{
    public float Value;
    public Vector3 Direction;
    public float KnockbackForce;

    public Damage(float value, Vector3 direction, float knockbackForce)
    {
        Value = value;
        Direction = direction;
        KnockbackForce = knockbackForce;
    }
}
