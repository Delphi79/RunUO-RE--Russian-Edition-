using System;
using Server;
using System.Collections;

namespace Server.Items
{
	/// <summary>
	/// Raises your physical resistance for a short time while lowering your ability to inflict damage. Requires Bushido or Ninjitsu skill.
	/// </summary>
	public class DefenseMastery : WeaponAbility
	{
		public DefenseMastery()
		{
		}

		public override int BaseMana { get { return 30; } }

		private static Hashtable m_Table = new Hashtable();

		private static Mobile att;

		private static object[] mods;

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		private static void Expire_Callback( object state )
		{
			Mobile m = (Mobile) state;

			m_Table.Remove( m );

			att.RemoveResistanceMod( (ResistanceMod) mods[ 0 ] );
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

			attacker.SendLocalizedMessage( 1063353 ); // You perform a masterful defense!

			m_Table[ attacker ] = t = Timer.DelayCall( TimeSpan.FromSeconds( 3.0 ), new TimerStateCallback( Expire_Callback ), attacker );

			if ( mods == null )
			{
				mods = new object[1] {new ResistanceMod( ResistanceType.Physical, 70 )};

				m_Table[ attacker ] = mods;

				attacker.AddResistanceMod( (ResistanceMod) mods[ 0 ] );
			}

			att = attacker;

			attacker.FixedParticles( 0x375A, 1, 17, 0x7F2, 0x3E8, 0x3, EffectLayer.Waist );
		}
	}
}