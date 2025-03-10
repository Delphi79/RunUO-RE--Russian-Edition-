using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Scripts.Gumps
{
	public class EditCapGump : Gump
	{
		public const bool OldStyle = PropsConfig.OldStyle;

		public const int GumpOffsetX = PropsConfig.GumpOffsetX;
		public const int GumpOffsetY = PropsConfig.GumpOffsetY;

		public const int TextHue = PropsConfig.TextHue;
		public const int TextOffsetX = PropsConfig.TextOffsetX;

		public const int OffsetGumpID = PropsConfig.OffsetGumpID;
		public const int HeaderGumpID = PropsConfig.HeaderGumpID;
		public const int EntryGumpID = PropsConfig.EntryGumpID;
		public const int BackGumpID = PropsConfig.BackGumpID;
		public const int SetGumpID = PropsConfig.SetGumpID;

		public const int SetWidth = PropsConfig.SetWidth;
		public const int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
		public const int SetButtonID1 = PropsConfig.SetButtonID1;
		public const int SetButtonID2 = PropsConfig.SetButtonID2;

		public const int PrevWidth = PropsConfig.PrevWidth;
		public const int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
		public const int PrevButtonID1 = PropsConfig.PrevButtonID1;
		public const int PrevButtonID2 = PropsConfig.PrevButtonID2;

		public const int NextWidth = PropsConfig.NextWidth;
		public const int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
		public const int NextButtonID1 = PropsConfig.NextButtonID1;
		public const int NextButtonID2 = PropsConfig.NextButtonID2;

		public const int OffsetSize = PropsConfig.OffsetSize;

		public const int EntryHeight = PropsConfig.EntryHeight;
		public const int BorderSize = PropsConfig.BorderSize;

		private const int EntryWidth = 160;

		private const int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
		private const int TotalHeight = OffsetSize + (2*(EntryHeight + OffsetSize));

		private const int BackWidth = BorderSize + TotalWidth + BorderSize;
		private const int BackHeight = BorderSize + TotalHeight + BorderSize;

		private Mobile m_From;
		private Mobile m_Target;
		private Skill m_Skill;

		private CapsGumpGroup m_Selected;

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 1 )
			{
				try
				{
					if ( m_From.AccessLevel >= AccessLevel.Seer )
					{
						TextRelay text = info.GetTextEntry( 0 );

						if ( text != null )
						{
							m_Skill.Cap = Convert.ToDouble( text.Text );
							Server.Scripts.Commands.CommandLogging.LogChangeProperty( m_From, m_Target, String.Format( "{0}.Base", m_Skill.Cap ), m_Skill.Cap.ToString() );
						}
					}
					else
					{
						m_From.SendMessage( "You may not change that." );
					}

					m_From.SendGump( new CapsGump( m_From, m_Target, m_Selected ) );
				} 
				catch
				{
					m_From.SendMessage( "Bad format. ###.# expected." );
					m_From.SendGump( new EditCapGump( m_From, m_Target, m_Skill, m_Selected ) );
				}
			}
			else
			{
				m_From.SendGump( new CapsGump( m_From, m_Target, m_Selected ) );
			}
		}

		public EditCapGump( Mobile from, Mobile target, Skill skill, CapsGumpGroup selected ) : base( GumpOffsetX, GumpOffsetY )
		{
			m_From = from;
			m_Target = target;
			m_Skill = skill;
			m_Selected = selected;

			string initialText = m_Skill.Cap.ToString( "F1" );

			AddPage( 0 );

			AddBackground( 0, 0, BackWidth, BackHeight, BackGumpID );
			AddImageTiled( BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), TotalHeight, OffsetGumpID );

			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize;

			AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
			AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, skill.Name );
			x += EntryWidth + OffsetSize;

			if ( SetGumpID != 0 )
			{
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );
			}

			x = BorderSize + OffsetSize;
			y += EntryHeight + OffsetSize;

			AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
			AddTextEntry( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, 0, initialText );
			x += EntryWidth + OffsetSize;

			if ( SetGumpID != 0 )
			{
				AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );
			}

			AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 1, GumpButtonType.Reply, 0 );
		}
	}

	public class CapsGump : Gump
	{
		public static bool OldStyle = PropsConfig.OldStyle;

		public const int GumpOffsetX = PropsConfig.GumpOffsetX;
		public const int GumpOffsetY = PropsConfig.GumpOffsetY;

		public const int TextHue = PropsConfig.TextHue;
		public const int TextOffsetX = PropsConfig.TextOffsetX;

		public const int OffsetGumpID = PropsConfig.OffsetGumpID;
		public const int HeaderGumpID = PropsConfig.HeaderGumpID;
		public const int EntryGumpID = PropsConfig.EntryGumpID;
		public const int BackGumpID = PropsConfig.BackGumpID;
		public const int SetGumpID = PropsConfig.SetGumpID;

		public const int SetWidth = PropsConfig.SetWidth;
		public const int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
		public const int SetButtonID1 = PropsConfig.SetButtonID1;
		public const int SetButtonID2 = PropsConfig.SetButtonID2;

		public const int PrevWidth = PropsConfig.PrevWidth;
		public const int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
		public const int PrevButtonID1 = PropsConfig.PrevButtonID1;
		public const int PrevButtonID2 = PropsConfig.PrevButtonID2;

		public const int NextWidth = PropsConfig.NextWidth;
		public const int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
		public const int NextButtonID1 = PropsConfig.NextButtonID1;
		public const int NextButtonID2 = PropsConfig.NextButtonID2;

		public const int OffsetSize = PropsConfig.OffsetSize;

		public const int EntryHeight = PropsConfig.EntryHeight;
		public const int BorderSize = PropsConfig.BorderSize;

		private static bool PrevLabel = OldStyle, NextLabel = OldStyle;

		private const int PrevLabelOffsetX = PrevWidth + 1;
		private const int PrevLabelOffsetY = 0;

		private const int NextLabelOffsetX = -29;
		private const int NextLabelOffsetY = 0;

		private const int NameWidth = 107;
		private const int ValueWidth = 128;

		private const int EntryCount = 15;

		private const int TypeWidth = NameWidth + OffsetSize + ValueWidth;

		private const int TotalWidth = OffsetSize + NameWidth + OffsetSize + ValueWidth + OffsetSize + SetWidth + OffsetSize;
		private const int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize)*(EntryCount + 1));

		private const int BackWidth = BorderSize + TotalWidth + BorderSize;
		private const int BackHeight = BorderSize + TotalHeight + BorderSize;

		private const int IndentWidth = 12;

		private Mobile m_From;
		private Mobile m_Target;

		private CapsGumpGroup[] m_Groups;
		private CapsGumpGroup m_Selected;

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			int buttonID = info.ButtonID - 1;

			int index = buttonID/3;
			int type = buttonID%3;

			switch ( type )
			{
				case 0:
					{
						if ( index >= 0 && index < m_Groups.Length )
						{
							CapsGumpGroup newSelection = m_Groups[ index ];

							if ( m_Selected != newSelection )
							{
								m_From.SendGump( new CapsGump( m_From, m_Target, newSelection ) );
							}
							else
							{
								m_From.SendGump( new CapsGump( m_From, m_Target, null ) );
							}
						}

						break;
					}
				case 1:
					{
						if ( m_Selected != null && index >= 0 && index < m_Selected.Skills.Length )
						{
							Skill sk = m_Target.Skills[ m_Selected.Skills[ index ] ];

							if ( sk != null )
							{
								if ( m_From.AccessLevel >= AccessLevel.Seer )
								{
									m_From.SendGump( new EditCapGump( m_From, m_Target, sk, m_Selected ) );
								}
								else
								{
									m_From.SendMessage( "You may not change that." );
									m_From.SendGump( new CapsGump( m_From, m_Target, m_Selected ) );
								}
							}
							else
							{
								m_From.SendGump( new CapsGump( m_From, m_Target, m_Selected ) );
							}
						}

						break;
					}
				case 2:
					{
						if ( m_Selected != null && index >= 0 && index < m_Selected.Skills.Length )
						{
							Skill sk = m_Target.Skills[ m_Selected.Skills[ index ] ];

							if ( sk != null )
							{
								if ( m_From.AccessLevel >= AccessLevel.GameMaster )
								{
									switch ( sk.Lock )
									{
										case SkillLock.Up:
											sk.SetLockNoRelay( SkillLock.Down );
											sk.Update();
											break;
										case SkillLock.Down:
											sk.SetLockNoRelay( SkillLock.Locked );
											sk.Update();
											break;
										case SkillLock.Locked:
											sk.SetLockNoRelay( SkillLock.Up );
											sk.Update();
											break;
									}
								}
								else
								{
									m_From.SendMessage( "You may not change that." );
								}

								m_From.SendGump( new CapsGump( m_From, m_Target, m_Selected ) );
							}
						}

						break;
					}
			}
		}

		public int GetButtonID( int type, int index )
		{
			return 1 + (index*3) + type;
		}

		public CapsGump( Mobile from, Mobile target ) : this( from, target, null )
		{
		}

		public CapsGump( Mobile from, Mobile target, CapsGumpGroup selected ) : base( GumpOffsetX, GumpOffsetY )
		{
			m_From = from;
			m_Target = target;

			m_Groups = CapsGumpGroup.Groups;
			m_Selected = selected;

			int count = m_Groups.Length;

			if ( selected != null )
			{
				count += selected.Skills.Length;
			}

			int totalHeight = OffsetSize + ((EntryHeight + OffsetSize)*(count + 1));

			AddPage( 0 );

			AddBackground( 0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID );
			AddImageTiled( BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID );

			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize;

			int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize*4) - (OldStyle ? SetWidth + OffsetSize : 0);

			if ( OldStyle )
			{
				AddImageTiled( x, y, TotalWidth - (OffsetSize*3) - SetWidth, EntryHeight, HeaderGumpID );
			}
			else
			{
				AddImageTiled( x, y, PrevWidth, EntryHeight, HeaderGumpID );
			}

			x += PrevWidth + OffsetSize;

			if ( !OldStyle )
			{
				AddImageTiled( x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize*2 : 0), EntryHeight, HeaderGumpID );
			}

			x += emptyWidth + OffsetSize;

			if ( !OldStyle )
			{
				AddImageTiled( x, y, NextWidth, EntryHeight, HeaderGumpID );
			}

			for ( int i = 0; i < m_Groups.Length; ++i )
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				CapsGumpGroup group = m_Groups[ i ];

				AddImageTiled( x, y, PrevWidth, EntryHeight, HeaderGumpID );

				if ( group == selected )
				{
					AddButton( x + PrevOffsetX, y + PrevOffsetY, 0x15E2, 0x15E6, GetButtonID( 0, i ), GumpButtonType.Reply, 0 );
				}
				else
				{
					AddButton( x + PrevOffsetX, y + PrevOffsetY, 0x15E1, 0x15E5, GetButtonID( 0, i ), GumpButtonType.Reply, 0 );
				}

				x += PrevWidth + OffsetSize;

				x -= (OldStyle ? OffsetSize : 0);

				AddImageTiled( x, y, emptyWidth + (OldStyle ? OffsetSize*2 : 0), EntryHeight, EntryGumpID );
				AddLabel( x + TextOffsetX, y, TextHue, group.Name );

				x += emptyWidth + (OldStyle ? OffsetSize*2 : 0);
				x += OffsetSize;

				if ( SetGumpID != 0 )
				{
					AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );
				}

				if ( group == selected )
				{
					int indentMaskX = BorderSize;
					int indentMaskY = y + EntryHeight + OffsetSize;

					for ( int j = 0; j < group.Skills.Length; ++j )
					{
						Skill sk = target.Skills[ group.Skills[ j ] ];

						x = BorderSize + OffsetSize;
						y += EntryHeight + OffsetSize;

						x += OffsetSize;
						x += IndentWidth;

						AddImageTiled( x, y, PrevWidth, EntryHeight, HeaderGumpID );

						AddButton( x + PrevOffsetX, y + PrevOffsetY, 0x15E1, 0x15E5, GetButtonID( 1, j ), GumpButtonType.Reply, 0 );

						x += PrevWidth + OffsetSize;

						x -= (OldStyle ? OffsetSize : 0);

						AddImageTiled( x, y, emptyWidth + (OldStyle ? OffsetSize*2 : 0) - OffsetSize - IndentWidth, EntryHeight, EntryGumpID );
						AddLabel( x + TextOffsetX, y, TextHue, sk == null ? "(null)" : sk.Name );

						x += emptyWidth + (OldStyle ? OffsetSize*2 : 0) - OffsetSize - IndentWidth;
						x += OffsetSize;

						if ( SetGumpID != 0 )
						{
							AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );
						}

						if ( sk != null )
						{
							int buttonID1, buttonID2;
							int xOffset, yOffset;

							switch ( sk.Lock )
							{
								default:
								case SkillLock.Up:
									buttonID1 = 0x983;
									buttonID2 = 0x983;
									xOffset = 6;
									yOffset = 4;
									break;
								case SkillLock.Down:
									buttonID1 = 0x985;
									buttonID2 = 0x985;
									xOffset = 6;
									yOffset = 4;
									break;
								case SkillLock.Locked:
									buttonID1 = 0x82C;
									buttonID2 = 0x82C;
									xOffset = 5;
									yOffset = 2;
									break;
							}

							AddButton( x + xOffset, y + yOffset, buttonID1, buttonID2, GetButtonID( 2, j ), GumpButtonType.Reply, 0 );

							y += 1;
							x -= OffsetSize;
							x -= 1;
							x -= 50;

							AddImageTiled( x, y, 50, EntryHeight - 2, OffsetGumpID );

							x += 1;
							y += 1;

							AddImageTiled( x, y, 48, EntryHeight - 4, EntryGumpID );

							AddLabelCropped( x + TextOffsetX, y - 1, 48 - TextOffsetX, EntryHeight - 3, TextHue, sk.Cap.ToString( "F1" ) );

							y -= 2;
						}
					}

					AddImageTiled( indentMaskX, indentMaskY, IndentWidth + OffsetSize, (group.Skills.Length*(EntryHeight + OffsetSize)) - (i < (m_Groups.Length - 1) ? OffsetSize : 0), BackGumpID + 4 );
				}
			}
		}
	}

	public class CapsGumpGroup
	{
		private string m_Name;
		private SkillName[] m_Skills;

		public string Name { get { return m_Name; } }
		public SkillName[] Skills { get { return m_Skills; } }

		public CapsGumpGroup( string name, SkillName[] skills )
		{
			m_Name = name;
			m_Skills = skills;

			Array.Sort( m_Skills, new SkillNameComparer() );
		}

		private class SkillNameComparer : IComparer
		{
			public SkillNameComparer()
			{
			}

			public int Compare( object x, object y )
			{
				SkillName a = (SkillName) x;
				SkillName b = (SkillName) y;

				string aName = SkillInfo.Table[ (int) a ].Name;
				string bName = SkillInfo.Table[ (int) b ].Name;

				return aName.CompareTo( bName );
			}
		}

		private static CapsGumpGroup[] m_Groups = new CapsGumpGroup[] {new CapsGumpGroup( "Crafting", new SkillName[] {SkillName.Alchemy, SkillName.Blacksmith, SkillName.Cartography, SkillName.Carpentry, SkillName.Cooking, SkillName.Fletching, SkillName.Inscribe, SkillName.Tailoring, SkillName.Tinkering} ), new CapsGumpGroup( "Bardic", new SkillName[] {SkillName.Discordance, SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation} ), new CapsGumpGroup( "Magical", new SkillName[] {SkillName.Chivalry, SkillName.EvalInt, SkillName.Magery, SkillName.MagicResist, SkillName.Meditation, SkillName.Necromancy, SkillName.SpiritSpeak, SkillName.Ninjitsu, SkillName.Bushido} ), new CapsGumpGroup( "Miscellaneous", new SkillName[] {SkillName.Camping, SkillName.Fishing, SkillName.Focus, SkillName.Healing, SkillName.Herding, SkillName.Lockpicking, SkillName.Lumberjacking, SkillName.Mining, SkillName.Snooping, SkillName.Veterinary} ), new CapsGumpGroup( "Combat Ratings", new SkillName[] {SkillName.Archery, SkillName.Fencing, SkillName.Macing, SkillName.Parry, SkillName.Swords, SkillName.Tactics, SkillName.Wrestling} ), new CapsGumpGroup( "Actions", new SkillName[] {SkillName.AnimalTaming, SkillName.Begging, SkillName.DetectHidden, SkillName.Hiding, SkillName.RemoveTrap, SkillName.Poisoning, SkillName.Stealing, SkillName.Stealth, SkillName.Tracking} ), new CapsGumpGroup( "Lore & Knowledge", new SkillName[] {SkillName.Anatomy, SkillName.AnimalLore, SkillName.ArmsLore, SkillName.Forensics, SkillName.ItemID, SkillName.TasteID} )};

		public static CapsGumpGroup[] Groups { get { return m_Groups; } }
	}
}