using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Accounting;

namespace Server.Items
{
	public class SoulStone : Item
	{
		public static readonly TimeSpan UseDelay = TimeSpan.FromDays( 1.0 );

		private string m_Account;
		private DateTime m_NextUse;

		private SkillName m_Skill;
		private double m_SkillValue;

		[CommandProperty( AccessLevel.GameMaster )]
		public string Account { get { return m_Account; } set { m_Account = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime NextUse { get { return m_NextUse; } set { m_NextUse = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill
		{
			get { return m_Skill; }
			set
			{
				m_Skill = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double SkillValue
		{
			get { return m_SkillValue; }
			set
			{
				m_SkillValue = value;

				if ( !(this is SoulStoneFragment) )
				{
					if ( m_SkillValue > 0.0 )
					{
						this.ItemID = 0x2A94;
					}
					else
					{
						this.ItemID = 0x2A93;
					}
				}

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsEmpty { get { return m_SkillValue <= 0.0; } }

		[Constructable]
		public SoulStone() : this( null )
		{
		}

		[Constructable]
		public SoulStone( string account ) : base( 0x2A93 )
		{
			Light = LightType.Circle300;
			Weight = 5.0;
			LootType = LootType.Blessed;

			m_Account = account;
			m_NextUse = DateTime.MinValue;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( !this.IsEmpty )
			{
				list.Add( 1070721, "#{0}\t{1:0.0}", 1044060 + (int) this.Skill, this.SkillValue ); // Skill stored: ~1_skillname~ ~2_skillamount~
			}
		}

		private static bool CheckCombat( Mobile m, TimeSpan time )
		{
			for ( int i = 0; i < m.Aggressed.Count; ++i )
			{
				AggressorInfo info = (AggressorInfo) m.Aggressed[ i ];

				if ( DateTime.Now - info.LastCombatTime < time )
				{
					return true;
				}
			}

			return false;
		}

		private bool CheckUse( Mobile from )
		{
			DateTime now = DateTime.Now;

			if ( this.Deleted || !this.IsAccessibleTo( from ) )
			{
				return false;
			}
			else if ( from.Map != this.Map || !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
				return false;
			}
			else if ( this is SoulStoneFragment )
			{
				SoulStoneFragment ssf = this as SoulStoneFragment;

				if ( ssf.UsesRemaining <= 0 )
				{
					from.SendLocalizedMessage( 1070975 ); // That soulstone fragment has no more uses.
					return false;
				}

				return true;
			}
			else if ( this.Account != null && (!(from.Account is Account) || ((Account) from.Account).Username != this.Account) )
			{
				from.SendLocalizedMessage( 1070714 ); // This is an Account Bound Soulstone, and your character is not bound to it.  You cannot use this Soulstone.
				return false;
			}
			else if ( CheckCombat( from, TimeSpan.FromMinutes( 2.0 ) ) )
			{
				from.SendLocalizedMessage( 1070727 ); // You must wait two minutes after engaging in combat before you can use a Soulstone.
				return false;
			}
			else if ( from.Criminal )
			{
				from.SendLocalizedMessage( 1070728 ); // You must wait two minutes after committing a criminal act before you can use a Soulstone.
				return false;
			}
			else if ( from.Region.GetLogoutDelay( from ) > TimeSpan.Zero )
			{
				from.SendLocalizedMessage( 1070729 ); // In order to use your Soulstone, you must be in a safe log-out location.
				return false;
			}
			else if ( !from.Alive )
			{
				from.SendLocalizedMessage( 1070730 ); // You may not use a Soulstone while your character is dead.
				return false;
			}
			else if ( Factions.Sigil.ExistsOn( from ) )
			{
				from.SendLocalizedMessage( 1070731 ); // You may not use a Soulstone while your character has a faction town sigil.
				return false;
			}
			else if ( from.Spell != null && from.Spell.IsCasting )
			{
				from.SendLocalizedMessage( 1070733 ); // You may not use a Soulstone while your character is casting a spell.
				return false;
			}
			else if ( from.Poisoned )
			{
				from.SendLocalizedMessage( 1070734 ); // You may not use a Soulstone while your character is poisoned.
				return false;
			}
			else if ( from.Paralyzed )
			{
				from.SendLocalizedMessage( 1070735 ); // You may not use a Soulstone while your character is paralyzed.
				return false;
			}
			else if ( now < this.NextUse )
			{
				TimeSpan time = this.NextUse - now;

				if ( time.TotalHours > 0.0 )
				{
					from.SendLocalizedMessage( 1070736, ((int) time.TotalHours).ToString() ); // You must wait ~1_hours~ hours before you can use your Soulstone.
				}
				else if ( time.TotalMinutes > 0.0 )
				{
					from.SendLocalizedMessage( 1070737, ((int) time.TotalMinutes).ToString() ); // You must wait ~1_minutes~ minutes before you can use your Soulstone.
				}
				else
				{
					from.SendLocalizedMessage( 1070738, ((int) time.TotalSeconds).ToString() ); // You must wait ~1_seconds~ seconds before you can use your Soulstone.
				}

				return false;
			}
			else
			{
				return true;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !CheckUse( from ) )
			{
				return;
			}

			from.CloseGump( typeof( SelectSkillGump ) );
			from.CloseGump( typeof( ConfirmSkillGump ) );
			from.CloseGump( typeof( ConfirmTransferGump ) );
			from.CloseGump( typeof( ConfirmRemovalGump ) );
			from.CloseGump( typeof( ErrorGump ) );

			if ( this.IsEmpty )
			{
				from.SendGump( new SelectSkillGump( this, from ) );
			}
			else
			{
				from.SendGump( new ConfirmTransferGump( this, from ) );
			}
		}

		private class SelectSkillGump : Gump
		{
			private SoulStone m_Stone;

			public SelectSkillGump( SoulStone stone, Mobile from ) : base( 50, 50 )
			{
				m_Stone = stone;

				AddPage( 0 );

				AddBackground( 0, 0, 520, 440, 0x13BE );

				AddImageTiled( 10, 10, 500, 20, 0xA40 );
				AddImageTiled( 10, 40, 500, 360, 0xA40 );
				AddImageTiled( 10, 410, 500, 20, 0xA40 );

				AddAlphaRegion( 10, 10, 500, 420 );

				AddHtmlLocalized( 10, 12, 500, 20, 1061087, 0x7FFF, false, false ); // Which skill do you wish to transfer to the Soulstone?

				AddButton( 10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 412, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL

				for ( int i = 0, n = 0; i < from.Skills.Length; i++ )
				{
					Skill skill = from.Skills[ i ];

					if ( skill.Base > 0.0 )
					{
						int p = n%30;

						if ( p == 0 )
						{
							int page = n/30;

							if ( page > 0 )
							{
								AddButton( 260, 380, 0xFA5, 0xFA6, 0, GumpButtonType.Page, page + 1 );
								AddHtmlLocalized( 305, 382, 200, 20, 1011066, 0x7FFF, false, false ); // Next page
							}

							AddPage( page + 1 );

							if ( page > 0 )
							{
								AddButton( 10, 380, 0xFAE, 0xFAF, 0, GumpButtonType.Page, page );
								AddHtmlLocalized( 55, 382, 200, 20, 1011067, 0x7FFF, false, false ); // Previous page
							}
						}

						int x = (p%2 == 0) ? 10 : 260;
						int y = (p/2)*20 + 40;

						AddButton( x, y, 0xFA5, 0xFA6, i + 1, GumpButtonType.Reply, 0 );
						AddHtmlLocalized( x + 45, y + 2, 200, 20, 1044060 + i, 0x7FFF, false, false );

						n++;
					}
				}
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( info.ButtonID == 0 || !m_Stone.IsEmpty )
				{
					return;
				}

				Mobile from = sender.Mobile;

				int iSkill = info.ButtonID - 1;
				if ( iSkill < 0 || iSkill >= from.Skills.Length )
				{
					return;
				}

				Skill skill = from.Skills[ iSkill ];
				if ( skill.Base <= 0.0 )
				{
					return;
				}

				if ( !m_Stone.CheckUse( from ) )
				{
					return;
				}

				from.SendGump( new ConfirmSkillGump( m_Stone, skill ) );
			}
		}

		private class ConfirmSkillGump : Gump
		{
			private SoulStone m_Stone;
			private Skill m_Skill;

			public ConfirmSkillGump( SoulStone stone, Skill skill ) : base( 50, 50 )
			{
				m_Stone = stone;
				m_Skill = skill;

				AddBackground( 0, 0, 520, 440, 0x13BE );

				AddImageTiled( 10, 10, 500, 20, 0xA40 );
				AddImageTiled( 10, 40, 500, 360, 0xA40 );
				AddImageTiled( 10, 410, 500, 20, 0xA40 );

				AddAlphaRegion( 10, 10, 500, 420 );

				AddHtmlLocalized( 10, 12, 500, 20, 1070709, 0x7FFF, false, false ); // <CENTER>Confirm Soulstone Transfer</CENTER>

				/* <CENTER>Soulstone</CENTER><BR>
				 * You are using a Soulstone.  This powerful artifact allows you to remove skill points
				 * from your character and store them in the stone for later retrieval.  In order to use
				 * the stone, you must make sure your Skill Lock for the indicated skill is pointed downward.
				 * Click the "Skills" button on your Paperdoll to access the Skill List, and double-check
				 * your skill lock.<BR><BR>
				 * 
				 * Once you activate the stone, all skill points in the indicated skill will be removed from
				 * your character.  These skill points can later be retrieved.  IMPORTANT: When retrieving
				 * skill points from a Soulstone, the Soulstone WILL REPLACE any existing skill points
				 * already on your character!<BR><BR>
				 * 
				 * This is an Account Bound Soulstone.  Skill pointsstored inside can be retrieved by any
				 * character on the same account as the character who placed them into the stone.
				 */
				AddHtmlLocalized( 10, 42, 500, 110, 1061067, 0x7FFF, false, true );

				AddHtmlLocalized( 10, 200, 390, 20, 1062297, 0x7FFF, false, false ); // Skill Chosen:
				AddHtmlLocalized( 210, 200, 390, 20, 1044060 + skill.SkillID, 0x7FFF, false, false );

				AddHtmlLocalized( 10, 220, 390, 20, 1062298, 0x7FFF, false, false ); // Current Value:
				AddLabel( 210, 220, 0x481, skill.Base.ToString( "0.0" ) );

				AddHtmlLocalized( 10, 240, 390, 20, 1062299, 0x7FFF, false, false ); // Current Cap:
				AddLabel( 210, 240, 0x481, skill.Cap.ToString( "0.0" ) );

				AddHtmlLocalized( 10, 260, 390, 20, 1062300, 0x7FFF, false, false ); // New Value:
				AddLabel( 210, 260, 0x481, "0.0" );

				AddButton( 10, 360, 0xFA5, 0xFA6, 2, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 362, 450, 20, 1070720, 0x7FFF, false, false ); // Activate the stone.  I am ready to transfer the skill points to it.

				AddButton( 10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 382, 450, 20, 1062279, 0x7FFF, false, false ); // No, let me make another selection.

				AddButton( 10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 412, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( info.ButtonID == 0 || !m_Stone.IsEmpty )
				{
					return;
				}

				Mobile from = sender.Mobile;

				if ( !m_Stone.CheckUse( from ) )
				{
					return;
				}

				if ( info.ButtonID == 1 ) // Is asking for another selection
				{
					from.SendGump( new SelectSkillGump( m_Stone, from ) );
					return;
				}

				if ( m_Skill.Base <= 0.0 )
				{
					return;
				}

				if ( m_Skill.Lock != SkillLock.Down )
				{
					// <CENTER>Unable to Transfer Selected Skill to Soulstone</CENTER>

					/* You cannot transfer the selected skill to the Soulstone at this time. The selected
					 * skill may be locked or set to raise in your skill menu. Click on "Skills" in your
					 * paperdoll menu to check your raise/locked/lower settings and your total skills.
					 * Make any needed adjustments, then click "Continue". If you do not wish to transfer
					 * the selected skill at this time, click "Cancel".
					 */

					from.SendGump( new ErrorGump( m_Stone, 1070710, 1070711 ) );
					return;
				}

				m_Stone.Skill = m_Skill.SkillName;
				m_Stone.SkillValue = m_Skill.Base;

				m_Skill.Base = 0.0;

				from.SendLocalizedMessage( 1070712 ); // You have successfully transferred your skill points into the Soulstone.

				Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0, 0, 0, 0, 0, 5060, 0 );
				Effects.PlaySound( from.Location, from.Map, 0x243 );

				int k = (m_Stone is SoulStoneFragment) ? 3 : 1;

				for ( int i = 1; i < k; i++ )
				{
					Effects.SendMovingParticles( new Entity( Server.Serial.Zero, new Point3D( from.X - 6, from.Y - 6, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer) 255, 0x100 );
				}

				Effects.SendTargetParticles( from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer) 255, 0x100 );


			}
		}

		private class ConfirmTransferGump : Gump
		{
			private SoulStone m_Stone;

			public ConfirmTransferGump( SoulStone stone, Mobile from ) : base( 50, 50 )
			{
				m_Stone = stone;

				AddBackground( 0, 0, 520, 440, 0x13BE );

				AddImageTiled( 10, 10, 500, 20, 0xA40 );
				AddImageTiled( 10, 40, 500, 360, 0xA40 );
				AddImageTiled( 10, 410, 500, 20, 0xA40 );

				AddAlphaRegion( 10, 10, 500, 420 );

				AddHtmlLocalized( 10, 12, 500, 20, 1070709, 0x7FFF, false, false ); // <CENTER>Confirm Soulstone Transfer</CENTER>

				/* <CENTER>Soulstone</CENTER><BR>
				 * You are using a Soulstone.  This powerful artifact allows you to remove skill points
				 * from your character and store them in the stone for later retrieval.  In order to use
				 * the stone, you must make sure your Skill Lock for the indicated skill is pointed downward.
				 * Click the "Skills" button on your Paperdoll to access the Skill List, and double-check
				 * your skill lock.<BR><BR>
				 * 
				 * Once you activate the stone, all skill points in the indicated skill will be removed from
				 * your character.  These skill points can later be retrieved.  IMPORTANT: When retrieving
				 * skill points from a Soulstone, the Soulstone WILL REPLACE any existing skill points
				 * already on your character!<BR><BR>
				 * 
				 * This is an Account Bound Soulstone.  Skill pointsstored inside can be retrieved by any
				 * character on the same account as the character who placed them into the stone.
				 */
				AddHtmlLocalized( 10, 42, 500, 110, 1061067, 0x7FFF, false, true );

				AddHtmlLocalized( 10, 200, 390, 20, 1070718, 0x7FFF, false, false ); // Skill Stored:
				AddHtmlLocalized( 210, 200, 390, 20, 1044060 + (int) stone.Skill, 0x7FFF, false, false );

				Skill fromSkill = from.Skills[ stone.Skill ];

				AddHtmlLocalized( 10, 220, 390, 20, 1062298, 0x7FFF, false, false ); // Current Value:
				AddLabel( 210, 220, 0x481, fromSkill.Base.ToString( "0.0" ) );

				AddHtmlLocalized( 10, 240, 390, 20, 1062299, 0x7FFF, false, false ); // Current Cap:
				AddLabel( 210, 240, 0x481, fromSkill.Cap.ToString( "0.0" ) );

				AddHtmlLocalized( 10, 260, 390, 20, 1062300, 0x7FFF, false, false ); // New Value:
				AddLabel( 210, 260, 0x481, stone.SkillValue.ToString( "0.0" ) );

				AddButton( 10, 360, 0xFA5, 0xFA6, 2, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 362, 450, 20, 1070719, 0x7FFF, false, false ); // Activate the stone.  I am ready to retrieve the skill points from it.

				AddButton( 10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 382, 450, 20, 1070723, 0x7FFF, false, false ); // Remove all skill points from this stone and DO NOT absorb them.

				AddButton( 10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 412, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( info.ButtonID == 0 || m_Stone.IsEmpty )
				{
					return;
				}

				Mobile from = sender.Mobile;

				if ( !m_Stone.CheckUse( from ) )
				{
					return;
				}

				if ( info.ButtonID == 1 ) // Remove skill points
				{
					from.SendGump( new ConfirmRemovalGump( m_Stone ) );
					return;
				}

				SkillName skill = m_Stone.Skill;
				double skillValue = m_Stone.SkillValue;

				Skill fromSkill = from.Skills[ m_Stone.Skill ];

				if ( fromSkill.Lock != SkillLock.Up || (from.SkillsTotal - fromSkill.BaseFixedPoint + ((int) skillValue*10) > from.SkillsCap) )
				{
					// <CENTER>Unable to Absorb Selected Skill from Soulstone</CENTER>

					/* You cannot absorb the selected skill from the Soulstone at this time. The selected
					 * skill may be locked or set to lower in your skill menu. You may also be at your
					 * total skill cap.  Click on "Skills" in your paperdoll menu to check your
					 * raise/locked/lower settings and your total skills.  Make any needed adjustments,
					 * then click "Continue". If you do not wish to transfer the selected skill at this
					 * time, click "Cancel".
					 */

					from.SendGump( new ErrorGump( m_Stone, 1070717, 1070716 ) );
					return;
				}

				if ( skillValue > fromSkill.Cap )
				{
					// <CENTER>Unable to Absorb Selected Skill from Soulstone</CENTER>

					/* The amount of skill stored in this stone exceeds your individual skill cap for
					 * that skill.  In order to retrieve the skill points stored in this stone, you must
					 * obtain a Power Scroll of the appropriate type and level in order to increase your
					 * skill cap.  You cannot currently retrieve the skill points stored in this stone.
					 */

					from.SendGump( new ErrorGump( m_Stone, 1070717, 1070715 ) );
					return;
				}

				if ( fromSkill.Base >= skillValue )
				{
					// <CENTER>Unable to Absorb Selected Skill from Soulstone</CENTER>

					/* You cannot transfer the selected skill to the Soulstone at this time. The selected
					 * skill has a skill level higher than what is stored in the Soulstone.
					 */

					// Wrong message?!

					from.SendGump( new ErrorGump( m_Stone, 1070717, 1070802 ) );
					return;
				}

				fromSkill.Base = skillValue;
				m_Stone.SkillValue = 0.0;

				m_Stone.NextUse = DateTime.Now + UseDelay;

				if ( m_Stone is SoulStoneFragment )
				{
					SoulStoneFragment ssf = m_Stone as SoulStoneFragment;

					ssf.UsesRemaining--;

					if ( ssf.UsesRemaining == 0 )
					{
						from.SendLocalizedMessage( 1070974 ); // You have used up your soulstone fragment.
					}
				}
				else
				{
					from.SendLocalizedMessage( 1070713 ); // You have successfully absorbed the Soulstone's skill points.

					Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0, 0, 0, 0, 0, 5060, 0 );
					Effects.PlaySound( from.Location, from.Map, 0x243 );

					int k = (m_Stone is SoulStoneFragment) ? 3 : 1;

					for ( int i = 1; i < k; i++ )
					{
						Effects.SendMovingParticles( new Entity( Server.Serial.Zero, new Point3D( from.X - 6, from.Y - 6, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer) 255, 0x100 );
					}

					Effects.SendTargetParticles( from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer) 255, 0x100 );
				}
			}
		}

		private class ConfirmRemovalGump : Gump
		{
			private SoulStone m_Stone;

			public ConfirmRemovalGump( SoulStone stone ) : base( 50, 50 )
			{
				m_Stone = stone;

				AddBackground( 0, 0, 520, 440, 0x13BE );

				AddImageTiled( 10, 10, 500, 20, 0xA40 );
				AddImageTiled( 10, 40, 500, 360, 0xA40 );
				AddImageTiled( 10, 410, 500, 20, 0xA40 );

				AddAlphaRegion( 10, 10, 500, 420 );

				AddHtmlLocalized( 10, 12, 500, 20, 1070725, 0x7FFF, false, false ); // <CENTER>Confirm Soulstone Skill Removal</CENTER>

				/* WARNING!<BR><BR>
				 * 
				 * You are about to permanently remove all skill points stored in this Soulstone.
				 * You WILL NOT absorb these skill points.  They will be DELETED.<BR><BR>
				 * 
				 * Are you sure you wish to do this?  If not, press the Cancel button.
				 */
				AddHtmlLocalized( 10, 42, 500, 110, 1070724, 0x7FFF, false, true );

				AddButton( 10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 382, 450, 20, 1052072, 0x7FFF, false, false ); // Continue

				AddButton( 10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 412, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( info.ButtonID == 0 || m_Stone.IsEmpty )
				{
					return;
				}

				Mobile from = sender.Mobile;

				if ( !m_Stone.CheckUse( from ) )
				{
					return;
				}

				m_Stone.SkillValue = 0.0;
				from.SendLocalizedMessage( 1070726 ); // You have successfully deleted the Soulstone's skill points.
			}
		}

		private class ErrorGump : Gump
		{
			private SoulStone m_Stone;

			public ErrorGump( SoulStone stone, int title, int message ) : base( 50, 50 )
			{
				m_Stone = stone;

				AddBackground( 0, 0, 520, 440, 0x13BE );

				AddImageTiled( 10, 10, 500, 20, 0xA40 );
				AddImageTiled( 10, 40, 500, 360, 0xA40 );
				AddImageTiled( 10, 410, 500, 20, 0xA40 );

				AddAlphaRegion( 10, 10, 500, 420 );

				AddHtmlLocalized( 10, 12, 500, 20, title, 0x7FFF, false, false );

				AddHtmlLocalized( 10, 42, 500, 110, message, 0x7FFF, false, true );

				AddButton( 10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 382, 450, 20, 1052072, 0x7FFF, false, false ); // Continue

				AddButton( 10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 412, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( info.ButtonID == 0 )
				{
					return;
				}

				Mobile from = sender.Mobile;

				if ( !m_Stone.CheckUse( from ) )
				{
					return;
				}

				if ( m_Stone.IsEmpty )
				{
					from.SendGump( new SelectSkillGump( m_Stone, from ) );
				}
				else
				{
					from.SendGump( new ConfirmTransferGump( m_Stone, from ) );
				}
			}
		}

		public SoulStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( (string) m_Account );
			writer.Write( (DateTime) m_NextUse );

			writer.WriteEncodedInt( (int) m_Skill );
			writer.Write( (double) m_SkillValue );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Account = reader.ReadString();
			m_NextUse = reader.ReadDateTime();

			m_Skill = (SkillName) reader.ReadEncodedInt();
			m_SkillValue = reader.ReadDouble();
		}
	}
}