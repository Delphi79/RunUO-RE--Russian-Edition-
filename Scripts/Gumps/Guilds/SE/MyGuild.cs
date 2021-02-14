using System;
using System.Collections;
using Server;
using Server.Guilds;
using Server.Factions;
using Server.Network;
using Server.Prompts;
using Server.Targeting;

namespace Server.Gumps
{
	public class MyGuildGump : Gump
	{
		protected Mobile m_Mobile;
		protected Guild m_Guild;
		protected Faction m_Faction;
		protected const string DefaultWebsite = "Guild website is not yet set";
		protected const string DefaultCharter = "The guild leader has not yet set the guild charter.";

		public MyGuildGump( Mobile from, Guild guild ) : base( 30, 40 )
		{
			m_Mobile = from;
			m_Guild = guild;

			Guild g = from.Guild as Guild;
			Mobile leader = g.Leader;
			string gname = g.Name;

			Dragable = true;

			AddPage( 0 );
			AddBackground( 0, 0, 600, 440, 0x24AE );
			AddBackground( 66, 40, 150, 26, 0x2486 );
			AddBackground( 236, 40, 150, 26, 0x2486 );
			AddBackground( 401, 40, 150, 26, 0x2486 );
			AddBackground( 450, 370, 100, 26, 0x2486 );

			Design();

			AddButton( 71, 45, 0x845, 0x846, 10, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 96, 43, 110, 26, 1063014, 0xF, false, false );
			AddButton( 241, 45, 0x845, 0x846, 20, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 266, 43, 110, 26, 1062974, false, false );
			AddButton( 406, 45, 0x845, 0x846, 30, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 431, 43, 110, 26, 1062978, false, false );

			AddImageTiled( 65, 80, 160, 26, 0xA40 );
			AddImageTiled( 67, 82, 156, 22, 0xBBC );
			AddHtmlLocalized( 70, 83, 150, 20, 1062954, false, false );
			AddHtml( 233, 84, 320, 26, gname, false, false );
			AddImageTiled( 65, 114, 160, 26, 0xA40 );
			AddImageTiled( 67, 116, 156, 22, 0xBBC );
			AddHtmlLocalized( 70, 117, 150, 20, 1063025, false, false );
			AddHtml( 233, 118, 320, 26, g.AllianceName, false, false );

			if ( g.AllianceName.Length > 0 )
			{
				AddButton( 40, 120, 0x4B9, 0x4BA, 200, GumpButtonType.Reply, 0 ); // Button on Alliance Roster
			}

			AddImageTiled( 65, 148, 160, 26, 0xA40 );
			AddImageTiled( 67, 150, 156, 22, 0xBBC );
			AddHtmlLocalized( 70, 151, 150, 20, 1063084, false, false );

			Faction faction = Faction.Find( m_Guild.Leader as Mobile );

			if ( faction != null )
			{
				AddHtml( 233, 152, 320, 26, faction.ToString(), false, false ); // Guild Faction
			}
			AddHtml( 233, 152, 320, 26, "", false, false ); // Guild Faction
			AddImageTiled( 65, 196, 480, 4, 0x238D );

			string charter;

			if ( (charter = g.Charter) == null || (charter = charter.Trim()).Length <= 0 )
			{
				charter = DefaultCharter;
			}

			AddHtml( 65, 216, 480, 80, charter, true, true );

			if ( from != null && from == leader )
			{
				AddButton( 40, 251, 0x4B9, 0x4BA, 40, GumpButtonType.Reply, 0 );
			}

			string website;

			if ( (website = g.Website) == null || (website = website.Trim()).Length <= 0 )
			{
				website = DefaultWebsite;
			}

			AddHtml( 65, 306, 480, 30, website, true, false );

			if ( from != null && from == leader )
			{
				AddButton( 40, 313, 0x4B9, 0x4BA, 50, GumpButtonType.Reply, 0 );
			}

			AddButton( 65, 370, 0xD2, 0xD3, 100, GumpButtonType.Reply, 0 );
			AddCheck( 65, 370, 0xD2, 0xD3, from.DisplayGuildTitle, 100 );

			AddHtmlLocalized( 95, 370, 150, 26, 1063085, false, false );
			AddButton( 455, 375, 0x845, 0x846, 60, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 480, 373, 60, 26, 3006115, false, false );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Guild.BadMember( m_Mobile ) )
			{
				return;
			}

			switch ( info.ButtonID )
			{
				case 10: // -- Open My Guild Gump
					{
						m_Mobile.CloseGump( typeof( SEGuildGump ) );
						m_Mobile.SendGump( new SEGuildGump( m_Mobile, m_Guild ) );
						break;
					}
				case 20: // -- Open Guild Member List
					{
						m_Mobile.CloseGump( typeof( SEGuildGump ) );
						m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 2, "" ) );
						break;
					}
				case 30: // -- Open Diplomancy Gump
					{
						BaseGuild[] guilds = Guild.Search( "" );
						if ( guilds.Length > 0 )
						{
							m_Mobile.CloseGump( typeof( SEGuildGump ) );
							m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );
						}
						break;
					}
				case 40: // -- Set Guild Charter
					{
						m_Mobile.SendLocalizedMessage( 1013071 ); // Enter the new guild charter (50 characters max):
						m_Mobile.Prompt = new GCharterPrompt( m_Mobile, m_Guild );
						break;
					}
				case 50: // -- Set Guild Website
					{
						m_Mobile.SendLocalizedMessage( 1013072 ); // Enter the new website for the guild (50 characters max):
						m_Mobile.Prompt = new GWebsitePrompt( m_Mobile, m_Guild );
						break;
					}
				case 60: // -- Resgn from Guild
					{
						m_Mobile.SendGump( new ResignGump( m_Mobile, m_Guild ) );
						m_Mobile.SendLocalizedMessage( 1063332 ); // Are you sure you wish to resign from your guild?
						break;
					}
				case 100: // -- Show Guild Title
					{
						m_Mobile.DisplayGuildTitle = !m_Mobile.DisplayGuildTitle;
						m_Mobile.SendGump( new SEGuildGump( m_Mobile, m_Guild ) );
						break;
					}
				case 200: // -- Show Alliance
					{
						m_Mobile.CloseGump( typeof( DiplomacyGump ) );

						if ( m_Guild.Allies.Count > 0 )
						{
							ArrayList guilds = new ArrayList( m_Guild.Allies );
							guilds.Add( m_Guild );
							m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );
						}
						break;
					}
			}
		}

		protected virtual void Design()
		{
		}
	}
}