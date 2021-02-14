using System;
using System.Collections;
using Server;
using Server.Guilds;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Gumps
{
	public class WarRequestGump : Gump
	{
		private Mobile m_Mobile;
		private Guild m_Guild;
		private Guild t_Guild;

		public WarRequestGump( Mobile from, Guild target ) : base( 10, 40 )
		{
			m_Mobile = from;

			t_Guild = target;

			m_Guild = from.Guild as Guild;

			int h, m;
			uint ET;

			ET = m_Guild.GetWarTime( t_Guild );

			h = (int) (ET/60);

			m = (int) (ET - h*60);

			AddPage( 0 );

			AddBackground( 0, 0, 520, 335, 0x242C );

			AddHtmlLocalized( 20, 15, 480, 26, 1062975, 0x0, false, false ); // Guild Relationship

			AddImageTiled( 20, 40, 480, 2, 0x2711 );

			AddHtmlLocalized( 20, 50, 120, 26, 1062954, 0x0, true, false ); // Guild Name

			string name;

			if ( (name = target.Name) != null && (name = name.Trim()).Length <= 0 )
			{
				name = "";
			}

			AddHtml( 150, 53, 360, 26, name, false, false );

			AddHtmlLocalized( 20, 80, 120, 26, 1063025, 0x0, true, false ); // Alliance

			AddHtml( 150, 83, 360, 26, "<basefont color=#black></basefont>", false, false );

			AddHtmlLocalized( 20, 110, 120, 26, 1063139, 0x0, true, false ); // Abbreviation

			string abbr;

			if ( (abbr = target.Abbreviation) != null && (abbr = abbr.Trim()).Length <= 0 )
			{
				abbr = "";
			}

			AddHtml( 150, 113, 120, 26, abbr, false, false );

			AddHtmlLocalized( 280, 110, 120, 26, 1062966, 0x0, true, false ); // Your Kills

			AddHtml( 410, 113, 120, 26, "<basefont color=#990000>0/" + m_Guild.GetMaxKills( t_Guild ) + "</basefont>", false, false );

			AddHtmlLocalized( 20, 140, 120, 26, 1062968, 0x0, true, false ); // Time Remaining

			AddHtml( 150, 143, 120, 26, "<basefont color=#990000>" + h + ":" + m + "</basefont>", false, false );

			AddHtmlLocalized( 280, 140, 120, 26, 1062967, 0x0, true, false ); // Their Kills

			AddHtml( 410, 143, 120, 26, "<basefont color=#990000>0/" + m_Guild.GetMaxKills( t_Guild ) + "</basefont>", false, false );

			AddImageTiled( 20, 172, 480, 2, 0x2711 );

			AddBackground( 275, 290, 225, 26, 0x2486 );

			AddButton( 280, 295, 0x845, 0x846, 0, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 305, 293, 185, 26, 3000091, 0x0, false, false ); // Cancel

			AddBackground( 20, 290, 225, 26, 0x2486 );

			AddButton( 25, 295, 0x845, 0x846, 1, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 50, 293, 185, 26, 1062982, 0x0, false, false ); // Dismiss Challenge

			AddHtmlLocalized( 20, 180, 480, 30, 1063027, 0x0, true, false ); // You have challenged this guild to war!

			AddImageTiled( 20, 245, 480, 2, 0x2711 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			int m_Rank = (m_Mobile as PlayerMobile).GuildRank;

			if ( m_Guild.BadMember( m_Mobile ) )
			{
				return;
			}

			switch ( info.ButtonID )
			{
				case 0:
					{
						m_Mobile.CloseGump( typeof( WarRequestGump ) );

						BaseGuild[] guilds = Guild.Search( "" );

						m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );

						break;
					}
				case 1:
					{
						if ( m_Rank != 4 && m_Rank != 5 )
						{
							m_Mobile.SendLocalizedMessage( 1063440 ); // You don't have permission to negotiate wars. 

							return;
						}
						else if ( t_Guild != null )
						{
							t_Guild.WarInvitations.Remove( m_Guild );

							m_Guild.WarDeclarations.Remove( t_Guild );

							m_Guild.DelWar( t_Guild );

							t_Guild.WarInitializations.Remove( m_Guild );

							m_Mobile.SendLocalizedMessage( 1070752 ); // The proposal has been updated.
						}

						m_Mobile.CloseGump( typeof( WarRequestGump ) );

						break;
					}
			}
		}
	}
}