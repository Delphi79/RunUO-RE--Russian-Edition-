using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Guilds;
using Server.Network;
using System.IO;

namespace Server.Gumps
{
	public abstract class GMobileList : Gump
	{
		protected Mobile m_Mobile;
		protected Guild m_Guild;
		protected ArrayList m_List;
		protected ArrayList m_SortList;
		protected bool updown;

		public GMobileList( Mobile from, Guild guild, ArrayList list, int Sort_Type, string FindText ) : base( 30, 40 )
		{
			m_Mobile = from;
			m_Guild = guild;

			Dragable = true;

			AddPage( 0 );

			if ( Sort_Type >= 10 )
			{
				updown = true;
				Sort_Type = Sort_Type - 10;
			}
			else
			{
				updown = false;
			}

			AddBackground( 0, 0, 600, 440, 0x24AE );
			AddBackground( 66, 40, 150, 26, 0x2486 );
			AddBackground( 236, 40, 150, 26, 0x2486 );
			AddBackground( 401, 40, 150, 26, 0x2486 );
			AddBackground( 130, 75, 385, 30, 0xBB8 );

			AddImageTiled( 65, 110, 140, 26, 0xA40 );
			AddImageTiled( 67, 112, 136, 22, 0xBBC );
			AddHtmlLocalized( 70, 113, 130, 20, 1062955, false, false );

			if ( Sort_Type == 0 ) // Sort by Name
			{
				if ( updown )
				{
					AddButton( 189, 117, 0x983, 0x984, 5001, GumpButtonType.Reply, 0 );
				}
				else
				{
					AddButton( 189, 117, 0x985, 0x986, 5000, GumpButtonType.Reply, 0 );
				}
			}
			else
			{
				AddButton( 189, 117, 0x2716, 0x2716, 5000, GumpButtonType.Reply, 0 );
			}

			AddImageTiled( 207, 110, 90, 26, 0xA40 );
			AddImageTiled( 209, 112, 86, 22, 0xBBC );
			AddHtmlLocalized( 212, 113, 80, 20, 1062956, false, false );

			if ( Sort_Type == 1 ) // Sort by Rank
			{
				if ( updown )
				{
					AddButton( 281, 117, 0x983, 0x984, 6001, GumpButtonType.Reply, 0 );
				}
				else
				{
					AddButton( 281, 117, 0x985, 0x986, 6000, GumpButtonType.Reply, 0 );
				}
			}
			else
			{
				AddButton( 281, 117, 0x2716, 0x2716, 6000, GumpButtonType.Reply, 0 );
			}

			AddImageTiled( 299, 110, 90, 26, 0xA40 );
			AddImageTiled( 301, 112, 86, 22, 0xBBC );
			AddHtmlLocalized( 304, 113, 80, 20, 1062952, false, false );

			if ( Sort_Type == 2 ) // Sort by LastOn
			{
				if ( updown )
				{
					AddButton( 374, 117, 0x983, 0x984, 7001, GumpButtonType.Reply, 0 );
				}
				else
				{
					AddButton( 374, 117, 0x985, 0x986, 7000, GumpButtonType.Reply, 0 );
				}
			}
			else
			{
				AddButton( 374, 117, 0x2716, 0x2716, 7000, GumpButtonType.Reply, 0 );
			}

			AddImageTiled( 391, 110, 160, 26, 0xA40 );
			AddImageTiled( 393, 112, 156, 22, 0xBBC );
			AddHtmlLocalized( 396, 113, 150, 20, 1062953, false, false );

			if ( Sort_Type == 3 ) // Sort by Guild Title
			{
				if ( updown )
				{
					AddButton( 535, 117, 0x983, 0x984, 8001, GumpButtonType.Reply, 0 );
				}
				else
				{
					AddButton( 535, 117, 0x985, 0x986, 8000, GumpButtonType.Reply, 0 );
				}
			}
			else
			{
				AddButton( 535, 117, 0x2716, 0x2716, 8000, GumpButtonType.Reply, 0 );
			}

			Design();

			m_List = new ArrayList();

			for ( int j1 = 0; j1 < list.Count; j1++ )
			{
				m_List.Add( (Mobile) list[ j1 ] );
			}

			m_SortList = new ArrayList();

			switch ( Sort_Type )
			{
				case 0:
					{
						m_List.Sort( new ListNameSorter( updown ) );
						break;
					}
				case 1:
					{
						m_List.Sort( new ListGRSorter( updown ) );
						break;
					}
				case 2:
					{
						int j = 0;
						while ( j < m_List.Count )
						{
							if ( ((Mobile) m_List[ j ]).NetState != null )
							{
								m_SortList.Add( m_List[ j ] );
								m_List.Remove( m_List[ j ] );
							}
							else
							{
								j++;
							}
						}

						m_List.Sort( new ListLastOnSorter( updown ) );

						for ( j = 0; j < m_List.Count; j++ )
						{
							m_SortList.Add( m_List[ j ] );
						}
						m_List = m_SortList;

						break;
					}
				case 3:
					{
						m_List.Sort( new ListGTSorter( updown ) );

						break;
					}

				case 4:
					{
						FindText = FindText.ToLower();
						int j2 = 0;
						while ( j2 < m_List.Count )
						{
							if ( ((Mobile) m_List[ j2 ]).Name.ToLower().IndexOf( FindText ) < 0 )
							{
								m_List.Remove( m_List[ j2 ] );
							}
							else
							{
								j2++;
							}
						}

						m_List.Sort( new ListNameSorter( updown ) );

						break;
					}
			}

			for ( int i = 0; i < m_List.Count; ++i )
			{
				if ( (i%8) == 0 )
				{
					AddBackground( 225, 372, 150, 26, 0x2486 );
					AddButton( 230, 377, 0x845, 0x846, 9000, GumpButtonType.Reply, 0 );
					AddHtmlLocalized( 255, 375, 110, 26, 1062992, false, false );

					AddButton( 95, 80, 0x15E1, 0x15E5, 0, GumpButtonType.Page, (i/8) + 1 );
					AddPage( (i/8) + 1 );
					AddButton( 65, 80, 0x15E3, 0x15E7, 0, GumpButtonType.Page, (i/8) );
				}

				Mobile m = (Mobile) m_List[ i ];

				AddButton( 40, 143 + ((i%8)*28), 0x4B9, 0x4BA, i + 1, GumpButtonType.Reply, 0 );

				AddImageTiled( 65, 138 + ((i%8)*28), 140, 26, 0xA40 );
				AddImageTiled( 67, 140 + ((i%8)*28), 136, 22, 0xBBC );

				Mobile fealty = from.GuildFealty;

				if ( fealty == null || !guild.IsMember( fealty ) )
				{
					fealty = guild.Leader;
				}

				NetState ns = ((Mobile) m_List[ i ]).NetState;

				string name;

				if ( (name = m.Name) != null && (name = name.Trim()).Length <= 0 )
				{
					name = "";
				}

				if ( name == from.Name )
				{
					if ( fealty == from && (from != guild.Leader) )
					{
						AddHtml( 70, 141 + ((i%8)*28), 130, 20, "<basefont color=#006600>" + name + " *</basefont>", false, false );
					}
					else
					{
						AddHtml( 70, 141 + ((i%8)*28), 130, 20, "<basefont color=#006600>" + name + "</basefont>", false, false );
					}
				}
				if ( ns != null && (m != from) )
				{
					if ( m == fealty && (m != guild.Leader) )
					{
						AddHtml( 70, 141 + ((i%8)*28), 130, 20, "<basefont color=#0000CC>" + name + " *</basefont>", false, false );
					}
					else
					{
						AddHtml( 70, 141 + ((i%8)*28), 130, 20, "<basefont color=#0000CC>" + name + "</basefont>", false, false );
					}
				}
				if ( ns == null && (m != from) )
				{
					if ( m == fealty && (m != guild.Leader) )
					{
						AddHtml( 70, 141 + ((i%8)*28), 130, 20, "" + name + " *", false, false );
					}
					else
					{
						AddHtml( 70, 141 + ((i%8)*28), 130, 20, name, false, false );
					}
				}

				AddImageTiled( 207, 138 + ((i%8)*28), 90, 26, 0xA40 );
				AddImageTiled( 209, 140 + ((i%8)*28), 86, 22, 0xBBC );

				int rank = 1062963;

				switch ( (m as PlayerMobile).GuildRank )
				{
					case 1:
						rank = 1062963; // Ronin
						break;
					case 2:
						rank = 1062962; // Member
						break;
					case 3:
						rank = 1062961; // Emissary
						break;
					case 4:
						rank = 1062960; // Warlord
						break;
					case 5:
						rank = 1062959; // Guild Leader
						break;
				}
				AddHtmlLocalized( 212, 141 + ((i%8)*28), 80, 20, rank, false, false );

				AddImageTiled( 299, 138 + ((i%8)*28), 90, 26, 0xA40 );
				AddImageTiled( 301, 140 + ((i%8)*28), 86, 22, 0xBBC );

				AddImageTiled( 391, 138 + ((i%8)*28), 160, 26, 0xA40 );
				AddImageTiled( 393, 140 + ((i%8)*28), 156, 22, 0xBBC );


				string title;

				if ( (title = m.GuildTitle) != null && (title = title.Trim()).Length <= 0 )
				{
					title = "";
				}

				AddHtml( 396, 141 + ((i%8)*28), 150, 20, title, false, false ); // Guild Title

				if ( ns != null )
				{
					AddHtmlLocalized( 304, 141 + ((i%8)*28), 80, 20, 1063015, false, false ); // Online or LastOn
				}
				else
				{
					string laston = null;
					laston = ((Mobile) m as PlayerMobile).m_LastLogin.ToString();
					AddHtml( 304, 141 + ((i%8)*28), 80, 20, laston, false, false );
					continue;
				}
			}
		}

