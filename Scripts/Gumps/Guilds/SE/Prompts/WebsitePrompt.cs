using System;
using Server;
using Server.Guilds;
using Server.Prompts;

namespace Server.Gumps
{
	public class GWebsitePrompt : Prompt
	{
		private Mobile m_Mobile;
		private Guild m_Guild;

		public GWebsitePrompt( Mobile m, Guild g )
		{
			m_Mobile = m;
			m_Guild = g;
		}

		public override void OnCancel( Mobile from )
		{
			if ( m_Guild.BadLeader( m_Mobile ) )
			{
				return;
			}

			m_Mobile.CloseGump( typeof( SEGuildGump ) );

			m_Mobile.SendLocalizedMessage( 1070778 );
		}

		public override void OnResponse( Mobile from, string text )
		{
			if ( m_Guild.BadLeader( m_Mobile ) )
			{
				return;
			}

			Guild g = from.Guild as Guild;

			text = text.Trim();

			if ( text.Length > 50 )
			{
				text = text.Substring( 0, 50 );
			}

			if ( text.Length > 0 )
			{
				g.Website = text;
			}

			m_Mobile.CloseGump( typeof( SEGuildGump ) );

			m_Mobile.SendLocalizedMessage( 1070778 );
		}
	}
}