using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Modifiable
{
    [SerializeField]
    private float m_BaseValue;
    [SerializeField]
    private float m_Value;
    [SerializeField]
    private List<Modifier> m_Modifiers;

    public float Value => m_Value;

    public float BaseValue 
    {
        get
        {
            return m_BaseValue;
        }

        set
        {
            if (value != m_BaseValue)
            {
                m_BaseValue = value;
                UpdateValue();
            }
        }
    }

    public Modifiable(float baseValue)
        : this()
    {
        m_BaseValue = baseValue;
        UpdateValue();
    }

    public void AddModifier(Modifier modifier)
    {
        if (m_Modifiers != null)
        {
            if (ModifierIdxById(modifier.Id) == -1)
            {
                m_Modifiers.Add(modifier);
                UpdateValue();
            }
        }
        else
        {
            m_Modifiers = new List<Modifier>()
            {
                modifier,
            };
        }
    }

    public void RemoveModifier(string id)
    {
        var removeAtIdx = ModifierIdxById(id);
        if (removeAtIdx == -1) { return; }
        m_Modifiers.RemoveAt(removeAtIdx);
        UpdateValue();
    }

    private int ModifierIdxById(string id)
    {
        if (m_Modifiers != null)
        {
            var count = m_Modifiers.Count;
            for (var i = 0; i < count; ++i)
            {
                if (m_Modifiers[i].Id == id)
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

        if (m_Modifiers != null)
        {
            var count = m_Modifiers.Count;
            for (var i = 0; i < count; ++i)
            {
                var modifier = m_Modifiers[i];
                add += modifier.Add;
                mul *= modifier.Multiply;
            }
        }

        m_Value = BaseValue * mul + add;
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
