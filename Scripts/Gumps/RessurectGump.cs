using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Gumps
{
	public enum ResurrectMessage
	{
		ChaosShrine = 0,
		VirtueShrine = 1,
		Healer = 2,
		Generic = 3,
	}

	public class ResurrectGump : Gump
	{
		private Mobile m_Healer;
		private int m_Price;

		public ResurrectGump( Mobile owner ) : this( owner, owner, ResurrectMessage.Generic )
		{
		}

		public ResurrectGump( Mobile owner, Mobile healer ) : this( owner, healer, ResurrectMessage.Generic )
		{
		}

		public ResurrectGump( Mobile owner, ResurrectMessage msg ) : this( owner, owner, msg )
		{
		}

		public ResurrectGump( Mobile owner, Mobile healer, ResurrectMessage msg ) : base( 100, 0 )
		{
			m_Healer = healer;

			AddPage( 0 );

			AddBackground( 0, 0, 400, 350, 2600 );

			AddHtmlLocalized( 0, 20, 400, 35, 1011022, false, false ); // <center>Resurrection</center>

			AddHtmlLocalized( 50, 55, 300, 140, 1011023 + (int) msg, true, true ); /* It is possible for you to be resurrected here by this healer. Do you wish to try?<br>
																				   * CONTINUE - You chose to try to come back to life now.<br>
																				   * CANCEL - You prefer to remain a ghost for now.
																				   */

			AddButton( 200, 227, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 235, 230, 110, 35, 1011012, false, false ); // CANCEL

			AddButton( 65, 227, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 100, 230, 110, 35, 1011011, false, false ); // CONTINUE
		}

		public ResurrectGump( Mobile owner, Mobile healer, int price ) : base( 150, 50 )
		{
			m_Healer = healer;
			m_Price = price;

			Closable = false;

			AddPage( 0 );

			AddImage( 0, 0, 3600 );

			AddImageTiled( 0, 14, 15, 200, 3603 );
			AddImageTiled( 380, 14, 14, 200, 3605 );

			AddImage( 0, 201, 3606 );

			AddImageTiled( 15, 201, 370, 16, 3607 );
			AddImageTiled( 15, 0, 370, 16, 3601 );

			AddImage( 380, 0, 3602 );

			AddImage( 380, 201, 3608 );

			AddImageTiled( 15, 15, 365, 190, 2624 );

			AddRadio( 30, 140, 9727, 9730, true, 1 );
			AddHtmlLocalized( 65, 145, 300, 25, 1060015, 0x7FFF, false, false ); // Grudgingly pay the money

			AddRadio( 30, 175, 9727, 9730, false, 0 );
			AddHtmlLocalized( 65, 178, 300, 25, 1060016, 0x7FFF, false, false ); // I'd rather stay dead, you scoundrel!!!

			AddHtmlLocalized( 30, 20, 360, 35, 1060017, 0x7FFF, false, false ); // Wishing to rejoin the living, are you?  I can restore your body... for a price of course...

			AddHtmlLocalized( 30, 105, 345, 40, 1060018, 0x5B2D, false, false ); // Do you accept the fee, which will be withdrawn from your bank?

			AddImage( 65, 72, 5605 );

			AddImageTiled( 80, 90, 200, 1, 9107 );
			AddImageTiled( 95, 92, 200, 1, 9157 );

			AddLabel( 90, 70, 1645, price.ToString() );
			AddHtmlLocalized( 140, 70, 100, 25, 1023823, 0x7FFF, false, false ); // gold coins

			AddButton( 290, 175, 247, 248, 2, GumpButtonType.Reply, 0 );

			AddImageTiled( 15, 14, 365, 1, 9107 );
			AddImageTiled( 380, 14, 1, 190, 9105 );
			AddImageTiled( 15, 205, 365, 1, 9107 );
			AddImageTiled( 15, 14, 1, 190, 9105 );
			AddImageTiled( 0, 0, 395, 1, 9157 );
			AddImageTiled( 394, 0, 1, 217, 9155 );
			AddImageTiled( 0, 216, 395, 1, 9157 );
			AddImageTiled( 0, 0, 1, 217, 9155 );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			from.CloseGump( typeof( ResurrectGump ) );

			if ( info.ButtonID == 1 || info.ButtonID == 2 )
			{
				if ( from.Map == null || !from.Map.CanFit( from.Location, 16, false, false ) )
				{
					from.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
					return;
				}

				if ( m_Price > 0 )
				{
					if ( info.IsSwitched( 1 ) )
					{
						if ( Banker.Withdraw( from, m_Price ) )
						{
							from.SendLocalizedMessage( 1060398, m_Price.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
							from.SendLocalizedMessage( 1060022, Banker.GetBalance( from ).ToString() ); // You have ~1_AMOUNT~ gold in cash remaining in your bank box.
						}
						else
						{
							from.SendLocalizedMessage( 1060020 ); // Unfortunately, you do not have enough cash in your bank to cover the cost of the healing.
							return;
						}
					}
					else
					{
						from.SendLocalizedMessage( 1060019 ); // You decide against paying the healer, and thus remain dead.
						return;
					}
				}

				from.PlaySound( 0x214 );

				from.Resurrect();

				Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0, 0, 0, 0, 0, 5060, 0 );

				Effects.SendMovingParticles( new Entity( 0, new Point3D( from.X - 6, from.Y - 6, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer) 255, 0x100 );
				Effects.SendMovingParticles( new Entity( 0, new Point3D( from.X - 4, from.Y - 6, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer) 255, 0x100 );
				Effects.SendMovingParticles( new Entity( 0, new Point3D( from.X - 6, from.Y - 4, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer) 255, 0x100 );

				Effects.SendTargetParticles( from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer) 255, 0x100 );

				if ( m_Healer != null && from != m_Healer )
				{
					VirtueLevel level = VirtueHelper.GetLevel( m_Healer, VirtueName.Compassion );

					switch ( level )
					{
						case VirtueLevel.Seeker:
							from.Hits = AOS.Scale( from.HitsMax, 20 );
							break;
						case VirtueLevel.Follower:
							from.Hits = AOS.Scale( from.HitsMax, 40 );
							break;
						case VirtueLevel.Knight:
							from.Hits = AOS.Scale( from.HitsMax, 80 );
							break;
					}
				}

				Mobile m = from;

				Misc.Titles.AwardFame( from, -100, true ); // TODO: Proper fame loss

				if ( !Core.AOS && from.ShortTermMurders >= 5 )
				{
					double loss = (100.0 - (4.0 + (from.ShortTermMurders/5.0)))/100.0; //5 to 15% loss
					if ( loss < 0.85 )
					{
						loss = 0.85;
					}
					else if ( loss > 0.95 )
					{
						loss = 0.95;
					}

					if ( from.RawStr*loss > 10 )
					{
						from.RawStr = (int) (from.RawStr*loss);
					}
					if ( from.RawInt*loss > 10 )
					{
						from.RawInt = (int) (from.RawInt*loss);
					}
					if ( from.RawDex*loss > 10 )
					{
						from.RawDex = (int) (from.RawDex*loss);
					}

					for ( int s = 0; s < from.Skills.Length; s++ )
					{
						if ( from.Skills[ s ].Base*loss > 35 )
						{
							from.Skills[ s ].Base *= loss;
						}
					}
				}
			}
		}
	}
}