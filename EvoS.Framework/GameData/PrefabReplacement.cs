using System;

namespace EvoS.Framework.GameData;

[Serializable]
public class PrefabReplacement
{
    public PrefabResourceLink OriginalPrefab;
    public PrefabResourceLink Replacement;
}
