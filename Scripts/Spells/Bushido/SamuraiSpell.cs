using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Spells.Ninjitsu;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells.Bushido
{
	public abstract class SamuraiSpell : Spell
	{
		public abstract double RequiredSkill { get; }
		public abstract int RequiredMana { get; }

		public abstract int SpellNumber { get; }

		public override SkillName CastSkill { get { return SkillName.Bushido; } }

		public override bool ClearHandsOnCast { get { return false; } }
		public override bool BlocksMovement { get { return false; } }

		public override int CastDelayBase { get { return 0; } }
		public override int CastDelayCircleScalar { get { return 0; } }
		public override int CastDelayFastScalar { get { return 5; } }
		public override int CastDelayPerSecond { get { return 1; } }
		public override int CastDelayMinimum { get { return 0; } }

		public override int CastRecoveryBase { get { return 0; } }
		public override int CastRecoveryCircleScalar { get { return 0; } }
		public override int CastRecoveryFastScalar { get { return 5; } }
		public override int CastRecoveryPerSecond { get { return 1; } }
		public override int CastRecoveryMinimum { get { return 0; } }

		public SamuraiSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public static bool CheckExpansion( Mobile from )
		{
			if ( !(from is PlayerMobile) )
			{
				return true;
			}

			int flags = from.NetState == null ? 0 : from.NetState.Flags;

			if ( (flags & 0x10) != 0 )
			{
				return true;
			}

			return false;
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			{
				return false;
			}

			if ( !CheckExpansion( Caster ) )
			{
				Caster.SendLocalizedMessage( 1063456 ); // You must upgrade to Samurai Empire in order to use that ability.

				return false;
			}

			if ( Caster.Skills[ SkillName.Bushido ].Value < RequiredSkill )
			{
				string args = String.Format( "{0}\t{1}\t ", RequiredSkill.ToString( "F1" ), CastSkill.ToString() );
				Caster.SendLocalizedMessage( 1063013, args ); // You need at least ~1_SKILL_REQUIREMENT~ ~2_SKILL_NAME~ skill to use that ability.
				return false;
			}
			else if ( Caster.Mana < ScaleMana( RequiredMana ) )
			{
				Caster.SendLocalizedMessage( 1060174, RequiredMana.ToString() ); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}

			if ( HonorableExecution.UnderEffect( Caster ) || LightningStrike.UnderEffect( Caster ) || MomentumStrike.UnderEffect( Caster ) )
			{
				return true;
			}

			WeaponAbility ability = WeaponAbility.GetCurrentAbility( Caster );

			if ( ability != null )
			{
				WeaponAbility.ClearCurrentAbility( Caster );
			}

			if ( AnimalForm.UnderEffect( Caster ) )
			{
				if ( this is HonorableExecution || this is LightningStrike || this is MomentumStrike )
				{
					Caster.SendLocalizedMessage( 1063024 ); // You cannot perform this special move right now.

					return false;
				}
			}

			if ( FocusAttack.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0xF5, 0 ) );

				FocusAttack.m_Table.Remove( Caster );

				BaseWeapon weapon = Caster.Weapon as BaseWeapon;

				if ( weapon != null )
				{
					FocusAttack.RemoveBonus( weapon );
				}
			}

			if ( DeathStrike.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0xF6, 0 ) );

				DeathStrike.m_Table.Remove( Caster );
			}

			if ( KiAttack.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0xF8, 0 ) );

				KiAttack.m_Table.Remove( Caster );
			}

			if ( SurpriseAttack.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0xF9, 0 ) );

				SurpriseAttack.m_Table.Remove( Caster );
			}

			if ( Backstab.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0xFA, 0 ) );

				Backstab.m_Table.Remove( Caster );
			}

			return true;
		}

		public override bool CheckFizzle()
		{
			int mana = ScaleMana( RequiredMana );

			if ( Caster.Skills[ SkillName.Bushido ].Value < RequiredSkill )
			{
				Caster.SendLocalizedMessage( 1070768, RequiredSkill.ToString( "F1" ) ); // You need ~1_SKILL_REQUIREMENT~ Bushido skill to perform that attack!
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendLocalizedMessage( 1060174, RequiredMana.ToString() ); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}

			if ( !base.CheckFizzle() )
			{
				return false;
			}

			Caster.Mana -= mana;

			return true;
		}

		public override void DoFizzle()
		{
			Caster.Send( new SetNewSpell( SpellNumber, 0 ) );
			Caster.PlaySound( 0x1D6 );
			Caster.NextSpellTime = DateTime.Now;
		}

		public override void DoHurtFizzle()
		{
			Caster.PlaySound( 0x1D6 );
		}

		public override void OnDisturb( DisturbType type, bool message )
		{
			base.OnDisturb( type, message );

			if ( message )
			{
				Caster.PlaySound( 0x1D6 );
			}
		}

		public override void OnBeginCast()
		{
			base.OnBeginCast();

			if ( this is Confidence || this is Evasion || this is CounterAttack )
			{
				SendCastEffect();
			}
		}

		public virtual void SendCastEffect()
		{
			Caster.FixedEffect( 0x37C4, 10, 42, 4, 3 );
		}

		public override void GetCastSkills( out double min, out double max )
		{
			min = RequiredSkill;
			max = RequiredSkill + 50.0;
		}

		public override int GetMana()
		{
			return 0;
		}

		public static void FreeSamuraiSpells( Mobile m )
		{
			m.Send( new SetNewSpell( 0x91, 0 ) ); // honorable execution
			m.Send( new SetNewSpell( 0x92, 0 ) ); // confidence
			m.Send( new SetNewSpell( 0x93, 0 ) ); // evasion
			m.Send( new SetNewSpell( 0x94, 0 ) ); // counter attack
			m.Send( new SetNewSpell( 0x95, 0 ) ); // lightning strike
			m.Send( new SetNewSpell( 0x96, 0 ) ); // momentum strike
		}
	}
}