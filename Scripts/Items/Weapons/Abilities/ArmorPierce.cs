using System;
using Server;

namespace Server.Items
{
	/// <summary>
	/// Strike your opponent with great force, partially bypassing their armor and inflicting greater damage. Requires either Bushido or Ninjitsu skill, depending on the weapon.
	/// </summary>
	public class ArmorPierce : WeaponAbility
	{
		public ArmorPierce()
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

			attacker.SendLocalizedMessage( 1063350 ); // You pierce your opponent's armor!
			defender.SendLocalizedMessage( 1063351 ); // Your attacker pierced your armor!

			defender.FixedParticles( 0x3728, 1, 26, 0x26D6, 0, 0, EffectLayer.Waist );
		}
	}
}