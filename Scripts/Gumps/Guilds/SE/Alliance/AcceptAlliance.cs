using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Guilds;

namespace Server.Gumps
{
	public class AcceptAllianceGump : Gump
	{
		private Mobile m_Mobile;
		private Guild m_Guild, t_Guild;

		public AcceptAllianceGump( Mobile from, Guild guild ) : base( 10, 40 )
		{
			m_Mobile = from;

			m_Guild = from.Guild as Guild;

			t_Guild = guild;

			AddPage( 0 );

			AddBackground( 0, 0, 520, 335, 0x242C );

			AddHtmlLocalized( 20, 15, 480, 26, 1062975, 0x0, false, false ); // Guild Relationship

			AddImageTiled( 20, 40, 480, 2, 0x2711 );

			AddHtmlLocalized( 20, 50, 120, 26, 1062954, 0x0, true, false ); // Guild Name

			AddHtml( 150, 53, 360, 26, t_Guild.Name, false, false );

			AddHtmlLocalized( 20, 80, 120, 26, 1063025, 0x0, true, false ); // Alliance

			AddHtml( 150, 83, 360, 26, "<basefont color=#000099>" + t_Guild.AllianceName + "</basefont>", false, false );

			AddHtmlLocalized( 20, 110, 120, 26, 1063139, 0x0, true, false ); // Abbreviation

			AddHtml( 150, 113, 120, 26, t_Guild.Abbreviation, false, false );

			AddHtmlLocalized( 280, 110, 120, 26, 1062966, 0x0, true, false ); // Your Kills

			AddHtml( 410, 113, 120, 26, "<basefont color=#black>0/0</basefont>", false, false );

			AddHtmlLocalized( 20, 140, 120, 26, 1062968, 0x0, true, false ); // Time Remaining

			AddHtml( 150, 143, 120, 26, "<basefont color=#black>00:00</basefont>", false, false );

			AddHtmlLocalized( 280, 140, 120, 26, 1062967, 0x0, true, false ); // Their Kills

			AddHtml( 410, 143, 120, 26, "<basefont color=#black>0/0</basefont>", false, false );

			AddImageTiled( 20, 172, 480, 2, 0x2711 );

			AddBackground( 275, 290, 225, 26, 0x2486 );

			AddButton( 280, 295, 0x845, 0x846, 0, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 305, 293, 185, 26, 3000091, 0x0, false, false ); // Cancel

			AddBackground( 20, 290, 225, 26, 0x2486 );

			AddButton( 25, 295, 0x845, 0x846, 2, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 50, 293, 185, 26, 1062987, 0x0, false, false ); // Accept Request

			AddBackground( 20, 260, 225, 26, 0x2486 );

			AddButton( 25, 265, 0x845, 0x846, 1, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 50, 263, 185, 26, 1062988, 0x0, false, false ); // Deny Request

			AddBackground( 148, 215, 225, 26, 0x2486 );

			AddButton( 153, 220, 0x845, 0x846, 3, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 178, 218, 185, 26, 1063164, 0x0, false, false ); // Show Alliance Roster

			AddHtmlLocalized( 20, 180, 480, 30, 1062972, 0x0, true, false ); // This guild has requested an alliance.

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
				case 0: // Cancel
					{
						m_Mobile.CloseGump( typeof( AcceptAllianceGump ) );

						BaseGuild[] guilds = Guild.Search( "" );

						m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );

						break;
					}
				case 1: // Deny Alliance
					{
						if ( m_Rank != 5 )
						{
							m_Mobile.SendLocalizedMessage( 1063436 ); // You don't have permission to negotiate an alliance. 

							return;
						}

						if ( t_Guild != null )
						{
							if ( t_Guild.Allies.Count <= 0 )
							{
								t_Guild.AllianceName = "";
							}

							m_Guild.AllyInvitations.Remove( t_Guild );

							t_Guild.AllyDeclarations.Remove( m_Guild );

							m_Mobile.SendLocalizedMessage( 1070752 ); // The proposal nas been updated.

							m_Mobile.CloseGump( typeof( AcceptAllianceGump ) );
						}

						break;
					}
				case 2: // Accept Alliance
					{
						if ( m_Rank != 5 )
						{
							m_Mobile.SendLocalizedMessage( 1063436 ); // You don't have permission to negotiate an alliance. 

							return;
						}

						if ( t_Guild != null )
						{
							if ( t_Guild.Allies.Count <= 0 )
							{
								t_Guild.AllianceLeader = true;
							}

							m_Guild.AllyInvitations.Remove( t_Guild );

							t_Guild.AllyDeclarations.Remove( m_Guild );

							Guild a_Guild;

							if ( t_Guild.Allies.Count > 0 )
							{
								for ( int i = 0; i < t_Guild.Allies.Count; i++ )
								{
									a_Guild = t_Guild.Allies[ i ] as Guild;

									m_Guild.AddAlly( a_Guild );

									a_Guild.AddAlly( m_Guild );
								}
							}

							m_Guild.AddAlly( t_Guild );

							t_Guild.AddAlly( m_Guild );

							m_Guild.AllianceName = t_Guild.AllianceName;

							m_Mobile.SendLocalizedMessage( 1070760, m_Guild.AllianceName ); // Your Guild has joined the ~1_ALLIANCENAME~ Alliance.

							m_Mobile.SendLocalizedMessage( 1070761, t_Guild.Name ); // A new Guild has joined your Alliance: ~1_GUILDNAME~

							t_Guild.Leader.SendLocalizedMessage( 1070760, t_Guild.AllianceName ); // Your Guild has joined the ~1_ALLIANCENAME~ Alliance.

							t_Guild.Leader.SendLocalizedMessage( 1070761, m_Guild.Name ); //   A new Guild has joined your Alliance: ~1_GUILDNAME~

							m_Mobile.SendLocalizedMessage( 1070752 ); // The proposal nas been updated.
						}

						m_Mobile.CloseGump( typeof( AcceptAllianceGump ) );

						break;
					}
				case 3: // Show Alliance Roster
					{
						if ( m_Guild.Allies.Count > 0 )
						{
							ArrayList guilds = new ArrayList( m_Guild.Allies );

							guilds.Add( m_Guild );

							m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );
						}

						m_Mobile.CloseGump( typeof( AcceptAllianceGump ) );

						break;
					}
			}
		}
	}
}