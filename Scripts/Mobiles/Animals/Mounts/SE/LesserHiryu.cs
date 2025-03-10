using System;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Plants;

namespace Server.Mobiles
{
	[CorpseName( "a lesser hiryu corpse" )]
	public class LesserHiryu : BaseMount
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.Dismount;
		}

		private static int m_MinTime = 5;
		private static int m_MaxTime = 9;

		private DateTime m_NextAbilityTime;

		private ExpireTimer m_Timer;

		[Constructable]
		public LesserHiryu() : this( "a Lesser Hiryu" )
		{
		}

		[Constructable]
		public LesserHiryu( string name ) : base( name, 243, 0x3E94, AIType.AI_Animal, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Hue = Utility.RandomList( 0x0, 0x8032, 0x8037, 0x803C, 0x8294, 0x847F, 0x8487, 0x848D, 0x8490, 0x8495, 0x88A0, 0x889F );

			SetStr( 300, 410 );
			SetDex( 170, 270 );
			SetInt( 300, 325 );

			SetHits( 400, 600 );
			SetMana( 60, 65 );
			SetStam( 170, 270 );

			SetDamage( 15, 27 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45, 70 );
			SetResistance( ResistanceType.Fire, 60, 80 );
			SetResistance( ResistanceType.Cold, 5, 15 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.Anatomy, 75.1, 80.0 );
			SetSkill( SkillName.MagicResist, 85.1, 100.0 );
			SetSkill( SkillName.Tactics, 100.1, 120.0 );
			SetSkill( SkillName.Wrestling, 100.1, 120.0 );

			SetFameLevel( 5 );
			SetKarmaLevel( 5 );

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 98.7;

			PackGold( 1300, 1900 );
			PackMagicItems( 1, 5 );

			AddItem( Seed.RandomBonsaiSeed() );
		}

		public override int GetAngerSound()
		{
			return 0x4FF;
		}

		public override int GetIdleSound()
		{
			return 0x4FE;
		}

		public override int GetAttackSound()
		{
			return 0x4FD;
		}

		public override int GetHurtSound()
		{
			return 0x500;
		}

		public override int GetDeathSound()
		{
			return 0x4FC;
		}

		public override double GetControlChance( Mobile m )
		{
			return 1.0;
		}

		public override int TreasureMapLevel { get { return 3; } }
		public override int Meat { get { return 16; } }
		public override int Hides { get { return 60; } }
		public override HideType HideType { get { return HideType.Regular; } }
		public override FoodType FavoriteFood { get { return FoodType.Meat; } }

		public LesserHiryu( Serial serial ) : base( serial )
		{
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Gems, 4 );
		}

		public override void OnThink()
		{
			if ( DateTime.Now >= m_NextAbilityTime )
			{
				BaseAttackHelperSE.HiryuAbilitiesAttack( this, ref m_Timer );

				m_NextAbilityTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( m_MinTime, m_MaxTime ) );
			}

			base.OnThink();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( FightMode == FightMode.Agressor )
			{
				FightMode = FightMode.Closest;
			}

			if ( version < 1 )
			{
				SetStam( 170, 270 );
			}
		}
	}
}