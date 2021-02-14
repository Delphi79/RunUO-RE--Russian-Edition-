using System;
using Server;
using System.Collections;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
	public class DiplomacyGump : GListGump
	{
		public DiplomacyGump( Mobile from, Guild guild, ArrayList list, int Sort_Type ) : base( from, guild, list, Sort_Type )
		{
		}

		protected override void Design()
		{
			AddButton( 71, 45, 0x845, 0x846, 1000, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 96, 43, 110, 26, 1063014, false, false );

			AddButton( 241, 45, 0x845, 0x846, 2000, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 266, 43, 110, 26, 1062974, false, false );

			AddButton( 406, 45, 0x845, 0x846, 3000, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 431, 43, 110, 26, 1062978, 0xF, false, false );

			AddTextEntry( 135, 80, 375, 30, 0, 1, "" );

			AddButton( 520, 75, 0x867, 0x868, 750, GumpButtonType.Reply, 0 );

			AddButton( 355, 377, 0x845, 0x846, 975, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 380, 375, 160, 26, 1063083, false, false );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Guild.BadMember( m_Mobile ) )
			{
				return;
			}

			int i = info.ButtonID - 1;

			if ( i >= 0 && i < m_List.Count )
			{
				Guild g = (Guild) m_List[ i ];

				if ( g != null && !g.Disbanded )
				{
					if ( g == m_Guild )
					{
						m_Mobile.CloseGump( typeof( DiplomacyGump ) );

						m_Mobile.SendGump( new SEGuildGump( m_Mobile, m_Guild ) );
					}
					else if ( m_Guild.WarDeclarations.Contains( g ) )
					{
						m_Mobile.CloseGump( typeof( DiplomacyGump ) );

						m_Mobile.SendGump( new WarRequestGump( m_Mobile, g ) );
					}
					else if ( m_Guild.WarInvitations.Contains( g ) )
					{
						m_Mobile.CloseGump( typeof( DiplomacyGump ) );

						m_Mobile.SendGump( new ReceiveWarGump( m_Mobile, g ) );
					}
					else if ( m_Guild.IsWar( g ) )
					{
						m_Mobile.CloseGump( typeof( DiplomacyGump ) );

						m_Mobile.SendGump( new ProcessWarGump( m_Mobile, g ) );
					}
					else if ( m_Guild.AllyDeclarations.Contains( g ) )
					{
						m_Mobile.CloseGump( typeof( DiplomacyGump ) );

						m_Mobile.SendGump( new AllianceRequestGump( m_Mobile, g ) );
					}
					else if ( m_Guild.AllyInvitations.Contains( g ) )
					{
						m_Mobile.CloseGump( typeof( DiplomacyGump ) );

						m_Mobile.SendGump( new AcceptAllianceGump( m_Mobile, g ) );
					}
					else if ( m_Guild.IsAlly( g ) )
					{
						if ( m_Guild.AllianceLeader )
						{
							m_Mobile.CloseGump( typeof( DiplomacyGump ) );

							m_Mobile.SendGump( new AllianceLeaderGump( m_Mobile, g ) );
						}
						else
						{
							m_Mobile.CloseGump( typeof( DiplomacyGump ) );

							m_Mobile.SendGump( new SlaveAllyGump( m_Mobile, g ) );
						}
					}
					else
					{
						m_Mobile.CloseGump( typeof( DiplomacyGump ) );

						m_Mobile.SendGump( new DiplomacyMiscGump( m_Mobile, g ) );
					}
				}
			}

			if ( info.ButtonID == 750 )
			{
				string text = info.GetTextEntry( 1 ).Text;
				text = text.Trim();

				if ( text.Length >= 3 )
				{
					BaseGuild[] guilds = Guild.Search( text );

					m_Mobile.CloseGump( typeof( DiplomacyGump ) );

					if ( guilds.Length > 0 )
					{
						m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );
					}
					else
					{
						m_Mobile.SendLocalizedMessage( 1018003 ); // No guilds found matching - try another name in the search
					}
				}
				else
				{
					m_Mobile.SendMessage( "Search string must be at least three letters in length." );
				}
			}
			else if ( info.ButtonID == 3000 )
			{
				BaseGuild[] guilds = Guild.Search( "" );

				if ( guilds.Length > 0 )
				{
					m_Mobile.CloseGump( typeof( DiplomacyGump ) );

					m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );
				}
			}
			else if ( info.ButtonID == 2000 )
			{
				m_Mobile.CloseGump( typeof( DiplomacyGump ) );

				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 2, "" ) );
			}
			else if ( info.ButtonID == 1000 )
			{
				m_Mobile.CloseGump( typeof( DiplomacyGump ) );

				m_Mobile.SendGump( new SEGuildGump( m_Mobile, m_Guild ) );
			}
			else if ( info.ButtonID == 975 )
			{
				m_Mobile.CloseGump( typeof( DiplomacyGump ) );

				m_Mobile.SendGump( new ASearchGump( m_Mobile, m_Guild ) );
			}
			else if ( info.ButtonID == 1100 )
			{
				m_Mobile.CloseGump( typeof( DiplomacyGump ) );

				m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, m_List, 10 ) );
			}
			else if ( info.ButtonID == 1200 )
			{
				m_Mobile.CloseGump( typeof( DiplomacyGump ) );

				m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, m_List, 11 ) );
			}
			else if ( info.ButtonID == 1300 )
			{
				m_Mobile.CloseGump( typeof( DiplomacyGump ) );

				m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, m_List, 12 ) );
			}
			else if ( info.ButtonID == 1101 )
			{
				m_Mobile.CloseGump( typeof( DiplomacyGump ) );

				m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, m_List, 0 ) );
			}
			else if ( info.ButtonID == 1201 )
			{
				m_Mobile.CloseGump( typeof( DiplomacyGump ) );

				m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, m_List, 1 ) );
			}
			else if ( info.ButtonID == 1301 )
			{
				m_Mobile.CloseGump( typeof( DiplomacyGump ) );

				m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, m_List, 2 ) );
			}
		}
	}
}