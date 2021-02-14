using System;
using Server;

namespace Server.Items
{
	/// <summary>
	/// Does damage and paralyzes your opponent for a short time. Requires Bushido skill.
	/// </summary>
	public class NerveStrike : WeaponAbility
	{
		public NerveStrike()
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

			int damages = Utility.Random( 20, 28 );

			AOS.Damage( defender, attacker, damages, 100, 0, 0, 0, 0 );

			attacker.SendLocalizedMessage( 1063356 ); // You cripple your target with a nerve strike!
			defender.SendLocalizedMessage( 1063357 ); // Your attacker dealt a crippling nerve strike!

			defender.Freeze( TimeSpan.FromSeconds( 2.0 ) );

			attacker.FixedParticles( 0x37C4, 1, 8, 0x13AF, 0, 0, EffectLayer.Waist );

			defender.FixedEffect( 0x376A, 9, 32 );
			defender.PlaySound( 0x204 );
		}
	}
}