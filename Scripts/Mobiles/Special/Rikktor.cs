using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Spells;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
	public class Rikktor : BaseChampion
	{
		public override ChampionSkullType SkullType { get { return ChampionSkullType.Power; } }

		[Constructable]
		public Rikktor() : base( AIType.AI_ChampionMelee )
		{
			Body = 172;
			Name = "Rikktor";

			SetStr( 701, 900 );
			SetDex( 201, 350 );
			SetInt( 51, 100 );

			SetHits( 3000 );
			SetStam( 203, 650 );

			SetDamage( 28, 55 );

			SetDamageType( ResistanceType.Physical, 25 );
			SetDamageType( ResistanceType.Fire, 50 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 80, 90 );
			SetResistance( ResistanceType.Fire, 80, 90 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 80, 90 );
			SetResistance( ResistanceType.Energy, 80, 90 );

			SetSkill( SkillName.Anatomy, 100.0 );
			SetSkill( SkillName.MagicResist, 140.2, 160.0 );
			SetSkill( SkillName.Tactics, 100.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 130;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 3 );
		}

		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override ScaleType ScaleType { get { return ScaleType.All; } }
		public override int Scales { get { return 20; } }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( 0.3 >= Utility.RandomDouble() )
			{
				Earthquake();
			}
		}

		public void Earthquake()
		{
			Map map = this.Map;

			if ( map == null )
			{
				return;
			}

			ArrayList targets = new ArrayList();

			foreach ( Mobile m in this.GetMobilesInRange( 8 ) )
			{
				if ( m == this || !CanBeHarmful( m ) )
				{
					continue;
				}

				if ( m is BaseCreature && (((BaseCreature) m).Controled || ((BaseCreature) m).Summoned || ((BaseCreature) m).Team != this.Team) )
				{
					targets.Add( m );
				}
				else if ( m.Player )
				{
					targets.Add( m );
				}
			}

			PlaySound( 0x2F3 );

			for ( int i = 0; i < targets.Count; ++i )
			{
				Mobile m = (Mobile) targets[ i ];

				double damage = m.Hits*0.6;

				if ( damage < 10.0 )
				{
					damage = 10.0;
				}
				else if ( damage > 75.0 )
				{
					damage = 75.0;
				}

				DoHarmful( m );

				AOS.Damage( m, this, (int) damage, 100, 0, 0, 0, 0 );

				if ( m.Alive && m.Body.IsHuman && !m.Mounted )
				{
					m.Animate( 20, 7, 1, true, false, 0 ); // take hit
				}
			}
		}

		public override int GetAngerSound()
		{
			return Utility.Random( 0x2CE, 2 );
		}

		public override int GetIdleSound()
		{
			return 0x2D2;
		}

		public override int GetAttackSound()
		{
			return Utility.Random( 0x2C7, 5 );
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override int GetDeathSound()
		{
			return 0x2CC;
		}

		public Rikktor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}