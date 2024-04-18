using System;

namespace EvoS.Framework.GameData;

[Serializable]
public class GGPackData
{
    public GGPack[] m_ggPacks;

    public static GGPackData Get()
    {
        return GameWideData.Get().m_ggPackData;
    }
}
