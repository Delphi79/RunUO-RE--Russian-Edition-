using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	public class KhaldunRevenant : BaseCreature
	{
		private static Hashtable m_Table = new Hashtable();

		public static void Initialize()
		{
			EventSink.PlayerDeath += new PlayerDeathEventHandler( EventSink_PlayerDeath );
		}

		public static void EventSink_PlayerDeath( PlayerDeathEventArgs e )
		{
			Mobile m = e.Mobile;
			Mobile lastKiller = m.LastKiller;

			if ( lastKiller is BaseCreature )
			{
				lastKiller = ((BaseCreature) lastKiller).GetMaster();
			}

			if ( IsInsideKhaldun( m ) && IsInsideKhaldun( lastKiller ) && lastKiller.Player && !m_Table.Contains( lastKiller ) )
			{
				foreach ( AggressorInfo ai in m.Aggressors )
				{
					if ( ai.Attacker == lastKiller && ai.CanReportMurder )
					{
						SummonRevenant( m, lastKiller );
						break;
					}
				}
			}
		}

		public static void SummonRevenant( Mobile victim, Mobile killer )
		{
			KhaldunRevenant revenant = new KhaldunRevenant( killer );

			revenant.MoveToWorld( victim.Location, victim.Map );
			revenant.Combatant = killer;
			revenant.FixedParticles( 0, 0, 0, 0x13A7, EffectLayer.Waist );
			Effects.PlaySound( revenant.Location, revenant.Map, 0x29 );

			m_Table.Add( killer, null );
		}

		public static bool IsInsideKhaldun( Mobile from )
		{
			return from != null && from.Region != null && from.Region.Name == "Khaldun";
		}

		private Mobile m_Target;
		private DateTime m_ExpireTime;

		public override bool DeleteCorpseOnDeath { get { return true; } }

		public override void DisplayPaperdollTo( Mobile to )
		{
		}

		public override Mobile ConstantFocus { get { return m_Target; } }
		public override bool AlwaysAttackable { get { return true; } }

		public KhaldunRevenant( Mobile target ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.18, 0.36 )
		{
			Name = "a revenant";
			Body = 0x3CA;
			Hue = 0x41CE;

			m_Target = target;
			m_ExpireTime = DateTime.Now + TimeSpan.FromMinutes( 10.0 );

			SetStr( 401, 500 );
			SetDex( 296, 315 );
			SetInt( 101, 200 );

			SetHits( 241, 300 );
			SetStam( 242, 280 );

			SetDamage( 20, 30 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetSkill( SkillName.MagicResist, 100.1, 150.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Swords, 140.1, 150.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 60;

			Halberd weapon = new Halberd();
			weapon.Hue = 0x41CE;
			weapon.Movable = false;

			AddItem( weapon );
		}

		public override int GetIdleSound()
		{
			return 0x1BF;
		}

		public override int GetAngerSound()
		{
			return 0x107;
		}

		public override int GetDeathSound()
		{
			return 0xFD;
		}

		public override bool BardImmune { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override void OnThink()
		{
			if ( !m_Target.Alive || DateTime.Now > m_ExpireTime )
			{
				Delete();
				return;
			}

			//Combatant = m_Target;
			//FocusMob = m_Target;

			if ( AIObject != null )
			{
				AIObject.Action = ActionType.Combat;
			}

			base.OnThink();
		}

		public override bool OnBeforeDeath()
		{
			Effects.SendLocationEffect( Location, Map, 0x376A, 10, 1 );
			return true;
		}

		public override void OnDelete()
		{
			m_Table.Remove( m_Target );

			base.OnDelete();
		}

		public KhaldunRevenant( Serial serial ) : base( serial )
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

			Delete();
		}
	}
}