using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Engines.Craft;
using Server.Factions;
using Server.Mobiles;

namespace Server.Items
{
	public enum ClothingQuality
	{
		Low,
		Regular,
		Exceptional
	}

	public interface IArcaneEquip
	{
		bool IsArcane { get; }
		int CurArcaneCharges { get; set; }
		int MaxArcaneCharges { get; set; }
	}

	public abstract class BaseClothing : Item, IDyable, IScissorable, IFactionItem, ICraftable
	{
		#region Factions
		private FactionItem m_FactionState;

		public FactionItem FactionItemState
		{
			get { return m_FactionState; }
			set
			{
				m_FactionState = value;

				if ( m_FactionState == null )
				{
					Hue = 0;
				}

				LootType = (m_FactionState == null ? LootType.Regular : LootType.Blessed);
			}
		}
		#endregion

		private Mobile m_Crafter;
		private ClothingQuality m_Quality;
		private bool m_PlayerConstructed;
		protected CraftResource m_Resource;

		private AosAttributes m_AosAttributes;
		private AosArmorAttributes m_AosClothingAttributes;
		private AosSkillBonuses m_AosSkillBonuses;
		private AosElementAttributes m_AosResistances;

		private string m_Crafter_Name;

		public virtual bool AllowMaleWearer { get { return true; } }
		public virtual bool AllowFemaleWearer { get { return true; } }

