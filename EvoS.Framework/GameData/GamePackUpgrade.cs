using System;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.GameData;

[Serializable]
public class GamePackUpgrade
{
    public int AlreadyOwnedGamePack;
    public string ProductCode;
    public CountryPrices Prices;
}
