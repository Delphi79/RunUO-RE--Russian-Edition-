using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ore elemental corpse" )]
	public class ShadowIronElemental : BaseCreature
	{
		[Constructable]
		public ShadowIronElemental() : this( 2 )
		{
		}

		[Constructable]
		public ShadowIronElemental( int oreAmount ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a shadow iron elemental";
			Body = 111;
			BaseSoundID = 268;

			SetStr( 226, 255 );
			SetDex( 126, 145 );
			SetInt( 71, 92 );

			SetHits( 136, 153 );

			SetDamage( 9, 16 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30, 40 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 50.1, 95.0 );
			SetSkill( SkillName.Tactics, 60.1, 100.0 );
			SetSkill( SkillName.Wrestling, 60.1, 100.0 );

			Fame = 4500;
			Karma = -4500;

			VirtualArmor = 23;

			PackItem( new ShadowIronOre( oreAmount ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Gems, 2 );
		}

		public override bool AutoDispel { get { return true; } }
		public override int TreasureMapLevel { get { return 1; } }

		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from is BaseCreature )
			{
				BaseCreature bc = (BaseCreature) from;

				if ( bc.Controled || bc.BardTarget == this )
				{
					damage = 0; // Immune to pets and provoked creatures
				}
			}
		}

		public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
		{
			scalar = 0.0; // Immune to magic
		}

		public ShadowIronElemental( Serial serial ) : base( serial )
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