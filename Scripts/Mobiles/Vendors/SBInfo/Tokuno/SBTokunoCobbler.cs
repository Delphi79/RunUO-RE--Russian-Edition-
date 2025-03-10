using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBTokunoCobbler : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBTokunoCobbler()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( SamuraiTabi ), 16, 20, 0x2796, 0 ) );
				Add( new GenericBuyInfo( typeof( NinjaTabi ), 16, 20, 0x2797, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( SamuraiTabi ), 8 );
				Add( typeof( NinjaTabi ), 8 );
			}
		}
	}
}