using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBStavesWeapon : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBStavesWeapon()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( BlackStaff ), 27, 20, 0xDF1, 0 ) );
				Add( new GenericBuyInfo( typeof( GnarledStaff ), 24, 20, 0x13F8, 0 ) );
				Add( new GenericBuyInfo( typeof( QuarterStaff ), 30, 20, 0xE89, 0 ) );
				Add( new GenericBuyInfo( typeof( ShepherdsCrook ), 24, 20, 0xE81, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( BlackStaff ), 24 );
				Add( typeof( GnarledStaff ), 12 );
				Add( typeof( QuarterStaff ), 15 );
				Add( typeof( ShepherdsCrook ), 12 );
			}
		}
	}
}