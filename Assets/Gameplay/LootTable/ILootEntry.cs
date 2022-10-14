using System.Collections.Generic;

public interface ILootEntry<TAmount, TResource>
{
    IEnumerator<LootResult<TAmount, TResource>> GenerateLoot(System.Random random);
}
