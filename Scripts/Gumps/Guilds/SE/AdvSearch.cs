using System;
using System.Collections;
using Server;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
	public class ASearchGump : Gump
	{
		private Mobile m_Mobile;
		private Guild m_Guild;

		public ASearchGump( Mobile from, Guild guild ) : base( 20, 30 )
		{
			m_Mobile = from;
			m_Guild = guild;

			AddPage( 0 );

			AddBackground( 0, 0, 600, 440, 0x24AE );

			AddBackground( 66, 40, 150, 26, 0x2486 );

			AddBackground( 236, 40, 150, 26, 0x2486 );

			AddBackground( 401, 40, 150, 26, 0x2486 );

			AddBackground( 450, 370, 100, 26, 0x2486 );

			AddBackground( 340, 370, 100, 26, 0x2486 );

			AddButton( 71, 45, 0x845, 0x846, 3, GumpButtonType.Reply, 0 ); // -- My Guild

			AddHtmlLocalized( 96, 43, 110, 26, 1063014, false, false );

			AddButton( 241, 45, 0x845, 0x846, 4, GumpButtonType.Reply, 0 ); // -- Roster

			AddHtmlLocalized( 266, 43, 110, 26, 1062974, false, false );

			AddButton( 406, 45, 0x845, 0x846, 5, GumpButtonType.Reply, 0 ); // -- Diplomacy

			AddHtmlLocalized( 431, 43, 110, 26, 1062978, 75, false, false );

			AddHtmlLocalized( 65, 80, 480, 26, 1063124, 75, true, false );

			AddHtmlLocalized( 65, 110, 480, 26, 1063136, 75, false, false );

			AddRadio( 75, 140, 0xD2, 0xD3, false, 1 ); // -- Show Guilds with Relationship

			AddHtmlLocalized( 105, 140, 200, 26, 1063006, false, false );

			AddRadio( 75, 170, 0xD2, 0xD3, false, 2 ); // -- Show Guilds Awaiting Action

			AddHtmlLocalized( 105, 170, 200, 26, 1063005, false, false );

			AddRadio( 75, 200, 0xD2, 0xD3, true, 3 ); // -- Show All Guilds

			AddHtmlLocalized( 105, 200, 200, 26, 1063007, false, false );

			AddButton( 455, 375, 0x845, 0x846, 1, GumpButtonType.Reply, 0 ); // -- OK 

			AddHtmlLocalized( 480, 373, 60, 26, 1006044, false, false );

			AddButton( 345, 375, 0x845, 0x846, 0, GumpButtonType.Reply, 0 ); // -- Cancel

			AddHtmlLocalized( 370, 373, 60, 26, 1006045, false, false );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Guild.BadMember( m_Mobile ) )
			{
				return;
			}

			if ( info.ButtonID == 1 )
			{
				for ( int i = 0; i < info.Switches.Length; i++ )
				{
					int switchid = info.Switches[ i ];

					if ( switchid == 1 )
					{
						m_Mobile.CloseGump( typeof( ASearchGump ) );

						BaseGuild[] guilds = Guild.Search( "" );

						ArrayList m_SortGuild = new ArrayList();

						for ( int j = 0; j < guilds.Length; j++ )
						{
							if ( (guilds[ j ] as Guild).IsWar( m_Guild ) || m_Guild.IsWar( guilds[ j ] as Guild ) )
							{
								m_SortGuild.Add( guilds[ j ] );
							}
							if ( (guilds[ j ] as Guild).IsAlly( m_Guild ) || m_Guild.IsAlly( guilds[ j ] as Guild ) )
							{
								m_SortGuild.Add( guilds[ j ] );
							}
						}

						m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( m_SortGuild ), 3 ) );
					}
					else if ( switchid == 2 )
					{
						m_Mobile.CloseGump( typeof( ASearchGump ) );

						BaseGuild[] guilds = Guild.Search( "" );

						ArrayList m_SortGuild = new ArrayList();

						for ( int j = 0; j < guilds.Length; j++ )
						{
							if ( (guilds[ j ] as Guild).WarDeclarations.Contains( m_Guild ) || m_Guild.WarDeclarations.Contains( guilds[ j ] as Guild ) )
							{
								m_SortGuild.Add( guilds[ j ] );
							}
							if ( (guilds[ j ] as Guild).AllyDeclarations.Contains( m_Guild ) || m_Guild.AllyDeclarations.Contains( guilds[ j ] as Guild ) )
							{
								m_SortGuild.Add( guilds[ j ] );
							}
						}

						m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( m_SortGuild ), 4 ) );
					}
					else if ( switchid == 3 )
					{
						string text = "";
						text = text.Trim();

						if ( text.Length >= 0 )
						{
							BaseGuild[] guilds = Guild.Search( text );

							if ( guilds.Length > 0 )
							{
								m_Mobile.CloseGump( typeof( ASearchGump ) );

								m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );
							}
						}
					}
				}
			}
			else if ( info.ButtonID == 0 )
			{
				m_Mobile.CloseGump( typeof( ASearchGump ) );

				m_Mobile.SendGump( new SEGuildGump( m_Mobile, m_Guild ) );
			}
			else if ( info.ButtonID == 3 )
			{
				m_Mobile.CloseGump( typeof( ASearchGump ) );

				m_Mobile.SendGump( new SEGuildGump( m_Mobile, m_Guild ) );
			}
			else if ( info.ButtonID == 4 )
			{
				m_Mobile.CloseGump( typeof( ASearchGump ) );

				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 2, "" ) );
			}
			else if ( info.ButtonID == 5 )
			{
				string text = "";
				text = text.Trim();

				if ( text.Length >= 0 )
				{
					BaseGuild[] guilds = Guild.Search( text );

					if ( guilds.Length > 0 )
					{
						m_Mobile.CloseGump( typeof( ASearchGump ) );

						m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );
					}
				}
			}
		}
	}
}