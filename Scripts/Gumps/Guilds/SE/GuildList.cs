using System;
using System.Collections;
using Server;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
	public abstract class GListGump : Gump
	{
		protected Mobile m_Mobile;
		protected Guild m_Guild;
		protected ArrayList m_List;
		protected ArrayList m_SortAllyList;
		protected ArrayList m_SortWarList;
		protected bool updown;

		public GListGump( Mobile from, Guild guild, ArrayList list, int Sort_Type ) : base( 30, 40 )
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
			AddBackground( 350, 372, 200, 26, 0x2486 );


			AddImageTiled( 65, 110, 290, 26, 0xA40 );
			AddImageTiled( 67, 112, 286, 22, 0xBBC );
			AddHtmlLocalized( 70, 113, 280, 20, 1062954, false, false );

			if ( Sort_Type == 0 ) // Sort by Name
			{
				if ( updown )
				{
					AddButton( 339, 117, 0x983, 0x984, 1101, GumpButtonType.Reply, 0 );
				}
				else
				{
					AddButton( 339, 117, 0x985, 0x986, 1100, GumpButtonType.Reply, 0 );
				}
			}
			else
			{
				AddButton( 339, 117, 0x2716, 0x2716, 1100, GumpButtonType.Reply, 0 );
			}

			AddImageTiled( 357, 110, 60, 26, 0xA40 );
			AddImageTiled( 359, 112, 56, 22, 0xBBC );
			AddHtmlLocalized( 362, 113, 50, 20, 1062957, false, false );

			if ( Sort_Type == 1 ) // Sort by Abbreviation
			{
				if ( updown )
				{
					AddButton( 401, 117, 0x983, 0x984, 1201, GumpButtonType.Reply, 0 );
				}
				else
				{
					AddButton( 401, 117, 0x985, 0x986, 1200, GumpButtonType.Reply, 0 );
				}
			}
			else
			{
				AddButton( 401, 117, 0x2716, 0x2716, 1200, GumpButtonType.Reply, 0 );
			}

			AddImageTiled( 419, 110, 130, 26, 0xA40 );
			AddImageTiled( 421, 112, 126, 22, 0xBBC );
			AddHtmlLocalized( 424, 113, 120, 20, 1062958, false, false );

			if ( Sort_Type == 2 ) // Sort by Status
			{
				if ( updown )
				{
					AddButton( 534, 117, 0x983, 0x984, 1301, GumpButtonType.Reply, 0 );
				}
				else
				{
					AddButton( 534, 117, 0x985, 0x986, 1300, GumpButtonType.Reply, 0 );
				}
			}
			else
			{
				AddButton( 534, 117, 0x2716, 0x2716, 1300, GumpButtonType.Reply, 0 );
			}

			if ( Sort_Type == 3 )
			{
				AddHtmlLocalized( 66, 377, 280, 26, 1063138, 0xF, false, false );
			}
			else if ( Sort_Type == 4 )
			{
				AddHtmlLocalized( 66, 377, 280, 26, 1063137, 0xF, false, false );
			}
			else
			{
				AddHtmlLocalized( 66, 377, 280, 26, 1063136, 0xF, false, false );
			}

			Design();

			m_List = new ArrayList( list.Count );
			m_List = list;

			switch ( Sort_Type )
			{
				case 0:
					{
						m_List.Sort( new ListNameSorter( updown ) );
						break;
					}
				case 1:
					{
						m_List.Sort( new ListAbbrSorter( updown ) );
						break;
					}
				case 2:
					{
						m_SortWarList = new ArrayList();
						m_SortAllyList = new ArrayList();

						int j = 0;
						while ( j < m_List.Count )
						{
							if ( ((Guild) m_List[ j ]).IsWar( m_Guild ) )
							{
								m_SortWarList.Add( m_List[ j ] );
								m_List.Remove( m_List[ j ] );
							}
							else
							{
								j++;
							}
						}
						j = 0;
						while ( j < m_List.Count )
						{
							if ( ((Guild) m_List[ j ]).IsAlly( m_Guild ) )
							{
								m_SortAllyList.Add( m_List[ j ] );
								m_List.Remove( m_List[ j ] );
							}
							else
							{
								j++;
							}
						}

						m_SortAllyList.Sort( new ListNameSorter( updown ) );
						m_SortWarList.Sort( new ListNameSorter( updown ) );
						m_List.Sort( new ListNameSorter( updown ) );

						for ( j = 0; j < m_SortAllyList.Count; j++ )
						{
							m_SortWarList.Add( m_SortAllyList[ j ] );
						}
						for ( j = 0; j < m_List.Count; j++ )
						{
							m_SortWarList.Add( m_List[ j ] );
						}
						m_List = m_SortWarList;
						break;
					}
				case 3:
					{
						m_List.Sort( new ListRelationshipSorter( updown ) );
						break;
					}
				case 4:
					{
						m_List.Sort( new ListAwaitingSorter( updown ) );
						break;
					}
			}

			for ( int i = 0; i < list.Count; ++i )
			{
				if ( (i%8) == 0 )
				{
					AddButton( 95, 80, 0x15E1, 0x15E5, 0, GumpButtonType.Page, (i/8) + 1 ); // Next Page
					AddPage( (i/8) + 1 );
					AddButton( 65, 80, 0x15E3, 0x15E7, 0, GumpButtonType.Page, (i/8) ); // Previous Page
				}

				Guild g = (Guild) list[ i ] as Guild;

				if ( m_Guild.WarInvitations.Contains( g ) || m_Guild.AllyInvitations.Contains( g ) || g.AllyDeclarations.Contains( m_Guild ) || g.WarDeclarations.Contains( m_Guild ) || g.WarInvitations.Contains( m_Guild ) || g.AllyInvitations.Contains( m_Guild ) || m_Guild.AllyDeclarations.Contains( g ) || m_Guild.WarDeclarations.Contains( g ) )
				{
					AddButton( 36, 143 + ((i%8)*28), 0x8AF, 0x8AF, i + 1, GumpButtonType.Reply, 0 );
				}
				else if ( g == m_Guild || g.IsAlly( m_Guild ) || g.IsWar( m_Guild ) )
				{
					AddButton( 40, 143 + ((i%8)*28), 0x4B9, 0x4BA, i + 1, GumpButtonType.Reply, 0 );
				}
				else
				{
					AddButton( 40, 143 + ((i%8)*28), 0x4B9, 0x4BA, i + 1, GumpButtonType.Reply, 0 );
				}

				AddImageTiled( 65, 138 + ((i%8)*28), 290, 26, 0xA40 );
				AddImageTiled( 67, 140 + ((i%8)*28), 286, 22, 0xBBC );

				string name;

				if ( (name = g.Name) != null && (name = name.Trim()).Length <= 0 )
				{
					name = "";
				}

				if ( name == m_Guild.Name )
				{
					AddHtml( 70, 141 + ((i%8)*28), 280, 20, "<basefont color=#006600>" + name + "</basefont>", false, false );
				}
				else
				{
					AddHtml( 70, 141 + ((i%8)*28), 280, 20, name, false, false );
				}


				string abbr;

				if ( (abbr = g.Abbreviation) != null && (abbr = abbr.Trim()).Length <= 0 )
				{
					abbr = "";
				}

				AddImageTiled( 357, 138 + ((i%8)*28), 60, 26, 0xA40 );
				AddImageTiled( 359, 140 + ((i%8)*28), 56, 22, 0xBBC );

				AddHtml( 362, 141 + ((i%8)*28), 50, 20, abbr, false, false );

				AddImageTiled( 419, 138 + ((i%8)*28), 130, 26, 0xA40 );
				AddImageTiled( 421, 140 + ((i%8)*28), 126, 22, 0xBBC );


				if ( g == m_Guild )
				{
					if ( g.Allies.Count > 0 )
					{
						if ( g.AllianceLeader )
						{
							AddHtmlLocalized( 424, 141 + ((i%8)*28), 120, 20, 1063237, false, false ); // Alliance Leader
						}
						else
						{
							AddHtmlLocalized( 424, 141 + ((i%8)*28), 120, 20, 1062964, false, false ); // Ally
						}
					}
					else
					{
						AddHtmlLocalized( 424, 141 + ((i%8)*28), 120, 20, 3000085, false, false ); // Peace
					}
				}
				if ( m_Guild.IsWar( g ) )
				{
					AddHtmlLocalized( 424, 141 + ((i%8)*28), 120, 20, 3000086, false, false ); // War
				}
				else if ( m_Guild.IsAlly( g ) )
				{
					if ( g.AllianceLeader )
					{
						AddHtmlLocalized( 424, 141 + ((i%8)*28), 120, 20, 1063237, false, false ); // Alliance Leader
					}
					else
					{
						AddHtmlLocalized( 424, 141 + ((i%8)*28), 120, 20, 1062964, false, false ); // Ally
					}
				}
				else if ( g != m_Guild )
				{
					AddHtmlLocalized( 424, 141 + ((i%8)*28), 120, 20, 3000085, false, false ); // Peace
				}
				if ( i == m_List.Count - 1 )
				{
					if ( m_Guild.Allies.Count + 1 >= m_List.Count )
					{
						AddHtml( 66, 204 + (((i%8) - 1)*28), 300, 26, "<basefont color=#000066>" + g.AllianceName + "</basefont>", false, false );
					}
				}
			}
		}

		protected virtual void Design()
		{
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

				xstr = ((Guild) x).Name;
				ystr = ((Guild) y).Name;

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

		private class ListAbbrSorter : IComparer
		{
			private bool Dsort;

			public ListAbbrSorter( bool descend ) : base()
			{
				Dsort = descend;
			}

			public int Compare( object x, object y )
			{
				string xstr = null;
				string ystr = null;

				xstr = ((Guild) x).Abbreviation;
				ystr = ((Guild) y).Abbreviation;

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

		private class ListAwaitingSorter : IComparer
		{
			private bool Dsort;

			public ListAwaitingSorter( bool descend ) : base()
			{
				Dsort = descend;
			}

			public int Compare( object x, object y )
			{
				string xstr = null;
				string ystr = null;

				xstr = ((Guild) x).Name;
				ystr = ((Guild) y).Name;

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

		private class ListRelationshipSorter : IComparer
		{
			private bool Dsort;

			public ListRelationshipSorter( bool descend ) : base()
			{
				Dsort = descend;
			}

			public int Compare( object x, object y )
			{
				string xstr = null;
				string ystr = null;

				xstr = ((Guild) x).Name;
				ystr = ((Guild) y).Name;

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
	}
}