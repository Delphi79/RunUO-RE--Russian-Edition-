using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
	public class RosterMissLeader : RosterMiscGump
	{
		public RosterMissLeader( Mobile from, Mobile target, Guild guild ) : base( from, target, guild )
		{
			Design();

			AddHtmlLocalized( 50, 183, 110, 26, 1062993, 0x6400, false, false ); // Promote
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( m_Guild.BadMember( m_Mobile ) )
			{
				return;
			}

			else if ( m_Target.Deleted || !m_Guild.IsMember( m_Target ) )
			{
				return;
			}

			int m_Rank = (m_Mobile as PlayerMobile).GuildRank;
			int t_Rank = (m_Target as PlayerMobile).GuildRank;

			switch ( info.ButtonID )
			{
				case 1: // Cast Vote for This Member
					{
						if ( m_Target != null && (m_Rank > 1) )
						{
							if ( t_Rank == 5 )
							{
								m_Mobile.SendLocalizedMessage( 1063424 ); // You can't vote for the current guild leader.

								return;
							}

							state.Mobile.GuildFealty = m_Target;

							m_Mobile.SendLocalizedMessage( 1063159, m_Target.Name ); // You cast your vote for ~1_val~ for guild leader.
						}
						else
						{
							m_Mobile.SendLocalizedMessage( 1063149 ); // You don't have permission to vote.
						}

						break;
					}
				case 2: // Promote to Leader
					{
						if ( m_Rank == 5 )
						{
							m_Rank = 2;

							t_Rank = 5;

							m_Guild.Leader = m_Target;

							state.Mobile.GuildFealty = m_Target;


							for ( int i = 0; i < m_Guild.Members.Count; ++i )
							{
								((Mobile) m_Guild.Members[ i ]).GuildFealty = m_Target;
							}

							m_Mobile.SendLocalizedMessage( 1063156, m_Mobile.Name ); // The guild information for ~1_val~ has been updated.

							m_Mobile.SendLocalizedMessage( 1063156, m_Target.Name ); // The guild information for ~1_val~ has been updated.
						}

						(m_Target as PlayerMobile).GuildRank = t_Rank;

						(m_Mobile as PlayerMobile).GuildRank = m_Rank;

						break;
					}
				case 3: // Set Guild Title
					{
						if ( (m_Rank == 3 && t_Rank < 3) || (m_Rank == 5) )
						{
							m_Mobile.SendLocalizedMessage( 1011128 ); // Enter the new title for this guild member or 'none' to remove a title:

							m_Mobile.Prompt = new GTitlePrompt( m_Mobile, m_Target, m_Guild );
						}
						else
						{
							m_Mobile.SendLocalizedMessage( 1063148 ); // You don't have permission to change this member's guild title.
						}

						break;
					}
				case 4: // Demote
					{
						if ( m_Target != null )
						{
							if ( t_Rank == 5 )
							{
								m_Mobile.CloseGump( typeof( RosterMiscGump ) );

								m_Mobile.SendLocalizedMessage( 1063146 ); // You don't have permission to demote this member.

								return;
							}
							else if ( m_Rank == 5 && (t_Rank < 5) )
							{
								t_Rank -= 1;

								if ( t_Rank < 1 )
								{
									t_Rank = 1;

									m_Mobile.SendLocalizedMessage( 1063333 ); // You don't have demote Ronin.

									return;
								}
							}
							else if ( m_Rank == 3 )
							{
								if ( t_Rank >= 3 )
								{
									m_Mobile.SendLocalizedMessage( 1063146 ); //  You don't have permission to promote this member.

									return;
								}

								t_Rank -= 1;

								if ( t_Rank < 1 )
								{
									t_Rank = 1;

									m_Mobile.SendLocalizedMessage( 1063333 ); // You don't have demote Ronin.

									return;
								}
							}
							else if ( m_Rank <= 2 )
							{
								m_Mobile.SendLocalizedMessage( 1063146 ); // You don't have permission to demote this member.

								return;
							}
						}

						(m_Target as PlayerMobile).GuildRank = t_Rank;

						m_Mobile.CloseGump( typeof( RosterMiscGump ) );

						m_Mobile.SendLocalizedMessage( 1063156, m_Target.Name ); // The guild information for ~1_val~ has been updated.

						break;
					}
				case 5: // Kick Member
					{
						if ( (m_Rank == 5) || ((m_Rank == 3) && (t_Rank <= 2)) || ((m_Rank >= 2) && (t_Rank == 1)) )
						{
							if ( m_Rank == t_Rank )
							{
								m_Mobile.SendLocalizedMessage( 1063151 ); // You don't have permission to remove this member.

								return;
							}

							m_Mobile.CloseGump( typeof( RosterMiscGump ) );

							(m_Target as PlayerMobile).GuildRank = 0;

							m_Guild.RemoveMember( m_Target );
						}
						else
						{
							m_Mobile.SendLocalizedMessage( 1063151 ); // You don't have permission to remove this member.
						}

						break;
					}
				case 6: // Clear Vote For This Member
					{
						if ( m_Target != null && (m_Rank > 1) )
						{
							if ( m_Guild.Leader != null )
							{
								state.Mobile.GuildFealty = m_Guild.Leader;
							}
							else
							{
								state.Mobile.GuildFealty = m_Mobile;
							}

							m_Mobile.SendLocalizedMessage( 1063158 ); // You have cleared your vote for guild leader.	
						}

						break;
					}
			}
		}
	}
}