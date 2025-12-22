using UnityEngine;

public struct Damage
{
    public float Value;
    public Vector3 Direction;
    public Vector3 HitPoint;
    public float KnockbackForce;
    public Vector3 Normal;

    public Damage(float value, Vector3 direction, Vector3 hitPoint, float knockbackForce, Vector3 normal)
    {
        Value = value;
        Direction = direction;
        HitPoint = hitPoint;
        KnockbackForce = knockbackForce;
        Normal = normal;
    }
}
