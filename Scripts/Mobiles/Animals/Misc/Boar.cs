using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a pig corpse" )]
	public class Boar : BaseCreature
	{
		[Constructable]
		public Boar() : base( AIType.AI_Animal, FightMode.Agressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a boar";
			Body = 0x122;
			BaseSoundID = 0xC4;

			SetStr( 25 );
			SetDex( 15 );
			SetInt( 5 );

			SetHits( 15 );
			SetMana( 0 );

			SetDamage( 3, 6 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 15 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Poison, 5, 10 );

			SetSkill( SkillName.MagicResist, 9.0 );
			SetSkill( SkillName.Tactics, 9.0 );
			SetSkill( SkillName.Wrestling, 9.0 );

			Fame = 300;
			Karma = 0;

			VirtualArmor = 10;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 29.1;
		}

		public override int Meat { get { return 2; } }
		public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Boar( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}