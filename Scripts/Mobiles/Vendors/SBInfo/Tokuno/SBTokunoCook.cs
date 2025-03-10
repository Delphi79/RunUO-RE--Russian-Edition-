using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBTokunoCook : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBTokunoCook()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( WasabiClumps ), 2, 20, 0x24EB, 0 ) );
				Add( new GenericBuyInfo( typeof( Wasabi ), 2, 20, 0x24E8, 0 ) );
				Add( new GenericBuyInfo( typeof( Wasabi ), 2, 20, 0x24E9, 0 ) );
				Add( new GenericBuyInfo( typeof( SushiRolls ), 3, 20, 0x283E, 0 ) );
				Add( new GenericBuyInfo( typeof( SushiRolls ), 3, 20, 0x283F, 0 ) );
				Add( new GenericBuyInfo( typeof( SushiPlatter ), 3, 20, 0x2840, 0 ) );
				Add( new GenericBuyInfo( typeof( SushiPlatter ), 3, 20, 0x2841, 0 ) );
				Add( new GenericBuyInfo( typeof( GreenTeaBasket ), 2, 20, 0x284B, 0 ) );
				Add( new GenericBuyInfo( typeof( GreenTea ), 3, 20, 0x284C, 0 ) );
				Add( new GenericBuyInfo( typeof( MisoSoup ), 3, 20, 0x284D, 0 ) );
				Add( new GenericBuyInfo( typeof( WhiteMisoSoup ), 3, 20, 0x284E, 0 ) );
				Add( new GenericBuyInfo( typeof( RedMisoSoup ), 3, 20, 0x284F, 0 ) );
				Add( new GenericBuyInfo( typeof( AwaseMisoSoup ), 3, 20, 0x2850, 0 ) );
				Add( new GenericBuyInfo( typeof( BentoBox ), 6, 20, 0x2836, 0 ) );
				Add( new GenericBuyInfo( typeof( BentoBox ), 6, 20, 0x2837, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( WasabiClumps ), 1 );
				Add( typeof( Wasabi ), 1 );
				Add( typeof( SushiRolls ), 1 );
				Add( typeof( SushiPlatter ), 1 );
				Add( typeof( GreenTea ), 1 );
				Add( typeof( MisoSoup ), 1 );
				Add( typeof( WhiteMisoSoup ), 1 );
				Add( typeof( RedMisoSoup ), 1 );
				Add( typeof( AwaseMisoSoup ), 1 );
				Add( typeof( BentoBox ), 3 );
			}
		}
	}
}