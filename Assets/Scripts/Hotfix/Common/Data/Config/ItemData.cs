
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;


namespace Hotfix.Common.Data
{
    public sealed partial class ItemData : Luban.BeanBase
    {
        public ItemData(ByteBuf _buf) 
        {
            Id = _buf.ReadInt();
            Name = _buf.ReadString();
            Desc = _buf.ReadString();
            Price = _buf.ReadInt();
            UpgradeToItemId = _buf.ReadInt();
            UpgradeToItemId_Ref = null;
            if(_buf.ReadBool()){ ExpireTime = _buf.ReadLong(); } else { ExpireTime = null; }
            BatchUseable = _buf.ReadBool();
            Quality = (Item.ItemQuality)_buf.ReadInt();
            ExchangeStream = Item.ItemExchange.DeserializeItemExchange(_buf);
            {int n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);ExchangeList = new System.Collections.Generic.List<Item.ItemExchange>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { Item.ItemExchange _e0;  _e0 = Item.ItemExchange.DeserializeItemExchange(_buf); ExchangeList.Add(_e0);}}
            ExchangeColumn = Item.ItemExchange.DeserializeItemExchange(_buf);
        }

        public static ItemData DeserializeItemData(ByteBuf _buf)
        {
            return new ItemData(_buf);
        }

        /// <summary>
        /// 这是id
        /// </summary>
        public readonly int Id;
        /// <summary>
        /// 名字
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// 描述
        /// </summary>
        public readonly string Desc;
        /// <summary>
        /// 价格
        /// </summary>
        public readonly int Price;
        /// <summary>
        /// 引用当前表
        /// </summary>
        public readonly int UpgradeToItemId;
        public ItemData UpgradeToItemId_Ref;
        /// <summary>
        /// 过期时间
        /// </summary>
        public readonly long? ExpireTime;
        /// <summary>
        /// 能否批量使用
        /// </summary>
        public readonly bool BatchUseable;
        /// <summary>
        /// 品质
        /// </summary>
        public readonly Item.ItemQuality Quality;
        /// <summary>
        /// 道具兑换配置
        /// </summary>
        public readonly Item.ItemExchange ExchangeStream;
        public readonly System.Collections.Generic.List<Item.ItemExchange> ExchangeList;
        /// <summary>
        /// 道具兑换配置
        /// </summary>
        public readonly Item.ItemExchange ExchangeColumn;
    
        public const int __ID__ = 1241678205;
        public override int GetTypeId() => __ID__;

        public  void ResolveRef(Tables tables)
        {
            
            
            
            
            UpgradeToItemId_Ref = tables.TbItemData.GetOrDefault(UpgradeToItemId);
            
            
            
            ExchangeStream?.ResolveRef(tables);
            foreach (var _e in ExchangeList) { _e?.ResolveRef(tables); }
            ExchangeColumn?.ResolveRef(tables);
        }

        public override string ToString()
        {
            return "{ "
            + "id:" + Id + ","
            + "name:" + Name + ","
            + "desc:" + Desc + ","
            + "price:" + Price + ","
            + "upgradeToItemId:" + UpgradeToItemId + ","
            + "expireTime:" + ExpireTime + ","
            + "batchUseable:" + BatchUseable + ","
            + "quality:" + Quality + ","
            + "exchangeStream:" + ExchangeStream + ","
            + "exchangeList:" + Luban.StringUtil.CollectionToString(ExchangeList) + ","
            + "exchangeColumn:" + ExchangeColumn + ","
            + "}";
        }
    }

}
