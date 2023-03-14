using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Modifiable
{
    [SerializeField]
    private float OriginalValue;
    [SerializeField]
    private float FinalValue;
    [SerializeField]
    private List<Modifier> Modifiers;

    public float Value => FinalValue;

    public float BaseValue 
    {
        get
        {
            return BaseValue;
        }

        set
        {
            if (value != BaseValue)
            {
                BaseValue = value;
                UpdateValue();
            }
        }
    }

    public Modifiable(float baseValue)
        : this()
    {
        BaseValue = baseValue;
        UpdateValue();
    }

    public void AddModifier(Modifier modifier)
    {
        if (Modifiers != null)
        {
            if (ModifierIdxById(modifier.Id) == -1)
            {
                Modifiers.Add(modifier);
                UpdateValue();
            }
        }
        else
        {
            Modifiers = new List<Modifier>()
            {
                modifier,
            };
        }
    }

    public void RemoveModifier(string id)
    {
        var removeAtIdx = ModifierIdxById(id);
        if (removeAtIdx == -1) { return; }
        Modifiers.RemoveAt(removeAtIdx);
        UpdateValue();
    }

    private int ModifierIdxById(string id)
    {
        if (Modifiers != null)
        {
            var count = Modifiers.Count;
            for (var i = 0; i < count; ++i)
            {
                if (Modifiers[i].Id == id)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    private void UpdateValue()
    {
        var add = 0.0f;
        var mul = 1.0f;

        if (Modifiers != null)
        {
            var count = Modifiers.Count;
            for (var i = 0; i < count; ++i)
            {
                var modifier = Modifiers[i];
                add += modifier.Add;
                mul *= modifier.Multiply;
            }
        }

        FinalValue = BaseValue * mul + add;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

[Serializable]
public struct Modifier
{
    public string Id;

    public float Add;
    public float Multiply;
}
