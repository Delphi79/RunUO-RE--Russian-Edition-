using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "a juka corpse" )] // Why is this 'juka' and warriors 'jukan' ? :-(
		public class JukaMage : BaseCreature
	{
		[Constructable]
		public JukaMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a juka mage";
			Body = 765;

			SetStr( 201, 300 );
			SetDex( 71, 90 );
			SetInt( 451, 500 );

			SetHits( 121, 180 );

			SetDamage( 4, 10 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 20, 30 );
			SetResistance( ResistanceType.Fire, 35, 45 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 35, 45 );

			SetSkill( SkillName.Anatomy, 80.1, 90.0 );
			SetSkill( SkillName.EvalInt, 80.2, 100.0 );
			SetSkill( SkillName.Magery, 99.1, 100.0 );
			SetSkill( SkillName.Meditation, 80.2, 100.0 );
			SetSkill( SkillName.MagicResist, 140.1, 150.0 );
			SetSkill( SkillName.Tactics, 80.1, 90.0 );
			SetSkill( SkillName.Wrestling, 80.1, 90.0 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 16;

			Container bag = new Bag();

			int count = Utility.RandomMinMax( 10, 20 );

			for ( int i = 0; i < count; ++i )
			{
				Item item = Loot.RandomReagent();

				if ( item == null )
				{
					continue;
				}

				if ( !bag.TryDropItem( this, item, false ) )
				{
					item.Delete();
				}
			}

			PackItem( bag );

			PackItem( new ArcaneGem() );

			m_NextAbilityTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 2, 5 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int GetIdleSound()
		{
			return 0x1AC;
		}

		public override int GetAngerSound()
		{
			return 0x1CD;
		}

		public override int GetHurtSound()
		{
			return 0x1D0;
		}

		public override int GetDeathSound()
		{
			return 0x28D;
		}

		public override bool AlwaysMurderer { get { return true; } }
		public override bool CanRummageCorpses { get { return true; } }
		public override int Meat { get { return 1; } }

		private DateTime m_NextAbilityTime;

		public override void OnThink()
		{
			if ( DateTime.Now >= m_NextAbilityTime )
			{
				JukaLord toBuff = null;

				foreach ( Mobile m in this.GetMobilesInRange( 8 ) )
				{
					if ( m is JukaLord && IsFriend( m ) && m.Combatant != null && CanBeBeneficial( m ) && m.CanBeginAction( typeof( JukaMage ) ) && InLOS( m ) )
					{
						toBuff = (JukaLord) m;
						break;
					}
				}

				if ( toBuff != null )
				{
					if ( CanBeBeneficial( toBuff ) && toBuff.BeginAction( typeof( JukaMage ) ) )
					{
						m_NextAbilityTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 30, 60 ) );

						toBuff.Say( true, "Give me the power to destroy my enemies!" );
						this.Say( true, "Fight well my lord!" );

						DoBeneficial( toBuff );

						object[] state = new object[] {toBuff, toBuff.HitsMaxSeed, toBuff.RawStr, toBuff.RawDex};

						SpellHelper.Turn( this, toBuff );

						int toScale = toBuff.HitsMaxSeed;

						if ( toScale > 0 )
						{
							toBuff.HitsMaxSeed += AOS.Scale( toScale, 75 );
							toBuff.Hits += AOS.Scale( toScale, 75 );
						}

						toScale = toBuff.RawStr;

						if ( toScale > 0 )
						{
							toBuff.RawStr += AOS.Scale( toScale, 50 );
						}

						toScale = toBuff.RawDex;

						if ( toScale > 0 )
						{
							toBuff.RawDex += AOS.Scale( toScale, 50 );
							toBuff.Stam += AOS.Scale( toScale, 50 );
						}

						toBuff.Hits = toBuff.Hits;
						toBuff.Stam = toBuff.Stam;

						toBuff.FixedParticles( 0x375A, 10, 15, 5017, EffectLayer.Waist );
						toBuff.PlaySound( 0x1EE );

						Timer.DelayCall( TimeSpan.FromSeconds( 20.0 ), new TimerStateCallback( Unbuff ), state );
					}
				}
				else
				{
					m_NextAbilityTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 2, 5 ) );
				}
			}

			base.OnThink();
		}

		private void Unbuff( object state )
		{
			object[] states = (object[]) state;

			JukaLord toDebuff = (JukaLord) states[ 0 ];

			toDebuff.EndAction( typeof( JukaMage ) );

			if ( toDebuff.Deleted )
			{
				return;
			}

			toDebuff.HitsMaxSeed = (int) states[ 1 ];
			toDebuff.RawStr = (int) states[ 2 ];
			toDebuff.RawDex = (int) states[ 3 ];

			toDebuff.Hits = toDebuff.Hits;
			toDebuff.Stam = toDebuff.Stam;
		}

		public JukaMage( Serial serial ) : base( serial )
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