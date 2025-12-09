using UnityEngine;

[System.Serializable]
public class ConsumableStat
{
    [SerializeField] private float _maxValue;
    [SerializeField] private float _value;
    [SerializeField] private float _regenValue;

    public float MaxValue => _maxValue;
    public float Value => _value;
    public float RegenValue => _regenValue;

    public void Initialize()
    {
        SetMaxValue(_maxValue);
    }

    public void Regenerate(float time)
    {
        _value += _regenValue * time;

        if (_value > _maxValue)
        {
            _value = _maxValue;
        }
    }

    public bool TryConsume(float amount)
    {
        if (_value - amount < 0) return false;

        Consume(amount);
        return true;
    }

    public void Consume(float amount)
    {
        _value -= amount;
    }

    public void IncreaseMax(float amount)
    {
        _maxValue += amount;
    }

    public void Increase(float amount)
    {
        _value += amount;

        if (_value > _maxValue)
        {
            _value = _maxValue;
        }
    }
    public void SetMaxValue(float amount)
    {
        _maxValue = amount;
    }

    public void SetValue(float amount)
    {
        _value = amount;
    }
}
