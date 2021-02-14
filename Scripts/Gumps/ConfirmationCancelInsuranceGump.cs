using System;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
	public class ConfirmationCancelInsuranceGump : Gump
	{
		private PlayerMobile player;

		public ConfirmationCancelInsuranceGump( PlayerMobile pm ) : base( 250, 200 )
		{
			player = pm;

			AddPage( 0 );

			AddBackground( 0, 0, 240, 142, 5054 );

			AddImageTiled( 6, 6, 228, 100, 2624 );

			AddImageTiled( 6, 116, 228, 20, 2624 );

			AddAlphaRegion( 6, 6, 228, 142 );

			AddHtmlLocalized( 8, 8, 228, 100, 1071021, 0x7FFF, false, false ); // You are about to disable inventory insurance auto-renewal.

			AddButton( 6, 116, 4017, 4018, 0, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 40, 118, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL

			AddButton( 114, 116, 4005, 4007, 1, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 148, 118, 450, 20, 1071022, 0x7FFF, false, false ); // DISABLE IT!
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 0:
					{
						player.SendLocalizedMessage( 1042021 ); // Cancelled.

						break;
					}
				case 1:
					{
						player.SendLocalizedMessage( 1061075, "", 0x23 ); // You have cancelled automatically reinsuring all insured items upon death
						player.AutoRenewInsurance = false;

						break;
					}
			}
		}
	}
}