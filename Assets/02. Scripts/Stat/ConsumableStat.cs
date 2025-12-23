using System;
using UnityEngine;

[System.Serializable]
public class ConsumableStat
{
    [SerializeField] private float _maxValue;
    [SerializeField] private float _value;
    [SerializeField] private float _regenValue;

    public float MaxValue
    {
        get => _maxValue;
        private set
        {
            _maxValue = Mathf.Max(0, value); // 0 이상
            Value = _value;
            Notify();
        }
    }

    public float Value
    {
        get => _value;
        private set
        {
            _value = Mathf.Clamp(value, 0, _maxValue); // 0 ~ 최대치
            Notify();
        }
    }

    public float RegenValue => _regenValue;

    private event Action _onDataChanged;

    private void Notify() => _onDataChanged?.Invoke();

    public void Initialize(Action onDataChanged = null)
    {
        _onDataChanged = onDataChanged;
        MaxValue = _maxValue;
        Value = _value;
    }

    public void Regenerate(float time)
    {
        Value += _regenValue * time;
    }

    public bool TryConsume(float amount)
    {
        if (_value - amount < 0) return false;
        Value -= amount;
        return true;
    }

    public void Consume(float amount)
    {
        Value -= amount;
    }

    public void IncreaseMax(float amount)
    {
        MaxValue += amount;
    }

    public void Increase(float amount)
    {
        Value += amount;
    }

    public void SetMaxValue(float amount)
    {
        MaxValue = amount;
    }

    public void SetValue(float amount)
    {
        Value = amount;
    }

    public void Decrease(float amount)
    {
        Value -= amount;
    }
}
