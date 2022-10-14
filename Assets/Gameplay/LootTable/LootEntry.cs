using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NestedLootEntry<TAmount, TResource> : ILootEntry<TAmount, TResource>
{
    public struct WeightedEntry
    {
        public float Weight;
        public ILootEntry<TAmount, TResource> Entry;
    }

    public enum EAction
    {
        All = 0,
        Random = 1,
        First = 2,
    }

    public float Weight;
    public EAction Action;
    [Condition(nameof(Action), 1)]
    [Min(1)]
    public int PickCount;
    public List<WeightedEntry> Entries;

    public IEnumerator<LootResult<TAmount, TResource>> GenerateLoot(System.Random random)
    {
        if (Entries == null || Entries.Count == 0)
        {
            yield break;
        }

        IEnumerator<LootResult<TAmount, TResource>> enumerator;

        switch (Action)
        {
            case EAction.All:
                foreach (var entry in Entries)
                {
                    enumerator = entry.Entry.GenerateLoot(random);
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
                break;
            case EAction.Random:
                var copy = new List<WeightedEntry>(Entries);
                copy.Sort((a, b) => a.Weight.CompareTo(b.Weight));
                var totalWeight = copy.Sum(we => we.Weight);
                var count = Mathf.Min(copy.Count, PickCount);
                for (var i = 0; i < count; ++i)
                {
                    var score = (float)random.NextDouble() * totalWeight;
                    var idx = 0;
                    while (score > 0)
                    {
                        var entry = copy[idx];
                        score -= entry.Weight;
                        ++idx;
                        if (score < 0 || idx >= copy.Count)
                        {
                            enumerator = entry.Entry.GenerateLoot(random);
                            while (enumerator.MoveNext())
                            {
                                yield return enumerator.Current;
                            }
                            copy.RemoveAt(idx);
                            break;
                        }
                    }
                }
                break;
            case EAction.First:
                enumerator = Entries[0].Entry.GenerateLoot(random);
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                break;
        }
    }
}
