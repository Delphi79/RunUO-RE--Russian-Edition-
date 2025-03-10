using System;
using System.Collections;

namespace Server.Items
{
	/// <summary>
	/// A successful Paralyzing Blow will leave the target stunned, unable to move, attack, or cast spells, for a few seconds.
	/// </summary>
	public class ParalyzingBlow : WeaponAbility
	{
		public ParalyzingBlow()
		{
		}

		public override int BaseMana { get { return 30; } }

		public static readonly TimeSpan PlayerFreezeDuration = TimeSpan.FromSeconds( 3.0 );
		public static readonly TimeSpan NPCFreezeDuration = TimeSpan.FromSeconds( 6.0 );

		// No longer active in pub21:
		/*public override bool CheckSkills( Mobile from )
		{
			if ( !base.CheckSkills( from ) )
				return false;

			if ( !(from.Weapon is Fists) )
				return true;

			Skill skill = from.Skills[SkillName.Anatomy];

			if ( skill != null && skill.Base >= 80.0 )
				return true;

			from.SendLocalizedMessage( 1061811 ); // You lack the required anatomy skill to perform that attack!

			return false;
		}*/

		public static Hashtable m_Table = new Hashtable();

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		private static void Expire_Callback( object state )
		{
			Mobile m = (Mobile) state;

			m_Table.Remove( m );
		}

		public override void OnHit( Mobile attacker, Mobile defender, int damage )
		{
			if ( defender.Frozen )
			{
				attacker.SendLocalizedMessage( 1061923 ); // The target is already frozen.
				return;
			}

			if ( !Validate( attacker ) || !CheckMana( attacker, true ) )
			{
				return;
			}

			ClearCurrentAbility( attacker );

			attacker.SendLocalizedMessage( 1060163 ); // You deliver a paralyzing blow!
			defender.SendLocalizedMessage( 1060164 ); // The attack has temporarily paralyzed you!

			defender.Freeze( defender.Player ? PlayerFreezeDuration : NPCFreezeDuration );

			Timer t = (Timer) m_Table[ defender ];

			if ( t != null )
			{
				t.Stop();
			}

			m_Table[ defender ] = t = Timer.DelayCall( defender.Player ? PlayerFreezeDuration : NPCFreezeDuration, new TimerStateCallback( Expire_Callback ), defender );

			defender.FixedEffect( 0x376A, 9, 32 );
			defender.PlaySound( 0x204 );
		}
	}
}