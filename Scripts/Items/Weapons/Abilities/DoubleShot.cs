using System;
using Server;

namespace Server.Items
{
	/// <summary>
	/// Send two arrows flying at your opponent if you're mounted. Requires Bushido or Ninjitsu skill.
	/// </summary>
	public class DoubleShot : WeaponAbility
	{
		public DoubleShot()
		{
		}

		public override int BaseMana { get { return 30; } }

		public override bool CheckSkills( Mobile from )
		{
			if ( !base.CheckSkills( from ) )
			{
				return false;
			}

			Skill bushido = from.Skills[ SkillName.Bushido ];

			Skill ninjitsu = from.Skills[ SkillName.Ninjitsu ];

			if ( (bushido != null && bushido.Value >= 50.0) || (ninjitsu != null && ninjitsu.Value >= 50.0) )
			{
				return true;
			}

			from.SendLocalizedMessage( 1063347, 50.ToString() ); // You need ~1_SKILL_REQUIREMENT~ Bushido or Ninjitsu skill to perform that attack!

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

			if ( !attacker.Mounted )
			{
				return;
			}

			attacker.SendLocalizedMessage( 1063348 ); // You launch two shots at once!
			defender.SendLocalizedMessage( 1063349 ); // You're attacked with a barrage of shots!			
			defender.FixedParticles( 0x374A, 1, 15, 0x13BE, 0x17, 0x7, EffectLayer.Head );
		}
	}
}