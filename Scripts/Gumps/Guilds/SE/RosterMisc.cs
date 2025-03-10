using System;
using System.Collections;
using Server;
using Server.Guilds;
using Server.Network;
using Server.Prompts;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Gumps
{
	public class RosterMiscGump : Gump
	{
		protected Mobile m_Mobile, m_Target;
		protected Guild m_Guild;

		public RosterMiscGump( Mobile from, Mobile target, Guild guild ) : base( 20, 30 )
		{
			m_Mobile = from;
			m_Guild = from.Guild as Guild;
			m_Target = target;

			int m_Rank = (m_Mobile as PlayerMobile).GuildRank;

			int t_Rank = (m_Target as PlayerMobile).GuildRank;

			AddPage( 0 );

			AddBackground( 0, 0, 350, 255, 0x242C );

			AddImageTiled( 20, 40, 310, 2, 0x2711 );

			AddBackground( 20, 150, 310, 26, 0x2486 );

			AddBackground( 20, 180, 150, 26, 0x2486 );

			AddBackground( 180, 180, 150, 26, 0x2486 );

			AddBackground( 20, 210, 150, 26, 0x2486 );

			AddBackground( 180, 210, 150, 26, 0x2486 );

			AddHtmlLocalized( 20, 15, 310, 26, 1063018, false, false );

			AddHtmlLocalized( 20, 50, 150, 26, 1062955, true, false );

			string name;

			if ( (name = target.Name) != null && (name = name.Trim()).Length <= 0 )
			{
				name = "";
			}

			Mobile fealty = from.GuildFealty;

			if ( fealty == null || !guild.IsMember( fealty ) )
			{
				fealty = m_Guild.Leader;
			}

			if ( (fealty == target) && (target != m_Guild.Leader) )
			{
				AddHtml( 180, 53, 150, 26, "" + name + " *", false, false );
				AddButton( 25, 155, 0x845, 0x846, 6, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 50, 153, 270, 26, 1063082, false, false );
			}
			else
			{
				AddHtml( 180, 53, 150, 26, name, false, false );
				AddButton( 25, 155, 0x845, 0x846, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 50, 153, 270, 26, 1062996, false, false );
			}

			AddHtmlLocalized( 20, 80, 150, 26, 1062956, true, false );

			if ( target != null )
			{
				if ( t_Rank == 5 )
				{
					AddHtmlLocalized( 180, 83, 150, 26, 1062959, false, false ); // Guild Leader
				}
				else if ( t_Rank == 4 )
				{
					AddHtmlLocalized( 180, 83, 150, 26, 1062960, false, false ); // Warlord
				}
				else if ( t_Rank == 3 )
				{
					AddHtmlLocalized( 180, 83, 150, 26, 1062961, false, false ); // Emissary
				}
				else if ( t_Rank == 2 )
				{
					AddHtmlLocalized( 180, 83, 150, 26, 1062962, false, false ); // Member
				}
				else
				{
					AddHtmlLocalized( 180, 83, 150, 26, 1062963, false, false ); // Ronin
				}
			}
			AddHtmlLocalized( 20, 110, 150, 26, 1062953, true, false );

			string title;

			if ( (title = target.GuildTitle) != null && (title = title.Trim()).Length <= 0 )
			{
				title = "";
			}

			AddHtml( 180, 113, 150, 26, title, false, false );

			AddImageTiled( 20, 142, 310, 2, 0x2711 );

			AddButton( 25, 185, 0x845, 0x846, 2, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 50, 183, 110, 26, 1062993, false, false );

			AddButton( 185, 185, 0x845, 0x846, 3, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 210, 183, 110, 26, 1062995, false, false );

			AddButton( 25, 215, 0x845, 0x846, 4, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 50, 213, 110, 26, 1062994, false, false );

			AddButton( 185, 215, 0x845, 0x846, 5, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 210, 213, 110, 26, 1062997, false, false );
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
				case 2: // Promote
					{
						if ( m_Target != null )
						{
							if ( m_Rank == 5 && (t_Rank == 5) )
							{
								m_Mobile.CloseGump( typeof( RosterMiscGump ) );
								m_Mobile.SendMessage( 0, "Invalid Rank" );
								return;
							}
							if ( m_Rank == 5 && (t_Rank != 5) )
							{
								t_Rank += 1;

								if ( t_Rank == 5 )
								{
									m_Mobile.CloseGump( typeof( RosterMiscGump ) );
									m_Mobile.SendGump( new RosterMissLeader( m_Mobile, m_Target, m_Guild ) );
									m_Mobile.SendLocalizedMessage( 1063144 ); // Are you sure you wish to make this member the new guild leader?
								}
								else
								{
									(m_Target as PlayerMobile).GuildRank = t_Rank;
									(m_Mobile as PlayerMobile).GuildRank = m_Rank;
								}
							}
							else if ( m_Rank == 3 )
							{
								if ( t_Rank >= 3 )
								{
									m_Mobile.SendLocalizedMessage( 1063143 ); //  You don't have permission to promote this member.
									return;
								}

								t_Rank += 1;

								if ( t_Rank > 2 )
								{
									t_Rank = 2;
									m_Mobile.SendLocalizedMessage( 1063143 ); //  You don't have permission to promote this member.
									return;
								}
								(m_Target as PlayerMobile).GuildRank = t_Rank;
								(m_Mobile as PlayerMobile).GuildRank = m_Rank;
							}
							else if ( m_Rank <= 2 || t_Rank == 4 )
							{
								m_Mobile.SendLocalizedMessage( 1063143 ); //  You don't have permission to promote this member.
								return;
							}

							m_Mobile.CloseGump( typeof( RosterMiscGump ) );
						}

						m_Mobile.SendLocalizedMessage( 1063156, m_Target.Name ); // The guild information for ~1_val~ has been updated.

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
							else if ( m_Rank <= 2 || (m_Rank == 4) )
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

		protected virtual void Design()
		{
		}
	}

}