		private class ListNameSorter : IComparer
		{
			private bool Dsort;

			public ListNameSorter( bool descend ) : base()
			{
				Dsort = descend;
			}

			public int Compare( object x, object y )
			{
				string xstr = null;
				string ystr = null;

				xstr = ((Mobile) x).Name;
				ystr = ((Mobile) y).Name;

				if ( Dsort )
				{
					return String.Compare( ystr, xstr, true );
				}
				else
				{
					return String.Compare( xstr, ystr, true );
				}
			}
		}

		private class ListLastOnSorter : IComparer
		{
			private bool Dsort;

			public ListLastOnSorter( bool descend ) : base()
			{
				Dsort = descend;
			}

			public int Compare( object x, object y )
			{
				string xstr = null;
				string ystr = null;

				xstr = ((Mobile) x as PlayerMobile).m_LastLogin.ToString();
				ystr = ((Mobile) y as PlayerMobile).m_LastLogin.ToString();

				if ( Dsort )
				{
					return String.Compare( ystr, xstr, true );
				}
				else
				{
					return String.Compare( xstr, ystr, true );
				}
			}
		}

		private class ListGTSorter : IComparer
		{
			private bool Dsort;

			public ListGTSorter( bool descend ) : base()
			{
				Dsort = descend;
			}

			public int Compare( object x, object y )
			{
				string xstr = null;
				string ystr = null;

				xstr = ((Mobile) x).GuildTitle;
				ystr = ((Mobile) y).GuildTitle;

				if ( Dsort )
				{
					return String.Compare( ystr, xstr, true );
				}
				else
				{
					return String.Compare( xstr, ystr, true );
				}
			}
		}

		private class ListGRSorter : IComparer
		{
			private bool Dsort;

			public ListGRSorter( bool descend ) : base()
			{
				Dsort = descend;
			}

			public int Compare( object x, object y )
			{
				string xstr = ((Mobile) x as PlayerMobile).GuildRank.ToString();
				string ystr = ((Mobile) y as PlayerMobile).GuildRank.ToString();

				if ( Dsort )
				{
					return String.Compare( ystr, xstr, true );
				}
				else
				{
					return String.Compare( xstr, ystr, true );
				}
			}
		}

		protected virtual void Design()
		{
		}
	}
}