using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBTokunoScribe : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBTokunoScribe()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( BookOfBushido ), 280, 20, 0x238C, 0 ) );
				Add( new GenericBuyInfo( typeof( BookOfNinjitsu ), 335, 20, 0x23A0, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
			}
		}
	}
}