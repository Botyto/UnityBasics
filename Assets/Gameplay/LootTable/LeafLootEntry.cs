using System;
using System.Collections.Generic;

public class LeafLootEntry<TAmount, TResource> : LootResult<TAmount, TResource>, ILootEntry<TAmount, TResource>
{
    public IEnumerator<LootResult<TAmount, TResource>> GenerateLoot(Random random)
    {
        yield return this;
    }
}
