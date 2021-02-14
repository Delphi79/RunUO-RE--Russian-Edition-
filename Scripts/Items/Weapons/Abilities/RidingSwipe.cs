using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	/// <summary>
	/// If you are on foot, dismounts your opponent and damages the ethereal's rider or the live mount (which must be healed before ridden again).  If you are mounted, damages and stuns the mounted opponent. Requires Bushido skill.
	/// </summary>
	public class RidingSwipe : WeaponAbility
	{
		public RidingSwipe()
		{
		}

		public override int BaseMana { get { return 30; } }

		public static readonly TimeSpan BlockMountDuration = TimeSpan.FromSeconds( 10.0 ); // TODO: Taken from bola script, needs to be verified

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

			IMount mount_defender = defender.Mount;

			IMount mount_attacker = attacker.Mount;

			if ( mount_defender == null )
			{
				attacker.SendLocalizedMessage( 1060848 ); // This attack only works on mounted targets
				return;
			}

			if ( mount_attacker == null )
			{
				defender.FixedParticles( 0x376A, 9, 32, 0x13AF, 0, 0, EffectLayer.RightFoot );

				mount_defender.Rider = null;

				if ( mount_defender is Mobile )
				{
					Mobile m = (Mobile) mount_defender;

					int value = (int) (0.4*m.Hits);

					AOS.Damage( (Mobile) mount_defender, attacker, value, 100, 0, 0, 0, 0 );
				}

				defender.BeginAction( typeof( BaseMount ) );
				Timer.DelayCall( BlockMountDuration, new TimerStateCallback( ReleaseMountLock ), defender );
			}
			else
			{
				AOS.Damage( defender, attacker, Utility.Random( 5, 10 ), 100, 0, 0, 0, 0 );

				defender.Freeze( TimeSpan.FromSeconds( 3.0 ) ); // is correct?
			}
		}

		private void ReleaseMountLock( object state )
		{
			((Mobile) state).EndAction( typeof( BaseMount ) );
		}
	}
}