using System.Collections.Generic;

public class EmptyLootEntry<TAmount, TResource> : ILootEntry<TAmount, TResource>
{
    public IEnumerator<LootResult<TAmount, TResource>> GenerateLoot(System.Random random)
    {
        yield break;
    }
}
