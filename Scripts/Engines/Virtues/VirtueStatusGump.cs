using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server
{
	public class VirtueStatusGump : Gump
	{
		private Mobile m_Beholder;

		public VirtueStatusGump( Mobile beholder ) : base( 0, 0 )
		{
			m_Beholder = beholder;

			AddPage( 0 );

			AddImage( 30, 40, 2080 );
			AddImage( 47, 77, 2081 );
			AddImage( 47, 147, 2081 );
			AddImage( 47, 217, 2081 );
			AddImage( 47, 267, 2083 );
			AddImage( 70, 213, 2091 );

			AddPage( 1 );

			AddHtml( 140, 73, 200, 20, "The Virtues", false, false );

			AddHtmlLocalized( 80, 100, 100, 40, 1051000, false, false ); // Humility
			AddHtmlLocalized( 80, 129, 100, 40, 1051001, false, false ); // Sacrifice
			AddHtmlLocalized( 80, 159, 100, 40, 1051002, false, false ); // Compassion
			AddHtmlLocalized( 80, 189, 100, 40, 1051003, false, false ); // Spirituality
			AddHtmlLocalized( 200, 100, 200, 40, 1051004, false, false ); // Valor
			AddHtmlLocalized( 200, 129, 200, 40, 1051005, false, false ); // Honor
			AddHtmlLocalized( 200, 159, 200, 40, 1051006, false, false ); // Justice
			AddHtmlLocalized( 200, 189, 200, 40, 1051007, false, false ); // Honesty

			AddHtmlLocalized( 75, 224, 220, 60, 1052062, false, false ); // Click on a blue gem to view your status in that virtue.

			AddButton( 60, 100, 1210, 1210, 1, GumpButtonType.Reply, 0 );
			AddButton( 60, 129, 1210, 1210, 2, GumpButtonType.Reply, 0 );
			AddButton( 60, 159, 1210, 1210, 3, GumpButtonType.Reply, 0 );
			AddButton( 60, 189, 1210, 1210, 4, GumpButtonType.Reply, 0 );
			AddButton( 180, 100, 1210, 1210, 5, GumpButtonType.Reply, 0 );
			AddButton( 180, 129, 1210, 1210, 6, GumpButtonType.Reply, 0 );
			AddButton( 180, 159, 1210, 1210, 7, GumpButtonType.Reply, 0 );
			AddButton( 180, 189, 1210, 1210, 8, GumpButtonType.Reply, 0 );

			AddButton( 280, 43, 4014, 4014, 9, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 1:
					{
						m_Beholder.SendGump( new VirtueInfoGump( m_Beholder, 1051000, 1052051, m_Beholder.Virtues.Humility ) );
						break;
					}
				case 2:
					{
						m_Beholder.SendGump( new VirtueInfoGump( m_Beholder, 1051001, 1052053, m_Beholder.Virtues.Sacrifice ) );
						break;
					}
				case 3:
					{
						m_Beholder.SendGump( new VirtueInfoGump( m_Beholder, 1051002, 1053000, m_Beholder.Virtues.Compassion ) );
						break;
					}
				case 4:
					{
						m_Beholder.SendGump( new VirtueInfoGump( m_Beholder, 1051003, 1052056, m_Beholder.Virtues.Spirituality ) );
						break;
					}
				case 5:
					{
						m_Beholder.SendGump( new VirtueInfoGump( m_Beholder, 1051004, 1054033, m_Beholder.Virtues.Valor ) );
						break;
					}
				case 6:
					{
						m_Beholder.SendGump( new VirtueInfoGump( m_Beholder, 1051005, 1052058, m_Beholder.Virtues.Honor ) );
						break;
					}
				case 7:
					{
						m_Beholder.SendGump( new VirtueInfoGump( m_Beholder, 1051006, 1052059, m_Beholder.Virtues.Justice ) );
						break;
					}
				case 8:
					{
						m_Beholder.SendGump( new VirtueInfoGump( m_Beholder, 1051007, 1052060, m_Beholder.Virtues.Honesty ) );
						break;
					}
				case 9:
					{
						m_Beholder.SendGump( new VirtueGump( m_Beholder, m_Beholder ) );
						break;
					}
			}
		}
	}
}