		private int m_StrReq = 10; // at this moment all clothings have 10 str req. So don't serialize now; Is it correct OSI?

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrRequirement
		{
			get { return m_StrReq; }
			set
			{
				m_StrReq = value;
				InvalidateProperties();
			}
		}

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
		public ClothingQuality Quality
		{
			get { return m_Quality; }
			set
			{
				m_Quality = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool PlayerConstructed { get { return m_PlayerConstructed; } set { m_PlayerConstructed = value; } }

		public virtual CraftResource DefaultResource { get { return CraftResource.None; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get { return m_Resource; }
			set
			{
				m_Resource = value;
				Hue = CraftResources.GetHue( m_Resource );
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes Attributes
		{
			get { return m_AosAttributes; }
			set
			{
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosArmorAttributes ClothingAttributes
		{
			get { return m_AosClothingAttributes; }
			set
			{
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get { return m_AosSkillBonuses; }
			set
			{
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosElementAttributes Resistances
		{
			get { return m_AosResistances; }
			set
			{
			}
		}

		public virtual int InitMinHits { get { return 0; } }
		public virtual int InitMaxHits { get { return 0; } }

		private int m_MaxHitPoints;
		private int m_HitPoints;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxHitPoints
		{
			get { return m_MaxHitPoints; }
			set
			{
				m_MaxHitPoints = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitPoints
		{
			get { return m_HitPoints; }
			set
			{
				if ( value != m_HitPoints && MaxHitPoints > 0 )
				{
					m_HitPoints = value;

					if ( m_HitPoints < 0 )
					{
						Delete();
					}
					else if ( m_HitPoints > MaxHitPoints )
					{
						m_HitPoints = MaxHitPoints;
					}

					InvalidateProperties();
				}
			}
		}

		public virtual int BasePhysicalResistance { get { return 0; } }
		public virtual int BaseFireResistance { get { return 0; } }
		public virtual int BaseColdResistance { get { return 0; } }
		public virtual int BasePoisonResistance { get { return 0; } }
		public virtual int BaseEnergyResistance { get { return 0; } }

		public override int PhysicalResistance { get { return BasePhysicalResistance + m_AosResistances.Physical; } }
		public override int FireResistance { get { return BaseFireResistance + m_AosResistances.Fire; } }
		public override int ColdResistance { get { return BaseColdResistance + m_AosResistances.Cold; } }
		public override int PoisonResistance { get { return BasePoisonResistance + m_AosResistances.Poison; } }
		public override int EnergyResistance { get { return BaseEnergyResistance + m_AosResistances.Energy; } }

		public virtual int ArtifactRarity { get { return 0; } }

		public virtual int BaseStrBonus { get { return 0; } }
		public virtual int BaseDexBonus { get { return 0; } }
		public virtual int BaseIntBonus { get { return 0; } }

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

		public int ComputeStatBonus( StatType type )
		{
			if ( type == StatType.Str )
			{
				return BaseStrBonus + Attributes.BonusStr;
			}
			else if ( type == StatType.Dex )
			{
				return BaseDexBonus + Attributes.BonusDex;
			}
			else
			{
				return BaseIntBonus + Attributes.BonusInt;
			}
		}

		public virtual void AddStatBonuses( Mobile parent )
		{
			if ( parent == null )
			{
				return;
			}

			int strBonus = ComputeStatBonus( StatType.Str );
			int dexBonus = ComputeStatBonus( StatType.Dex );
			int intBonus = ComputeStatBonus( StatType.Int );

			if ( strBonus == 0 && dexBonus == 0 && intBonus == 0 )
			{
				return;
			}

			string modName = this.Serial.ToString();

			if ( strBonus != 0 )
			{
				parent.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );
			}

			if ( dexBonus != 0 )
			{
				parent.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );
			}

			if ( intBonus != 0 )
			{
				parent.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}
		}

		public virtual int OnHit( BaseWeapon weapon, int damageTaken )
		{
			int Absorbed = 2; // TODO: verify

			damageTaken -= Absorbed;
			if ( damageTaken < 0 )
			{
				damageTaken = 0;
			}

			if ( 25 > Utility.Random( 100 ) ) // 25% chance to lower durability
			{
				if ( Core.AOS && m_AosClothingAttributes.SelfRepair > Utility.Random( 10 ) )
				{
					HitPoints += 2;
				}
				else
				{
					int wear;

					if ( weapon.Type == WeaponType.Bashing )
					{
						wear = Absorbed/2;
					}
					else
					{
						wear = Utility.Random( 2 );
					}

					if ( wear > 0 && m_MaxHitPoints > 0 )
					{
						if ( m_HitPoints >= wear )
						{
							HitPoints -= wear;
							wear = 0;
						}
						else
						{
							wear -= HitPoints;
							HitPoints = 0;
						}

						if ( wear > 0 )
						{
							if ( m_MaxHitPoints > wear )
							{
								MaxHitPoints -= wear;

								if ( Parent is Mobile )
								{
									((Mobile) Parent).LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061121 ); // Your equipment is severely damaged.
								}
							}
							else
							{
								Delete();
							}
						}
					}
				}
			}

			return damageTaken;
		}

		public override bool OnEquip( Mobile from )
		{
			return base.OnEquip( from );
		}

		public override void OnAdded( object parent )
		{
			Mobile mob = parent as Mobile;

			if ( mob != null )
			{
				if ( Core.AOS )
				{
					m_AosSkillBonuses.AddTo( mob );
				}

				AddStatBonuses( mob );
				mob.CheckStatTimers();
			}

			base.OnAdded( parent );
		}

		public override void OnRemoved( object parent )
		{
			Mobile mob = parent as Mobile;

			if ( mob != null )
			{
				if ( Core.AOS )
				{
					m_AosSkillBonuses.Remove();
				}

				string modName = this.Serial.ToString();

				mob.RemoveStatMod( modName + "Str" );
				mob.RemoveStatMod( modName + "Dex" );
				mob.RemoveStatMod( modName + "Int" );

				mob.CheckStatTimers();
			}

			base.OnRemoved( parent );
		}

		public BaseClothing( int itemID, Layer layer ) : this( itemID, layer, 0 )
		{
		}

		public BaseClothing( int itemID, Layer layer, int hue ) : base( itemID )
		{
			Layer = layer;
			Hue = hue;

			m_Resource = DefaultResource;
			m_Quality = ClothingQuality.Regular;

			m_AosAttributes = new AosAttributes( this );
			m_AosClothingAttributes = new AosArmorAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );
			m_AosResistances = new AosElementAttributes( this );

			m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax( InitMinHits, InitMaxHits );
		}

		public BaseClothing( Serial serial ) : base( serial )
		{
		}

		public static void ValidateMobile( Mobile m )
		{
			for ( int i = m.Items.Count - 1; i >= 0; --i )
			{
				if ( i >= m.Items.Count )
				{
					continue;
				}

				Item item = (Item) m.Items[ i ];

				if ( item is BaseClothing )
				{
					BaseClothing clothing = (BaseClothing) item;

					if ( !clothing.AllowMaleWearer && m.Body.IsMale && m.AccessLevel < AccessLevel.GameMaster )
					{
						if ( clothing.AllowFemaleWearer )
						{
							m.SendLocalizedMessage( 1010388 ); // Only females can wear this.
						}
						else
						{
							m.SendMessage( "You may not wear this." );
						}

						m.AddToBackpack( clothing );
					}
					else if ( !clothing.AllowFemaleWearer && m.Body.IsFemale && m.AccessLevel < AccessLevel.GameMaster )
					{
						if ( clothing.AllowMaleWearer )
						{
							m.SendMessage( "Only males can wear this." );
						}
						else
						{
							m.SendMessage( "You may not wear this." );
						}

						m.AddToBackpack( clothing );
					}
				}
			}
		}

		public override bool AllowEquipedCast( Mobile from )
		{
			if ( base.AllowEquipedCast( from ) )
			{
				return true;
			}

			return (m_AosAttributes.SpellChanneling != 0);
		}

		public override bool CheckPropertyConfliction( Mobile m )
		{
			if ( base.CheckPropertyConfliction( m ) )
			{
				return true;
			}

			if ( Layer == Layer.Pants )
			{
				return (m.FindItemOnLayer( Layer.InnerLegs ) != null);
			}

			if ( Layer == Layer.Shirt )
			{
				return (m.FindItemOnLayer( Layer.InnerTorso ) != null);
			}

			return false;
		}

		private string GetNameString()
		{
			string name = this.Name;

			if ( name == null )
			{
				name = String.Format( "#{0}", LabelNumber );
			}

			return name;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			int oreType;

			if ( Hue == 0 )
			{
				oreType = 0;
			}
			else
			{
				switch ( m_Resource )
				{
					case CraftResource.DullCopper:
						oreType = 1053108;
						break; // dull copper
					case CraftResource.ShadowIron:
						oreType = 1053107;
						break; // shadow iron
					case CraftResource.Copper:
						oreType = 1053106;
						break; // copper
					case CraftResource.Bronze:
						oreType = 1053105;
						break; // bronze
					case CraftResource.Gold:
						oreType = 1053104;
						break; // golden
					case CraftResource.Agapite:
						oreType = 1053103;
						break; // agapite
					case CraftResource.Verite:
						oreType = 1053102;
						break; // verite
					case CraftResource.Valorite:
						oreType = 1053101;
						break; // valorite
					case CraftResource.SpinedLeather:
						oreType = 1061118;
						break; // spined
					case CraftResource.HornedLeather:
						oreType = 1061117;
						break; // horned
					case CraftResource.BarbedLeather:
						oreType = 1061116;
						break; // barbed
					case CraftResource.RedScales:
						oreType = 1060814;
						break; // red
					case CraftResource.YellowScales:
						oreType = 1060818;
						break; // yellow
					case CraftResource.BlackScales:
						oreType = 1060820;
						break; // black
					case CraftResource.GreenScales:
						oreType = 1060819;
						break; // green
					case CraftResource.WhiteScales:
						oreType = 1060821;
						break; // white
					case CraftResource.BlueScales:
						oreType = 1060815;
						break; // blue
					default:
						oreType = 0;
						break;
				}
			}

			if ( oreType != 0 )
			{
				list.Add( 1053099, "#{0}\t{1}", oreType, GetNameString() ); // ~1_oretype~ ~2_armortype~
			}
			else if ( Name == null )
			{
				list.Add( LabelNumber );
			}
			else
			{
				list.Add( Name );
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Crafter != null && m_Crafter_Name != null && m_Crafter_Name != "" )
			{
				list.Add( 1050043, m_Crafter_Name ); // crafted by ~1_NAME~
			}

			#region Factions
			if ( m_FactionState != null )
			{
				list.Add( 1041350 ); // faction item
			}
			#endregion

			if ( m_Quality == ClothingQuality.Exceptional )
			{
				list.Add( 1060636 ); // exceptional
			}

			m_AosSkillBonuses.GetProperties( list );

			int prop;

			if ( (prop = ArtifactRarity) > 0 )
			{
				list.Add( 1061078, prop.ToString() ); // artifact rarity ~1_val~
			}

			if ( (prop = m_AosAttributes.WeaponDamage) != 0 )
			{
				list.Add( 1060401, prop.ToString() ); // damage increase ~1_val~%
			}

			if ( (prop = m_AosAttributes.DefendChance) != 0 )
			{
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%
			}

			if ( (prop = m_AosAttributes.BonusDex) != 0 )
			{
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~
			}

			if ( (prop = m_AosAttributes.EnhancePotions) != 0 )
			{
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%
			}

			if ( (prop = m_AosAttributes.CastRecovery) != 0 )
			{
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~
			}

			if ( (prop = m_AosAttributes.CastSpeed) != 0 )
			{
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~
			}

			if ( (prop = m_AosAttributes.AttackChance) != 0 )
			{
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%
			}

			if ( (prop = m_AosAttributes.BonusHits) != 0 )
			{
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~
			}

			if ( (prop = m_AosAttributes.BonusInt) != 0 )
			{
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~
			}

			if ( (prop = m_AosAttributes.LowerManaCost) != 0 )
			{
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%
			}

			if ( (prop = m_AosAttributes.LowerRegCost) != 0 )
			{
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%
			}

			if ( (prop = m_AosClothingAttributes.LowerStatReq) != 0 )
			{
				list.Add( 1060435, prop.ToString() ); // lower requirements ~1_val~%
			}

			if ( (prop = m_AosAttributes.Luck) != 0 )
			{
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~
			}

			if ( (prop = m_AosClothingAttributes.MageArmor) != 0 )
			{
				list.Add( 1060437 ); // mage armor
			}

			if ( (prop = m_AosAttributes.BonusMana) != 0 )
			{
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~
			}

			if ( (prop = m_AosAttributes.RegenMana) != 0 )
			{
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~
			}

			if ( (prop = m_AosAttributes.NightSight) != 0 )
			{
				list.Add( 1060441 ); // night sight
			}

			if ( (prop = m_AosAttributes.ReflectPhysical) != 0 )
			{
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%
			}

			if ( (prop = m_AosAttributes.RegenStam) != 0 )
			{
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~
			}

			if ( (prop = m_AosAttributes.RegenHits) != 0 )
			{
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~
			}

			if ( (prop = m_AosClothingAttributes.SelfRepair) != 0 )
			{
				list.Add( 1060450, prop.ToString() ); // self repair ~1_val~
			}

			if ( (prop = m_AosAttributes.SpellChanneling) != 0 )
			{
				list.Add( 1060482 ); // spell channeling
			}

			if ( (prop = m_AosAttributes.SpellDamage) != 0 )
			{
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%
			} 

			if ( (prop = m_AosAttributes.BonusStam) != 0 )
			{
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~
			}

			if ( (prop = m_AosAttributes.BonusStr) != 0 )
			{
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~
			}

			if ( (prop = m_AosAttributes.WeaponSpeed) != 0 )
			{
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%
			}

			base.AddResistanceProperties( list );

			if ( (prop = m_AosClothingAttributes.DurabilityBonus) > 0 )
			{
				list.Add( 1060410, prop.ToString() ); // durability ~1_val~%
			}

			if ( m_HitPoints >= 0 && m_MaxHitPoints > 0 )
			{
				list.Add( 1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints ); // durability ~1_val~ / ~2_val~
			}

			int strReq = StrRequirement;

			if ( strReq > 0 )
			{
				list.Add( 1061170, strReq.ToString() ); // strength requirement ~1_val~
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			ArrayList attrs = new ArrayList();

			if ( DisplayLootType )
			{
				if ( LootType == LootType.Blessed )
				{
					attrs.Add( new EquipInfoAttribute( 1038021 ) ); // blessed
				}
				else if ( LootType == LootType.Cursed )
				{
					attrs.Add( new EquipInfoAttribute( 1049643 ) ); // cursed
				}
			}

			#region Factions
			if ( m_FactionState != null )
			{
				attrs.Add( new EquipInfoAttribute( 1041350 ) ); // faction item
			}
			#endregion

			if ( m_Quality == ClothingQuality.Exceptional )
			{
				attrs.Add( new EquipInfoAttribute( 1018305 - (int) m_Quality ) );
			}

			int number;

			if ( Name == null )
			{
				number = LabelNumber;
			}
			else
			{
				this.LabelTo( from, Name );
				number = 1041000;
			}

			if ( attrs.Count == 0 && Crafter == null && Name != null )
			{
				return;
			}

			EquipmentInfo eqInfo = new EquipmentInfo( number, m_Crafter, false, (EquipInfoAttribute[]) attrs.ToArray( typeof( EquipInfoAttribute ) ) );

			from.Send( new DisplayEquipmentInfo( this, eqInfo ) );
		}

		public override bool CanEquip( Mobile from )
		{
			if ( !AllowMaleWearer && from.Body.IsMale && from.AccessLevel < AccessLevel.GameMaster )
			{
				if ( AllowFemaleWearer )
				{
					from.SendLocalizedMessage( 1010388 ); // Only females can wear this.
				}
				else
				{
					from.SendMessage( "You may not wear this." );
				}

				return false;
			}
			else if ( !AllowFemaleWearer && from.Body.IsFemale && from.AccessLevel < AccessLevel.GameMaster )
			{
				if ( AllowMaleWearer )
				{
					from.SendMessage( "Only males can wear this." );
				}
				else
				{
					from.SendMessage( "You may not wear this." );
				}

				return false;
			}
			else if ( from.Str < StrRequirement )
			{
				from.SendLocalizedMessage( 500213 ); // You are not strong enough to equip that.
				return false;
			}
			else
			{
				return base.CanEquip( from );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 6 ); // version

			writer.Write( (string) m_Crafter_Name );

			writer.Write( (int) m_MaxHitPoints );
			writer.Write( (int) m_HitPoints );

			writer.Write( (int) m_Resource );

			m_AosAttributes.Serialize( writer );
			m_AosClothingAttributes.Serialize( writer );
			m_AosSkillBonuses.Serialize( writer );
			m_AosResistances.Serialize( writer );

			writer.Write( (bool) m_PlayerConstructed );

			writer.Write( (Mobile) m_Crafter );
			writer.Write( (int) m_Quality );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 6:
					{
						m_Crafter_Name = reader.ReadString();

						goto case 5;
					}
				case 5:
					{
						m_MaxHitPoints = reader.ReadInt();
						m_HitPoints = reader.ReadInt();

						goto case 4;
					}
				case 4:
					{
						m_Resource = (CraftResource) reader.ReadInt();

						goto case 3;
					}
				case 3:
					{
						m_AosAttributes = new AosAttributes( this, reader );
						m_AosClothingAttributes = new AosArmorAttributes( this, reader );
						m_AosSkillBonuses = new AosSkillBonuses( this, reader );
						m_AosResistances = new AosElementAttributes( this, reader );

						goto case 2;
					}
				case 2:
					{
						m_PlayerConstructed = reader.ReadBool();
						goto case 1;
					}
				case 1:
					{
						m_Crafter = reader.ReadMobile();
						m_Quality = (ClothingQuality) reader.ReadInt();
						break;
					}
				case 0:
					{
						m_Crafter = null;
						m_Quality = ClothingQuality.Regular;
						break;
					}
			}

			if ( version < 2 )
			{
				m_PlayerConstructed = true; // we don't know, so, assume it's crafted
			}

			if ( version < 3 )
			{
				m_AosAttributes = new AosAttributes( this );
				m_AosClothingAttributes = new AosArmorAttributes( this );
				m_AosSkillBonuses = new AosSkillBonuses( this );
				m_AosResistances = new AosElementAttributes( this );
			}

			if ( version < 4 )
			{
				m_Resource = DefaultResource;
			}

			Mobile parent = Parent as Mobile;

			if ( parent != null )
			{
				if ( Core.AOS )
				{
					m_AosSkillBonuses.AddTo( parent );
				}

				AddStatBonuses( parent );
				parent.CheckStatTimers();
			}
		}

		public virtual bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
			{
				return false;
			}
			else if ( RootParent is Mobile && from != RootParent )
			{
				return false;
			}

			Hue = sender.DyedHue;

			return true;
		}

		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 502437 ); // Items you wish to cut must be in your backpack.
				return false;
			}

			CraftSystem system = DefTailoring.CraftSystem;

			CraftItem item = system.CraftItems.SearchFor( GetType() );

			if ( item != null && item.Ressources.Count == 1 && item.Ressources.GetAt( 0 ).Amount >= 2 )
			{
				try
				{
					Type resourceType = null;

					CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

					if ( info != null && info.ResourceTypes.Length > 0 )
					{
						resourceType = info.ResourceTypes[ 0 ];
					}

					if ( resourceType == null )
					{
						resourceType = item.Ressources.GetAt( 0 ).ItemType;
					}

					Item res = (Item) Activator.CreateInstance( resourceType );

					ScissorHelper( from, res, m_PlayerConstructed ? (item.Ressources.GetAt( 0 ).Amount/2) : 1 );

					res.LootType = LootType.Regular;

					return true;
				} 
				catch
				{
				}
			}

			from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
			return false;
		}

		#region ICraftable Members
		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ClothingQuality) quality;

			if ( makersMark )
			{
				Crafter = from;
			}

			if ( DefaultResource != CraftResource.None )
			{
				Type resourceType = typeRes;

				if ( resourceType == null )
				{
					resourceType = craftItem.Ressources.GetAt( 0 ).ItemType;
				}

				Resource = CraftResources.GetFromType( resourceType );
			}
			else
			{
				Hue = resHue;
			}

			PlayerConstructed = true;

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
			{
				Hue = 0;
			}

			return quality;
		}
		#endregion
	}
}