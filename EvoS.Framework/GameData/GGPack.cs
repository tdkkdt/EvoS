using System;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.GameData;

[Serializable]
public class GGPack
{
    // [HideInInspector]
    public int Index;
    public int NumberOfBoosts;
    public CountryPrices Prices;
    public string ProductCode;
    public int SortOrder;
    // public Sprite GGPackSprite;
}
