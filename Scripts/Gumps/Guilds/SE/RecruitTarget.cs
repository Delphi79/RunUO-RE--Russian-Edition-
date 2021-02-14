using System;
using Server;
using Server.Guilds;
using Server.Targeting;
using Server.Mobiles;
using Server.Factions;

namespace Server.Gumps
{
	public class InviteTarget : Target
	{
		private Mobile m_Mobile;
		private Guild m_Guild;

		public InviteTarget( Mobile m, Guild guild ) : base( 10, false, TargetFlags.None )
		{
			m_Mobile = m;
			m_Guild = guild;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			int m_Rank = (from as PlayerMobile).GuildRank;

			if ( m_Rank != 3 && m_Rank != 5 )
			{
				return;
			}

			if ( targeted is Mobile )
			{
				Mobile m = (Mobile) targeted;

				PlayerState guildState = PlayerState.Find( m_Guild.Leader );
				PlayerState targetState = PlayerState.Find( m );

				Faction guildFaction = (guildState == null ? null : guildState.Faction);
				Faction targetFaction = (targetState == null ? null : targetState.Faction);

				if ( !m.Player )
				{
					m_Mobile.SendLocalizedMessage( 1063334 ); // That isn't a valid player.
				}
				else if ( !m.Alive )
				{
					m_Mobile.SendLocalizedMessage( 501162 ); // Only the living may be recruited.   // need test ?
				}
				else if ( m_Mobile == m )
				{
					m_Mobile.SendLocalizedMessage( 502128 ); // You flatter yourself.
				}
				else if ( !(m as PlayerMobile).AllowGuildInvites )
				{
					m_Mobile.SendLocalizedMessage( 1063049, m.Name ); // ~1_val~ is not accepting guild invitations.
				}
				else if ( m_Guild.IsMember( m ) )
				{
					m_Mobile.SendLocalizedMessage( 1063050, m.Name ); // ~1_val~ is already a member of your guild!
				}
				else if ( m_Guild.Accepted.Contains( m ) )
				{
					m_Mobile.SendLocalizedMessage( 1063052, m.Name ); // ~1_val~ is currently considering another guild invitation.
				}
				else if ( m.Guild != null )
				{
					m_Mobile.SendLocalizedMessage( 1063051, m.Name ); // ~1_val~ is already a member of a guild.
				}
				#region Factions
				else if ( guildFaction != targetFaction )
				{
					if ( guildFaction == null )
					{
						m_Mobile.SendLocalizedMessage( 1013027 ); // That player cannot join a non-faction guild.
					}
					else if ( targetFaction == null )
					{
						m_Mobile.SendLocalizedMessage( 1013026 ); // That player must be in a faction before joining this guild.
					}
					else
					{
						m_Mobile.SendLocalizedMessage( 1013028 ); // That person has a different faction affiliation.
					}
				}
				else if ( targetState != null && targetState.IsLeaving )
				{
					// OSI does this quite strangely, so we'll just do it this way
					m_Mobile.SendMessage( "That person is quitting their faction and so you may not recruit them." );
				}
				#endregion
				else
				{
					m_Guild.Accepted.Add( m );
					m.SendGump( new InviteGump( m, m_Mobile, m_Guild ) );
				}
			}
			else
			{
				m_Mobile.SendLocalizedMessage( 1063334 ); // That isn't a valid player.
			}
		}

		protected override void OnTargetFinish( Mobile from )
		{
			int m_Rank = (from as PlayerMobile).GuildRank;

			if ( m_Rank != 3 && m_Rank != 5 )
			{
				return;
			}

			m_Mobile.CloseGump( typeof( RosterGump ) );
		}
	}
}