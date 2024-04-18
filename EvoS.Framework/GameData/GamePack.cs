using System;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.GameData;

[Serializable]
public class GamePack
{
    // [HideInInspector]
    public int Index;
    public string EditionName;
    public CountryPrices Prices;
    public string ProductCode;
    public string Entitlement;
    public GamePackUpgrade[] Upgrades;
    // [TextArea(2, 5)]
    public string Description;
    // public Sprite Sprite;
    public int[] InventoryItemTemplateIds;

    // public string GetEditionName()
    // {
    //     return StringUtil.TR_GamePackEditionName(Index);
    // }
    //
    // public string GetDescription()
    // {
    //     return StringUtil.TR_GamePackDescription(Index);
    // }
}
