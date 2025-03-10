using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;
using Server.Engines.PartySystem;

namespace Server.Items
{
	public class ParagonChest : LockableContainer
	{
		public override int LabelNumber { get { return 3000541; } } // Treasure Chest

		public override bool UseLockedRestriction { get { return true; } }

		private static int[] m_ItemIDs = new int[]
		{
			0x9AB, 0xE40, 0xE41, 0xE7C
		};

		private static int[] m_Hues = new int[]
		{
			0x0, 0x455, 0x47E, 0x89F, 0x8A5, 0x8AB, 
			0x966, 0x96D, 0x972, 0x973, 0x979,   
		};

		private string m_Name;

		[Constructable]
		public ParagonChest( string name, int level ) : base( Utility.RandomList( m_ItemIDs ) )
		{
			m_Name = name;
			Hue = Utility.RandomList( m_Hues );
			Fill( level );
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, 1063449, m_Name ); // recovered from ~1_val~ (Paragon)
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1063449, m_Name );
		}

		private static void GetRandomAOSStats( out int attributeCount, out int min, out int max )
		{
			int rnd = Utility.Random( 15 );

			if ( rnd < 1 )
			{
				attributeCount = Utility.RandomMinMax( 2, 6 );
				min = 20;
				max = 70;
			}
			else if ( rnd < 3 )
			{
				attributeCount = Utility.RandomMinMax( 2, 4 );
				min = 20;
				max = 50;
			}
			else if ( rnd < 6 )
			{
				attributeCount = Utility.RandomMinMax( 2, 3 );
				min = 20;
				max = 40;
			}
			else if ( rnd < 10 )
			{
				attributeCount = Utility.RandomMinMax( 1, 2 );
				min = 10;
				max = 30;
			}
			else
			{
				attributeCount = 1;
				min = 10;
				max = 20;
			}
		}

		private void Fill( int level )
		{
			Enabled = true;
			TrapType = TrapType.ExplosionTrap;
			TrapPower = level*25;
			Locked = true;

			switch ( level )
			{
				case 1:
					RequiredSkill = 36;
					break;
				case 2:
					RequiredSkill = 76;
					break;
				case 3:
					RequiredSkill = 84;
					break;
				case 4:
					RequiredSkill = 92;
					break;
				case 5:
					RequiredSkill = 100;
					break;
			}

			LockLevel = RequiredSkill - 10;
			MaxLockLevel = RequiredSkill + 40;

			DropItem( new Gold( level*200 ) );

			for ( int i = 0; i < level; ++i )
			{
				DropItem( Loot.RandomScroll( 0, 63, SpellbookType.Regular ) );
			}

			for ( int i = 0; i < level*2; ++i )
			{
				Item item;

				if ( Core.AOS )
				{
					item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();
				}
				else
				{
					item = Loot.RandomArmorOrShieldOrWeapon();
				}

				if ( item is BaseWeapon )
				{
					BaseWeapon weapon = (BaseWeapon) item;

					if ( Core.AOS )
					{
						int attributeCount;
						int min, max;

						GetRandomAOSStats( out attributeCount, out min, out max );

						BaseRunicTool.ApplyAttributesTo( weapon, attributeCount, min, max );
					}
					else
					{
						weapon.DamageLevel = (WeaponDamageLevel) Utility.Random( 6 );
						weapon.AccuracyLevel = (WeaponAccuracyLevel) Utility.Random( 6 );
						weapon.DurabilityLevel = (WeaponDurabilityLevel) Utility.Random( 6 );
					}

					DropItem( item );
				}
				else if ( item is BaseArmor )
				{
					BaseArmor armor = (BaseArmor) item;

					if ( Core.AOS )
					{
						int attributeCount;
						int min, max;

						GetRandomAOSStats( out attributeCount, out min, out max );

						BaseRunicTool.ApplyAttributesTo( armor, attributeCount, min, max );
					}
					else
					{
						armor.ProtectionLevel = (ArmorProtectionLevel) Utility.Random( 6 );
						armor.Durability = (ArmorDurabilityLevel) Utility.Random( 6 );
					}

					DropItem( item );
				}
				else if ( item is BaseJewel )
				{
					int attributeCount;
					int min, max;

					GetRandomAOSStats( out attributeCount, out min, out max );

					BaseRunicTool.ApplyAttributesTo( (BaseJewel) item, attributeCount, min, max );

					DropItem( item );
				}
			}

			for ( int i = 0; i < level; i++ )
			{
				Item item = Loot.RandomPossibleReagent();
				item.Amount = Utility.RandomMinMax( 40, 60 );
				DropItem( item );
			}

			for ( int i = 0; i < level; i++ )
			{
				Item item = Loot.RandomGem();
				DropItem( item );
			}

			DropItem( new TreasureMap( level + 1, (Utility.RandomBool() ? Map.Felucca : Map.Trammel) ) );
		}

		public ParagonChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Name );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Name = reader.ReadString();
		}
	}
}