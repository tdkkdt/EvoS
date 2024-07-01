using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EvoS.Framework.Network.Static
{
    [Serializable]
    [EvosMessage(491)]
    public class InventoryComponent
    {
        public InventoryComponent()
        {
            NextItemId = 1;
            Items = new List<InventoryItem>();
            Karmas = new Dictionary<int, Karma>();
            Loots = new Dictionary<int, Loot>();
            CharacterItemDropBalanceValues = new Dictionary<CharacterType, int>();
            LastLockboxOpenTime = DateTime.MinValue;
        }

        public InventoryComponent(List<InventoryItem> items, Dictionary<int, Karma> karmas, Dictionary<int, Loot> loots)
        {
            Items = items;
            Karmas = karmas;
            Loots = loots;
        }

        public int NextItemId { get; set; }

        [EvosMessage(224)]
        public List<InventoryItem> Items { get; set; }

        [EvosMessage(496)]
        public Dictionary<int, Karma> Karmas { get; set; }

        [EvosMessage(492)]
        public Dictionary<int, Loot> Loots { get; set; }

        [EvosMessage(500)]
        public Dictionary<CharacterType, int> CharacterItemDropBalanceValues { get; set; }

        public DateTime LastLockboxOpenTime { get; set; }

        public object ShallowCopy()
        {
            return MemberwiseClone();
        }

        public InventoryComponent Clone()
        {
            string value = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<InventoryComponent>(value);
        }

        public InventoryComponent CloneForClient()
        {
            return new InventoryComponent
            {
                Items = Items,
                Loots = Loots,
                Karmas = Karmas,
                LastLockboxOpenTime = LastLockboxOpenTime
            };
        }

        private class InventoryItemListCache
        {
            public InventoryItemListCache()
            {
                Count = 0;
                Items = new List<InventoryItem>();
            }

            public int Count;

            public List<InventoryItem> Items;
        }
    }
}
