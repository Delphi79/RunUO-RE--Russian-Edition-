using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a solen worker corpse" )]
	public class RedSolenWorker : BaseCreature
	{
		[Constructable]
		public RedSolenWorker() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a red solen worker";
			Body = 781;
			BaseSoundID = 959;

			SetStr( 96, 120 );
			SetDex( 81, 105 );
			SetInt( 36, 60 );

			SetHits( 58, 72 );

			SetDamage( 5, 7 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 60.0 );
			SetSkill( SkillName.Tactics, 65.0 );
			SetSkill( SkillName.Wrestling, 60.0 );

			Fame = 1500;
			Karma = -1500;

			VirtualArmor = 28;

			SolenHelper.PackPicnicBasket( this );
			PackGold( 50, 100 );
			PackItem( new ZoogiFungus( Utility.RandomMinMax( 1, 6 ) ) );
		}

		public override int GetAngerSound()
		{
			return 0x269;
		}

		public override int GetIdleSound()
		{
			return 0x269;
		}

		public override int GetAttackSound()
		{
			return 0x186;
		}

		public override int GetHurtSound()
		{
			return 0x1BE;
		}

		public override int GetDeathSound()
		{
			return 0x8E;
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( SolenHelper.CheckRedFriendship( m ) )
			{
				return false;
			}
			else
			{
				return base.IsEnemy( m );
			}
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			SolenHelper.OnRedDamage( from );

			base.OnDamage( amount, from, willKill );
		}

		public RedSolenWorker( Serial serial ) : base( serial )
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