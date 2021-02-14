using System;
using System.Collections;
using Server;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
	public class ResignGump : MyGuildGump
	{
		public ResignGump( Mobile from, Guild guild ) : base( from, guild )
		{
			Design();

			string charter;

			if ( (charter = guild.Charter) == null || (charter = charter.Trim()).Length <= 0 )
			{
				charter = DefaultCharter;
			}

			AddHtml( 65, 216, 480, 80, charter, true, true );

			AddButton( 455, 375, 0x845, 0x846, 60, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 480, 373, 60, 26, 3006115, 0x3C00, false, false );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Guild.BadMember( m_Mobile ) )
			{
				return;
			}

			for ( int i = 0; i < info.Switches.Length; i++ )
			{
				int switchid = info.Switches[ i ];

				switch ( switchid )
				{
					case 400:
						{
							m_Mobile.DisplayGuildTitle = !m_Mobile.DisplayGuildTitle;

							break;
						}
				}
			}

			switch ( info.ButtonID )
			{
				case 10: // Open My Guild Gump
					{
						m_Mobile.CloseGump( typeof( ResignGump ) );
						m_Mobile.SendGump( new SEGuildGump( m_Mobile, m_Guild ) );
						break;
					}
				case 20: // Open Guild member list
					{
						m_Mobile.CloseGump( typeof( ResignGump ) );
						m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 2, "" ) );
						break;
					}
				case 30: // Open Diplomancy Gump
					{
						string text = "";

						BaseGuild[] guilds = Guild.Search( text );

						if ( guilds.Length > 0 )
						{
							m_Mobile.CloseGump( typeof( ResignGump ) );
							m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );
						}

						break;
					}
				case 60: // Resgn from Guild
					{
						m_Mobile.CloseGump( typeof( ResignGump ) );
						m_Guild.RemoveMember( m_Mobile );
						break;
					}
			}
		}
	}
}