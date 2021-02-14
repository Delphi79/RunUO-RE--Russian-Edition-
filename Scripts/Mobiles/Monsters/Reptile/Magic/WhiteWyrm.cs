using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a white wyrm corpse" )]
	public class WhiteWyrm : BaseCreature
	{
		private static double turnChance = 0.05;
		private static double returnChance = 0.02;

		private ArrayList items;

		[Constructable]
		public WhiteWyrm() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = Core.AOS ? 180 : 49;
			Name = "a white wyrm";
			BaseSoundID = 362;

			SetStr( 721, 760 );
			SetDex( 101, 130 );
			SetInt( 386, 425 );

			SetHits( 433, 456 );

			SetDamage( 17, 25 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 55, 70 );
			SetResistance( ResistanceType.Fire, 15, 25 );
			SetResistance( ResistanceType.Cold, 80, 90 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.EvalInt, 99.1, 100.0 );
			SetSkill( SkillName.Magery, 99.1, 100.0 );
			SetSkill( SkillName.MagicResist, 99.1, 100.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 18000;
			Karma = -18000;

			VirtualArmor = 64;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 96.3;

			items = new ArrayList();
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
		}

		public override int TreasureMapLevel { get { return 4; } }
		public override int Meat { get { return 19; } }
		public override int Hides { get { return 20; } }
		public override HideType HideType { get { return HideType.Barbed; } }
		public override int Scales { get { return 9; } }
		public override ScaleType ScaleType { get { return ScaleType.White; } }
		public override FoodType FavoriteFood { get { return FoodType.Meat; } }

		public WhiteWyrm( Serial serial ) : base( serial )
		{
		}

		private void InitOutfit()
		{
			Item hair = BaseMobileHelper.GetRandomHair();
			items.Add( hair );

			if ( Utility.RandomBool() )
			{
				items.Add( BaseMobileHelper.GetRandomBeard( hair.Hue ) );
			}

			items.Add( BaseMobileHelper.GetRandomShirt() );
			items.Add( BaseMobileHelper.GetRandomPants() );
			items.Add( BaseMobileHelper.GetRandomFeet() );
		}

		public override void OnThink()
		{
			if ( Controled || Combatant != null )
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

					BaseMobileHelper.Turn( this, items, 0x190, Utility.RandomSkinHue(), null, "the mystic llamaherder", true );
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

			if ( Core.AOS && Body == 49 )
			{
				Body = 180;
			}

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