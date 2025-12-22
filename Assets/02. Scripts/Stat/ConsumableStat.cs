using System;
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

    private event Action _onDataChanged;

    public void Initialize(Action onDataChanged = null)
    {
        _onDataChanged = onDataChanged;
        SetMaxValue(_maxValue);
    }

    public void Regenerate(float time)
    {
        _value += _regenValue * time;

        if (_value > _maxValue)
        {
            _value = _maxValue;
        }

        _onDataChanged?.Invoke();
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

        _onDataChanged?.Invoke();
    }

    public void IncreaseMax(float amount)
    {
        _maxValue += amount;

        _onDataChanged?.Invoke();
    }

    public void Increase(float amount)
    {
        _value += amount;

        if (_value > _maxValue)
        {
            _value = _maxValue;
        }

        _onDataChanged?.Invoke();
    }
    public void SetMaxValue(float amount)
    {
        _maxValue = amount;

        _onDataChanged?.Invoke();
    }

    public void SetValue(float amount)
    {
        _value = amount;

        _onDataChanged?.Invoke();
    }

    public void Decrease(float amount)
    {
        _value -= amount;
        if (_value < 0)
        {
            _value = 0;
        }

        _onDataChanged?.Invoke();
    }
}
