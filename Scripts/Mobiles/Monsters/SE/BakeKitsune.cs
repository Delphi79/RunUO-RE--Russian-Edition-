using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Engines.Plants;

namespace Server.Mobiles
{
	[CorpseName( "a bake kitsune corpse" )]
	public class BakeKitsune : BaseCreature
	{
		private static int m_MinTime = 12;
		private static int m_MaxTime = 20;

		private DateTime m_NextAbilityTime;

		private RageTimer m_Timer;

		private static double turnChance = 0.05;
		private static double returnChance = 0.02;

		private ArrayList items;

		[Constructable]
		public BakeKitsune() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Bake Kitsune";
			Body = 246;

			SetStr( 170, 220 );
			SetDex( 125, 145 );
			SetInt( 375, 425 );

			SetHits( 310, 350 );

			SetDamage( 16, 20 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Energy, 30 );

			SetResistance( ResistanceType.Physical, 40, 60 );
			SetResistance( ResistanceType.Fire, 70, 90 );
			SetResistance( ResistanceType.Cold, 40, 60 );
			SetResistance( ResistanceType.Poison, 40, 60 );
			SetResistance( ResistanceType.Energy, 40, 60 );

			SetSkill( SkillName.EvalInt, 80.1, 90.0 );
			SetSkill( SkillName.Magery, 80.1, 90.0 );
			SetSkill( SkillName.Meditation, 85.1, 95.0 );
			SetSkill( SkillName.MagicResist, 80.1, 100.0 );
			SetSkill( SkillName.Tactics, 70.1, 90.0 );
			SetSkill( SkillName.Wrestling, 50.1, 55.0 );

			Fame = 200;
			Karma = -200;

			VirtualArmor = 15;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 80.7;

			PackGold( 700, 1000 );
			PackNecroReg( 3 );
			PackMagicItems( 1, 5 );

			AddItem( Seed.RandomBonsaiSeed() );

			items = new ArrayList();
		}

		public override int GetAngerSound()
		{
			return 0x4DF;
		}

		public override int GetIdleSound()
		{
			return 0x4DE;
		}

		public override int GetAttackSound()
		{
			return 0x4DD;
		}

		public override int GetHurtSound()
		{
			return 0x4E0;
		}

		public override int GetDeathSound()
		{
			return 0x4DC;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int Meat { get { return 3; } }
		public override int Hides { get { return 30; } }
		public override HideType HideType { get { return HideType.Regular; } }
		public override FoodType FavoriteFood { get { return FoodType.Fish; } }

		public BakeKitsune( Serial serial ) : base( serial )
		{
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( DateTime.Now >= m_NextAbilityTime )
			{
				if ( BaseAttackHelperSE.IsUnderEffect( defender, BaseAttackHelperSE.m_RageTable ) )
				{
					return;
				}

				BaseAttackHelperSE.RageAttack( this, defender, ref m_Timer );

				m_NextAbilityTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( m_MinTime, m_MaxTime ) );
			}

		}

		private void InitOutfit()
		{
			int[] hues = new int[] {0x1A8, 0xEC, 0x99, 0x90, 0xB5, 0x336, 0x89};

			items.Add( new Kasa() );
			items.Add( new TattsukeHakama( hues[ Utility.Random( hues.Length ) ] ) );
			items.Add( new HakamaShita( 0x2C3 ) );
			items.Add( new NinjaTabi( 0x2C3 ) );
		}

		public override void OnThink()
		{
			if ( Controled )
			{
				if ( BodyMod != 0 )
				{
					BaseMobileHelper.Return( this, items );
				}

				return;
			}

			if ( BodyMod != 0 )
			{
				if ( Utility.RandomDouble() < returnChance )
				{
					BaseMobileHelper.Return( this, items );
				}
			}
			else
			{
				if ( Utility.RandomDouble() < turnChance )
				{
					InitOutfit();

					BaseMobileHelper.Turn( this, items, 0x190, Utility.RandomSkinHue(), NameList.RandomName( "male" ), "the mystic traveller", true );
				}
			}

			base.OnThink();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 );
			writer.Write( (int) BodyMod );
			writer.WriteItemList( items );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
					{
						BodyMod = reader.ReadInt();
						goto case 1;
					}
				case 1:
					{
						items = reader.ReadItemList();
						break;
					}
				case 0:
					{
						items = new ArrayList();
						break;
					}
			}
		}
	}
}