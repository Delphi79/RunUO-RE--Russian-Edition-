using System;
using Server;
using System.Collections;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class RosterGump : GMobileList
	{
		public RosterGump( Mobile from, Guild guild, int Sort_Type, string FindText ) : base( from, guild, guild.Members, Sort_Type, FindText )
		{
		}

		protected override void Design()
		{
			AddButton( 71, 45, 0x845, 0x846, 1000, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 96, 43, 110, 26, 1063014, false, false );

			AddButton( 241, 45, 0x845, 0x846, 2000, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 266, 43, 110, 26, 1062974, 0xF, false, false );

			AddButton( 406, 45, 0x845, 0x846, 3000, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 431, 43, 110, 26, 1062978, false, false );

			AddTextEntry( 135, 80, 375, 30, 0, 1, "" );

			AddButton( 520, 75, 0x867, 0x868, 4000, GumpButtonType.Reply, 0 );
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
				Mobile m = (Mobile) m_List[ i ];

				if ( m != null && !m.Deleted )
				{
					m_Mobile.CloseGump( typeof( RosterGump ) );
					m_Mobile.SendGump( new RosterMiscGump( m_Mobile, m, m_Guild ) );
				}
			}
			if ( info.ButtonID == 1000 ) // My Guild
			{
				m_Mobile.CloseGump( typeof( RosterGump ) );
				m_Mobile.SendGump( new SEGuildGump( m_Mobile, m_Guild ) );
			}
			else if ( info.ButtonID == 2000 ) // Roster 
			{
				m_Mobile.CloseGump( typeof( RosterGump ) );
				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 2, "" ) );
			}
			else if ( info.ButtonID == 3000 ) // Diplomacy
			{
				BaseGuild[] guilds = Guild.Search( "" );
				if ( guilds.Length > 0 )
				{
					m_Mobile.CloseGump( typeof( RosterGump ) );
					m_Mobile.SendGump( new DiplomacyGump( m_Mobile, m_Guild, new ArrayList( guilds ), 2 ) );
				}
			}
			else if ( info.ButtonID == 4000 ) // Search Button
			{
				string text = info.GetTextEntry( 1 ).Text;

				text = text.Trim();

				if ( text.Length >= 3 )
				{
					m_Mobile.CloseGump( typeof( RosterGump ) );
					m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 4, text ) );
				}
				else
				{
					m_Mobile.SendMessage( "Search string must be at least three letters in length." );
				}
			}
			else if ( info.ButtonID == 5000 ) // Sort for Name
			{
				m_Mobile.CloseGump( typeof( RosterGump ) );
				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 10, "" ) );
			}
			else if ( info.ButtonID == 6000 ) // Sort for Rank
			{
				m_Mobile.CloseGump( typeof( RosterGump ) );
				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 11, "" ) );
			}
			else if ( info.ButtonID == 7000 ) // Sort for LastOn
			{
				m_Mobile.CloseGump( typeof( RosterGump ) );
				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 12, "" ) );
			}
			else if ( info.ButtonID == 8000 ) // Sort for Guild Title
			{
				m_Mobile.CloseGump( typeof( RosterGump ) );
				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 13, "" ) );
			}
			else if ( info.ButtonID == 5001 ) // Sort for Name
			{
				m_Mobile.CloseGump( typeof( RosterGump ) );
				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 0, "" ) );
			}
			else if ( info.ButtonID == 6001 ) // Sort for Rank
			{
				m_Mobile.CloseGump( typeof( RosterGump ) );
				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 1, "" ) );
			}
			else if ( info.ButtonID == 7001 ) // Sort for LastOn
			{
				m_Mobile.CloseGump( typeof( RosterGump ) );
				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 2, "" ) );
			}
			else if ( info.ButtonID == 8001 ) // Sort for Guild Title
			{
				m_Mobile.CloseGump( typeof( RosterGump ) );
				m_Mobile.SendGump( new RosterGump( m_Mobile, m_Guild, 3, "" ) );
			}
			else if ( info.ButtonID == 9000 ) // Invite Player
			{
				if ( (m_Guild.Leader == m_Mobile) || ((m_Mobile as PlayerMobile).GuildRank == 3) )
				{
					m_Mobile.Target = new InviteTarget( m_Mobile, m_Guild );
					m_Mobile.SendLocalizedMessage( 1063048 ); //  Whom do you wish to invite into your guild?
				}
				else
				{
					m_Mobile.SendLocalizedMessage( 503301 ); //	 You don't have permission to do that.
				}
				m_Mobile.CloseGump( typeof( RosterGump ) );
			}
		}
	}
}