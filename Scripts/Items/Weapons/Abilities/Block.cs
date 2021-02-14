using System;
using Server;
using System.Collections;

namespace Server.Items
{
	/// <summary>
	/// Raises your defenses for a short time. Requires Bushido or Ninjitsu skill.
	/// </summary>
	public class Block : WeaponAbility
	{
		public Block()
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

			Skill ninjitsu = from.Skills[ SkillName.Ninjitsu ];

			if ( ninjitsu != null && ninjitsu.Value >= 50.0 )
			{
				return true;
			}

			from.SendLocalizedMessage( 1063352, 50.ToString() ); // You need ~1_SKILL_REQUIREMENT~ Ninjitsu skill to perform that attack!

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

			Skill ninjitsu = attacker.Skills[ SkillName.Ninjitsu ];

			int delay = (int) (ninjitsu.Value/12.0);

			attacker.SendLocalizedMessage( 1063345 ); // You block an attack!
			defender.SendLocalizedMessage( 1063346 ); // Your attack was blocked!

			attacker.FixedParticles( 0x37C4, 1, 16, 0x251D, 0x39D, 0x3, EffectLayer.RightHand );

			m_Table[ attacker ] = t = Timer.DelayCall( TimeSpan.FromSeconds( delay ), new TimerStateCallback( Expire_Callback ), attacker );
		}
	}
}