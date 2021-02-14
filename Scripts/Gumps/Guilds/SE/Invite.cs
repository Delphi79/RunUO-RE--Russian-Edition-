using System;
using Server;
using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Factions;

namespace Server.Gumps
{
	public class InviteGump : Gump
	{
		private Mobile m_Mobile, m_Invite;
		private Guild m_Guild, inv_Guild;

		public InviteGump( Mobile from, Mobile invite, Guild guild ) : base( 40, 40 )
		{
			m_Mobile = from;
			m_Invite = invite;
			m_Guild = from.Guild as Guild;
			inv_Guild = guild;

			AddPage( 0 );

			AddBackground( 0, 0, 350, 170, 0x2422 );
			AddHtmlLocalized( 25, 20, 300, 45, 1062946, 0x0, true, false ); // You have been invited to join a guild! (Warning: Accepting will make you attackable!)
			AddHtml( 25, 75, 300, 25, "<center>" + guild.Name + "</center>", true, false );
			AddButton( 265, 130, 0xF7, 0xF8, 2, GumpButtonType.Reply, 0 );
			AddButton( 195, 130, 0xF2, 0xF1, 0, GumpButtonType.Reply, 0 );
			AddCheck( 20, 130, 0xD2, 0xD3, false, 1 );
			AddHtmlLocalized( 45, 130, 150, 30, 1062943, 0x0, false, false ); // Ignore Guild Invites
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( inv_Guild != null )
			{
				if ( info.ButtonID == 0 && info.IsSwitched( 1 ) )
				{
					(m_Mobile as PlayerMobile).AllowGuildInvites = false;
					inv_Guild.Accepted.Remove( m_Mobile );
					m_Invite.SendLocalizedMessage( 1063250, String.Format( "{0}\t{1}\t", m_Mobile.Name, inv_Guild.Name ) ); //  ~1_val~ has declined your invitation to join ~2_val~.
					m_Mobile.SendLocalizedMessage( 1070698 ); // You are now ignoring guild invitations.
				}
				else if ( info.ButtonID == 0 )
				{
					inv_Guild.Accepted.Remove( m_Mobile );
					m_Invite.SendLocalizedMessage( 1063250, String.Format( "{0}\t{1}\t", m_Mobile.Name, inv_Guild.Name ) ); //  ~1_val~ has declined your invitation to join ~2_val~.
				}
				else if ( info.ButtonID == 2 )
				{
					inv_Guild.Accepted.Remove( m_Mobile );
					inv_Guild.AddMember( m_Mobile );
					m_Mobile.Guild = inv_Guild;
					(m_Mobile as PlayerMobile).GuildRank = 1;
					m_Mobile.SendLocalizedMessage( 1063056, inv_Guild.Name ); //  You have joined ~1_val~.
					m_Invite.SendLocalizedMessage( 1063249, String.Format( "{0}\t{1}\t", m_Mobile.Name, inv_Guild.Name ) ); //  ~1_val~ has accepted your invitation to join ~2_val~.
				}
			}
			m_Invite.CloseGump( typeof( InviteGump ) );
		}
	}
}