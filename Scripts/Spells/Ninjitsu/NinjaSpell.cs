using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Spells.Bushido;
using Server.Items;

namespace Server.Spells.Ninjitsu
{
	public abstract class NinjaSpell : Spell
	{
		public abstract double RequiredSkill { get; }
		public abstract int RequiredMana { get; }

		public abstract int SpellNumber { get; }

		public override SkillName CastSkill { get { return SkillName.Ninjitsu; } }

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

		public NinjaSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			{
				return false;
			}

			if ( !SamuraiSpell.CheckExpansion( Caster ) )
			{
				Caster.SendLocalizedMessage( 1063456 ); // You must upgrade to Samurai Empire in order to use that ability.

				return false;
			}

			if ( Caster.Skills[ SkillName.Ninjitsu ].Value < RequiredSkill )
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

			if ( FocusAttack.UnderEffect( Caster ) || DeathStrike.UnderEffect( Caster ) || SurpriseAttack.UnderEffect( Caster ) || Backstab.UnderEffect( Caster ) || KiAttack.UnderEffect( Caster ) )
			{
				return true;
			}

			if ( AnimalForm.UnderEffect( Caster ) )
			{
				if ( this is FocusAttack || this is DeathStrike || this is KiAttack || this is SurpriseAttack || this is Backstab )
				{
					Caster.SendLocalizedMessage( 1063024 ); // You cannot perform this special move right now.

					return false;
				}
			}

			WeaponAbility ability = WeaponAbility.GetCurrentAbility( Caster );

			if ( ability != null )
			{
				WeaponAbility.ClearCurrentAbility( Caster );
			}

			if ( HonorableExecution.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0x91, 0 ) );

				HonorableExecution.m_Table.Remove( Caster );
			}

			if ( LightningStrike.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0x95, 0 ) );

				LightningStrike.m_Table.Remove( Caster );
			}

			if ( MomentumStrike.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0x96, 0 ) );

				MomentumStrike.m_Table.Remove( Caster );
			}

			return true;
		}

		public override bool CheckFizzle()
		{
			int mana = ScaleMana( RequiredMana );

			if ( Caster.Skills[ SkillName.Ninjitsu ].Value < RequiredSkill )
			{
				Caster.SendLocalizedMessage( 1063352, RequiredSkill.ToString( "F1" ) ); // You need ~1_SKILL_REQUIREMENT~ Ninjitsu skill to perform that attack!
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
			if ( this is Shadowjump || this is MirrorImage || this is AnimalForm )
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502632 ); // The spell fizzles.

				Caster.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );
			}

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

			if ( this is AnimalForm )
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

		public static void FreeNinjaSpells( Mobile m )
		{
			m.Send( new SetNewSpell( 0xFA, 0 ) ); // backstab
			m.Send( new SetNewSpell( 0xF6, 0 ) ); // death strike
			m.Send( new SetNewSpell( 0xF5, 0 ) ); // focus attack
			m.Send( new SetNewSpell( 0xF8, 0 ) ); // ki attack
			m.Send( new SetNewSpell( 0xF9, 0 ) ); // surprise attack
		}
	}
}