using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Guilds;
using Server.Network;
using Server.Prompts;
using Server.Targeting;

namespace Server.Gumps
{
	public class DiplomacyMiscGump : Gump
	{
		private Mobile m_Mobile;
		private Guild m_Guild;
		private Guild t_Guild;

		public DiplomacyMiscGump( Mobile leader, Guild target ) : base( 20, 30 )
		{
			m_Mobile = leader;

			t_Guild = target;

			m_Guild = m_Mobile.Guild as Guild;

			AddPage( 0 );

			AddBackground( 0, 0, 520, 335, 0x242C );

			AddBackground( 275, 290, 225, 26, 0x2486 );

			AddBackground( 20, 290, 225, 26, 0x2486 );

			AddBackground( 20, 260, 225, 26, 0x2486 );

			AddHtmlLocalized( 20, 15, 480, 26, 1062975, false, false );

			AddImageTiled( 20, 40, 480, 2, 0x2711 );

			AddHtmlLocalized( 20, 50, 120, 26, 1062954, true, false );

			string name;

			if ( (name = target.Name) != null && (name = name.Trim()).Length <= 0 )
			{
				name = "";
			}

			AddHtml( 150, 53, 360, 26, name, false, false );

			AddHtmlLocalized( 20, 80, 120, 26, 1063025, true, false );

			AddHtml( 150, 83, 360, 26, "<basefont color=#black>" + t_Guild.AllianceName + "</basefont>", false, false );

			AddHtmlLocalized( 20, 110, 120, 26, 1063139, true, false );

			string abbr;

			if ( (abbr = target.Abbreviation) != null && (abbr = abbr.Trim()).Length <= 0 )
			{
				abbr = "";
			}

			AddHtml( 150, 113, 120, 26, abbr, false, false );

			AddHtmlLocalized( 280, 110, 120, 26, 1062966, true, false );

			AddHtml( 410, 113, 120, 26, "<basefont color=#black>0/0</basefont>", false, false );

			AddHtmlLocalized( 20, 140, 120, 26, 1062968, true, false );

			AddHtml( 150, 143, 120, 26, "<basefont color=#black>00:00</basefont>", false, false );

			AddHtmlLocalized( 280, 140, 120, 26, 1062967, true, false );

			AddHtml( 410, 143, 120, 26, "<basefont color=#black>0/0</basefont>", false, false );

			AddImageTiled( 20, 172, 480, 2, 0x2711 );

			AddButton( 280, 295, 0x845, 0x846, 0, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 305, 293, 185, 26, 3000091, false, false );

			AddButton( 25, 295, 0x845, 0x846, 1, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 50, 293, 185, 26, 1062989, false, false );

			AddButton( 25, 265, 0x845, 0x846, 2, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 50, 263, 185, 26, 1062990, false, false );

			if ( m_Guild.IsWar( t_Guild ) )
			{
				AddHtmlLocalized( 20, 180, 480, 30, 1062965, true, false ); // war
			}
			else if ( m_Guild.IsAlly( t_Guild ) )
			{
				AddHtmlLocalized( 20, 180, 480, 30, 1062970, true, false ); // allied
			}
			else
			{
				AddHtmlLocalized( 20, 180, 480, 30, 1062973, true, false ); // peace
			}

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
						m_Mobile.CloseGump( typeof( DiplomacyMiscGump ) );

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
						else
						{
							m_Mobile.CloseGump( typeof( DiplomacyMiscGump ) );

							m_Mobile.SendGump( new DeclareWarGump( m_Mobile, t_Guild ) );
						}

						break;
					}
				case 2:
					{
						if ( m_Rank != 5 )
						{
							m_Mobile.SendLocalizedMessage( 1063436 ); // You don't have permission to negotiate an alliance.

							return;
						}
						else
						{
							if ( t_Guild != null && (t_Guild.Enemies.Count <= 0) && (m_Guild.Enemies.Count <= 0) )
							{
								if ( m_Guild.Allies.Count == 0 && m_Guild.AllyDeclarations.Count == 0 && t_Guild.Allies.Count <= 0 )
								{
									m_Mobile.SendLocalizedMessage( 1063439 ); // Enter a name for the new alliance:

									m_Mobile.Prompt = new GuildAllyPrompt( m_Mobile, t_Guild, m_Guild );
								}
								else
								{
									if ( m_Guild.AllianceLeader && t_Guild.AllyDeclarations.Count == 0 && t_Guild.Allies.Count == 0 )
									{
										if ( !m_Guild.AllyDeclarations.Contains( t_Guild ) )
										{
											m_Guild.AllyDeclarations.Add( t_Guild );

											m_Mobile.SendLocalizedMessage( 1070750, t_Guild.Name );
										}

										if ( !t_Guild.AllyInvitations.Contains( m_Guild ) )
										{
											t_Guild.AllyInvitations.Add( m_Guild );
										}
									}
									else
									{
										m_Mobile.SendLocalizedMessage( 1070748 ); // Failed to create alliance.
									}

									m_Mobile.CloseGump( typeof( DiplomacyMiscGump ) );
								}
							}
						}

						break;
					}
			}
		}
	}
}