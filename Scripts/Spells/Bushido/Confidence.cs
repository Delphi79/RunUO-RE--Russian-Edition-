using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Bushido
{
	public class Confidence : SamuraiSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Confidence", null, SpellCircle.Seventh, -1, 9002 );

		public override double RequiredSkill { get { return 25.0; } }
		public override int RequiredMana { get { return 10; } }

		public override int SpellNumber { get { return 0x92; } }

		public static Hashtable m_Table = new Hashtable();

		public static Hashtable m_Table2 = new Hashtable();

		public static Timer GetTimer( Mobile m )
		{
			return (Timer) m_Table2[ m ];
		}

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		public static bool UnderEffect2( Mobile m )
		{
			return m_Table2.Contains( m );
		}

		private static int ToHeal( Mobile m )
		{
			Skill skill = m.Skills[ SkillName.Bushido ];

			double v = skill.Value;

			return Math.Max(2, (int) ((15 + (v*v)/576)));
		}

		private static void Expire_Callback( object state )
		{
			Mobile m = (Mobile) state;

			m.Send( new SetNewSpell( 0x92, 0 ) );

			m.SendLocalizedMessage( 1063116 ); // Your confidence wanes.

			m_Table.Remove( m );
		}

		private class RegenTimer : Timer
		{
			private Mobile m;

			public RegenTimer( Mobile mobile ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 4.0 / (ToHeal( mobile ) - 1) ), ToHeal( mobile ) )
			{
				m = mobile;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if ( !m.Alive )
				{
					m_Table2.Remove( m );

					Stop();
				}

				if ( UnderEffect2( m ) )
				{
					m.Hits += 1;
				}
			}
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			{
				return false;
			}

			if ( Evasion.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0x93, 0 ) );

				Evasion.m_Table.Remove( Caster );
			}

			if ( CounterAttack.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0x94, 0 ) );

				CounterAttack.m_Table.Remove( Caster );
			}

			return true;
		}

		public Confidence( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				Caster.Send( new SetNewSpell( SpellNumber, 1 ) );

				Caster.SendLocalizedMessage( 1063115 ); // You exude confidence.

				Caster.FixedParticles( 0x375A, 1, 17, 0x7DA, 0x960, 0x3, EffectLayer.Waist );

				Timer t = (Timer) m_Table[ Caster ];

				if ( t != null )
				{
					t.Stop();
				}

				m_Table[ Caster ] = t = Timer.DelayCall( TimeSpan.FromSeconds( 30.0 ), new TimerStateCallback( Expire_Callback ), Caster );

				Timer regen = (Timer) m_Table2[ Caster ];

				if ( regen != null )
				{
					regen.Stop();
				}

				m_Table2[ Caster ] = regen = new RegenTimer( Caster );

				regen.Start();
			}

			FinishSequence();
		}
	}
}