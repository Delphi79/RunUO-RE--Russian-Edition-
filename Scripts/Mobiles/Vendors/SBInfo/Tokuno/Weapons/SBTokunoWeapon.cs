using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBTokunoWeapon : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBTokunoWeapon()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( NoDachi ), 82, 20, 0x27A2, 0 ) );
				Add( new GenericBuyInfo( typeof( Tessen ), 83, 20, 0x27A3, 0 ) );
				Add( new GenericBuyInfo( typeof( Wakizashi ), 38, 20, 0x27A4, 0 ) );
				Add( new GenericBuyInfo( typeof( Tetsubo ), 43, 20, 0x27A6, 0 ) );
				Add( new GenericBuyInfo( typeof( Lajatang ), 108, 20, 0x27A7, 0 ) );
				Add( new GenericBuyInfo( typeof( Daisho ), 66, 20, 0x27A9, 0 ) );
				Add( new GenericBuyInfo( typeof( Tekagi ), 55, 20, 0x27AB, 0 ) );
				Add( new GenericBuyInfo( typeof( Shuriken ), 18, 20, 0x27F7, 0 ) );
				Add( new GenericBuyInfo( typeof( Kama ), 61, 20, 0x27AD, 0 ) );
				Add( new GenericBuyInfo( typeof( Sai ), 56, 20, 0x27AF, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Sai ), 28 );
				Add( typeof( Kama ), 30 );
				Add( typeof( Shuriken ), 9 );
				Add( typeof( Tekagi ), 27 );
				Add( typeof( Daisho ), 33 );
				Add( typeof( Lajatang ), 54 );
				Add( typeof( Tetsubo ), 21 );
				Add( typeof( Wakizashi ), 19 );
				Add( typeof( Tessen ), 41 );
				Add( typeof( NoDachi ), 41 );
			}
		}
	}
}