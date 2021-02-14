using System;
using Server.Items;
using Server.ContextMenus;
using Server.Spells.Bushido;
using Server.Targeting;
using System.Collections;
using Server.Engines.Craft;

namespace Server.Items
{
	public enum FukiyaQuality
	{
		Low,
		Regular,
		Exceptional
	}

	[FlipableAttribute( 0x27F5, 0x27AA )]
	public class Fukiya : Item, IUsesRemaining, ICraftable
	{
		private int m_UsesRemaining;
		private FukiyaQuality m_Quality;
		private Mobile m_Crafter;
		private Poison m_Poison;
		private int m_PoisonCharges;

		private string m_Crafter_Name;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set
			{
				m_UsesRemaining = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public FukiyaQuality Quality { get { return m_Quality; } set { m_Quality = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get { return m_Crafter; }
			set
			{
				m_Crafter = value;
				CheckName();
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonCharges
		{
			get { return m_PoisonCharges; }
			set
			{
				m_PoisonCharges = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Poison Poison
		{
			get { return m_Poison; }
			set
			{
				m_Poison = value;
				InvalidateProperties();
			}
		}

		public bool ShowUsesRemaining
		{
			get { return true; }
			set
			{
			}
		}

		[Constructable]
		public Fukiya() : base( 0x27F5 )
		{
			Weight = 1.0;

			Layer = Layer.OneHanded;

			m_UsesRemaining = 0;
		}

		public Fukiya( Serial serial ) : base( serial )
		{
		}

		public void CheckName()
		{
			string name = m_Crafter != null ? m_Crafter.Name : "";

			if ( m_Crafter != null && m_Crafter.Fame >= 10000 )
			{
				string title = m_Crafter.Female ? "Lady" : "Lord";

				name = title + " " + name;
			}

			if ( name != "" )
			{
				m_Crafter_Name = name;
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Crafter != null && m_Crafter_Name != null && m_Crafter_Name != "" )
			{
				list.Add( 1050043, m_Crafter_Name );
			} // crafted by ~1_NAME~

			if ( m_Quality == FukiyaQuality.Exceptional )
			{
				list.Add( 1060636 );
			} // exceptional

			if ( m_Poison != null && m_PoisonCharges > 0 )
			{
				list.Add( 1062412 + m_Poison.Level, m_PoisonCharges.ToString() );
			}

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public override bool OnEquip( Mobile from )
		{
			from.SendLocalizedMessage( 1070785 ); // Double click this item each time you wish to throw a shuriken.

			return true;
		}

		public override void OnDoubleClick( Mobile from )
		{
			Item item = from.FindItemOnLayer( Layer.OneHanded );

			Item item2 = from.FindItemOnLayer( Layer.TwoHanded );

			if ( item != null || item2 != null )
			{
				from.SendLocalizedMessage( 1063327 ); // You must have a free hand to use a fukiya.

				return;
			}

			if ( UsesRemaining > 0 )
			{
				InternalTarget t = new InternalTarget( this );
				from.Target = t;
			}
			else
			{
				from.SendLocalizedMessage( 1063325 );
			} // You have no fukiya darts!
		}

		// Taken from BaseWeapon.cs
		// TODO: Make static function to use everywhere
		public static bool CheckHitChance( Mobile attacker, Mobile defender )
		{
//			BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
			BaseWeapon defWeapon = defender.Weapon as BaseWeapon;

			Skill atkSkill = attacker.Skills[ SkillName.Ninjitsu ];
			Skill defSkill = defender.Skills[ defWeapon.Skill ];

			double atkValue = atkSkill.Value;
			double defValue = defWeapon.GetDefendSkillValue( attacker, defender );

			//attacker.CheckSkill( atkSkill.SkillName, defValue - 20.0, 120.0 );
			//defender.CheckSkill( defSkill.SkillName, atkValue - 20.0, 120.0 );

			double ourValue, theirValue;

			int bonus = 0;

			if ( Core.AOS )
			{
				if ( atkValue <= -20.0 )
				{
					atkValue = -19.9;
				}

				if ( defValue <= -20.0 )
				{
					defValue = -19.9;
				}

				// Hit Chance Increase = 45%
				int atkChance = AosAttributes.GetValue( attacker, AosAttribute.AttackChance );
				if ( atkChance > 45 )
				{
					atkChance = 45;
				}

				bonus += atkChance;

				if ( attacker.BodyMod == 0xF6 || attacker.BodyMod == 0x19 )
				{
					bonus += (int) (attacker.Skills[ SkillName.Ninjitsu ].Value*0.1);
				} // TODO: verify

				if ( Spells.Chivalry.DivineFurySpell.UnderEffect( attacker ) )
				{
					bonus += 10;
				} // attacker gets 10% bonus when they're under divine fury

				if ( HitLower.IsUnderAttackEffect( attacker ) )
				{
					bonus -= 25;
				} // Under Hit Lower Attack effect -> 25% malus

				if ( LightningStrike.UnderEffect( attacker ) )
				{
					bonus = 45;
				}

				ourValue = (atkValue + 20.0)*(100 + bonus);

				// Defense Chance Increase = 45%
				bonus = AosAttributes.GetValue( defender, AosAttribute.DefendChance );
				if ( bonus > 45 )
				{
					bonus = 45;
				}

				if ( Spells.Chivalry.DivineFurySpell.UnderEffect( defender ) )
				{
					bonus -= 20;
				} // defender loses 20% bonus when they're under divine fury

				if ( HitLower.IsUnderDefenseEffect( defender ) )
				{
					bonus -= 25;
				} // Under Hit Lower Defense effect -> 25% malus


				if ( BaseWeapon.UnderSurprise( defender ) )
				{
					bonus -= 20;
				} // TODO: verify


				if ( Feint.UnderEffect( defender ) && defender.Combatant != null && defender.Combatant == attacker )
				{
					int chf = Utility.Random( 10, 15 );

					bonus += (int) ((chf/100.0)*bonus);
				}

				if ( Block.UnderEffect( defender ) )
				{
					int chb = Utility.Random( 10, 15 );

					bonus += (int) ((chb/100.0)*bonus);
				}

				double discordanceScalar = 0.0;

				if ( SkillHandlers.Discordance.GetScalar( attacker, ref discordanceScalar ) )
				{
					bonus += (int) (discordanceScalar*100);
				}

				theirValue = (defValue + 20.0)*(100 + bonus);

				bonus = 0;
			}
			else
			{
				if ( atkValue <= -50.0 )
				{
					atkValue = -49.9;
				}

				if ( defValue <= -50.0 )
				{
					defValue = -49.9;
				}

				ourValue = (atkValue + 50.0);
				theirValue = (defValue + 50.0);
			}

			double chance = ourValue/(theirValue*2.0);

			chance *= 1.0 + ((double) bonus/100);

			if ( Core.AOS && chance < 0.02 )
			{
				chance = 0.02;
			}

			WeaponAbility ability = WeaponAbility.GetCurrentAbility( attacker );

			if ( ability != null )
			{
				chance *= ability.AccuracyScalar;
			}

			return attacker.CheckSkill( atkSkill.SkillName, chance );

			//return ( chance >= Utility.RandomDouble() );

		}

		private class InternalTarget : Target
		{
			private Fukiya m_fukiya;

			public InternalTarget( Fukiya fukiya ) : base( 10, false, TargetFlags.Harmful )
			{
				m_fukiya = fukiya;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_fukiya.Deleted )
				{
					return;
				}
				else if ( targeted is Mobile )
				{
					Mobile m = (Mobile) targeted;

					double dist = from.GetDistanceToSqrt( m.Location );

					if ( m.Map != from.Map || dist > 5 )
					{
						from.SendLocalizedMessage( 500446 ); // That is too far away.
						return;
					}

					if ( m != from && from.HarmfulCheck( m ) )
					{
						Direction to = from.GetDirectionTo( m );

						from.Direction = to;

						from.RevealingAction();

						from.Animate( from.Mounted ? 26 : 9, 7, 1, true, false, 0 );

						if ( CheckHitChance( from, m ) )
						{
							from.MovingEffect( m, 0x2806, 7, 1, false, false, 0x23A, 0 );

							AOS.Damage( m, from, Utility.Random( 4, 6 ), 100, 0, 0, 0, 0 );

							if ( m_fukiya.Poison != null && m_fukiya.PoisonCharges > 0 )
							{
								--m_fukiya.PoisonCharges;

								m.ApplyPoison( from, m_fukiya.Poison );
							}
						}
						else
						{
							FukiyaDart dart = new FukiyaDart();

							from.MovingEffect( dart, 0x2804, 7, 1, false, false, 0x23A, 0 );

							from.SendMessage( "You miss." );

							dart.Delete();
						}

						m_fukiya.UsesRemaining--;
					}
				}
			}
		}

		public override void GetContextMenuEntries( Mobile from, ArrayList list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from == this.Parent || IsChildOf( from.Backpack ) )
			{
				list.Add( new LoadEntry( from, this ) );

				list.Add( new UnloadEntry( from, this, (m_UsesRemaining > 0) ) );
			}
		}

		private class LoadEntry : ContextMenuEntry
		{
			private Mobile mobile;

			private Fukiya fukiya;

			public LoadEntry( Mobile from, Fukiya m_fukiya ) : base( 6224, 3 )
			{
				mobile = from;

				fukiya = m_fukiya;
			}

			public override void OnClick()
			{
				mobile.Target = new InternalTarget( fukiya );
			}

			private class InternalTarget : Target
			{
				private Fukiya m_fukiya;

				public InternalTarget( Fukiya fukiya ) : base( 1, false, TargetFlags.None )
				{
					m_fukiya = fukiya;
				}

				protected override void OnTarget( Mobile from, object targeted )
				{
					if ( targeted is FukiyaDart )
					{
						FukiyaDart dart = targeted as FukiyaDart;

						if ( dart.Poison != null && dart.PoisonCharges > 0 )
						{
							m_fukiya.Poison = dart.Poison;

							m_fukiya.PoisonCharges = dart.PoisonCharges;
						}

						if ( m_fukiya.UsesRemaining < 10 )
						{
							if ( dart.UsesRemaining + m_fukiya.UsesRemaining >= 10 )
							{
								int need = 10 - m_fukiya.UsesRemaining;

								m_fukiya.UsesRemaining += need;

								dart.UsesRemaining -= need;

								if ( dart.UsesRemaining < 1 )
								{
									dart.Delete();
								}
							}
							else
							{
								m_fukiya.UsesRemaining += dart.UsesRemaining;

								dart.Delete();
							}
						}
						else
						{
							from.SendLocalizedMessage( 1063330 );
						} // You cannot add anymore fukiya darts
					}
					else
					{
						from.SendLocalizedMessage( 1063329 ); // You can only load fukiya darts
					}
				}
			}
		}

		private class UnloadEntry : ContextMenuEntry
		{
			private Mobile mobile;

			private Fukiya fukiya;

			public UnloadEntry( Mobile from, Fukiya m_fukiya, bool enabled ) : base( 6225, 3 )
			{
				mobile = from;

				fukiya = m_fukiya;

				if ( !enabled )
				{
					Flags |= Network.CMEFlags.Disabled;
				}
			}

			public override void OnClick()
			{
				FukiyaDart dart = new FukiyaDart( fukiya.UsesRemaining );

				fukiya.UsesRemaining = 0;

				mobile.AddToBackpack( dart );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 );

			writer.Write( (string) m_Crafter_Name );

			Poison.Serialize( m_Poison, writer );

			writer.Write( m_PoisonCharges );

			writer.Write( (int) m_UsesRemaining );

			writer.Write( m_Crafter );

			writer.WriteEncodedInt( (int) m_Quality );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
					{
						m_Crafter_Name = reader.ReadString();

						goto case 1;
					}
				case 1:
					{
						m_Poison = Poison.Deserialize( reader );

						m_PoisonCharges = reader.ReadInt();

						goto case 0;
					}
				case 0:
					{
						m_UsesRemaining = reader.ReadInt();

						m_Crafter = reader.ReadMobile();

						m_Quality = (FukiyaQuality) reader.ReadEncodedInt();

						break;
					}
			}

			if ( Layer != Layer.OneHanded )
			{
				Layer = Layer.OneHanded;
			}

			if ( UsesRemaining > 10 )
			{
				UsesRemaining = 10;
			}
		}

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (FukiyaQuality) quality;

			if ( makersMark )
			{
				Crafter = from;
			}

			return quality;
		}
	}
}