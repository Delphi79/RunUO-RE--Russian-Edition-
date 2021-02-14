using System;
using Server;
using System.Collections;

namespace Server.Items
{
	/// <summary>
	/// Gain a defensive advantage over your primary opponent for a short time. Requires Bushido or Ninjitsu skill.
	/// </summary>
	public class Feint : WeaponAbility
	{
		public Feint()
		{
		}

		public override int BaseMana { get { return 30; } }

		private static Hashtable m_Table = new Hashtable();

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		private static void Expire_Callback( object state )
		{
			Mobile m = (Mobile) state;

			m_Table.Remove( m );
		}

		public override bool CheckSkills( Mobile from )
		{
			if ( !base.CheckSkills( from ) )
			{
				return false;
			}

			Skill bushido = from.Skills[ SkillName.Bushido ];

			if ( bushido != null && bushido.Value >= 50.0 )
			{
				return true;
			}

			from.SendLocalizedMessage( 1070768, 50.ToString() ); // You need ~1_SKILL_REQUIREMENT~ Bushido skill to perform that attack!

			return false;
		}

		public override void OnHit( Mobile attacker, Mobile defender, int damage )
		{
			if ( !Validate( attacker ) )
			{
				return;
			}
			if ( !CheckMana( attacker, true ) )
			{
				return;
			}
			ClearCurrentAbility( attacker );

			Timer t = (Timer) m_Table[ attacker ];

			if ( t != null )
			{
				t.Stop();
			}

			Skill bushido = attacker.Skills[ SkillName.Bushido ];

			int delay = (int) (bushido.Value/12.0);

			attacker.SendLocalizedMessage( 1063360 ); // You baffle your target with a feint!
			defender.SendLocalizedMessage( 1063361 ); // You were deceived by an attacker's feint!

			attacker.FixedParticles( 0x3728, 1, 13, 0x7F3, 0x962, 0, EffectLayer.Waist );

			m_Table[ attacker ] = t = Timer.DelayCall( TimeSpan.FromSeconds( delay ), new TimerStateCallback( Expire_Callback ), attacker );
		}
	}
}