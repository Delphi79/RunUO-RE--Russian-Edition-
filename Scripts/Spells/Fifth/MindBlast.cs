using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fifth
{
	public class MindBlastSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo( "Mind Blast", "Por Corp Wis", SpellCircle.Fifth, 218, Core.AOS ? 9002 : 9032, Reagent.BlackPearl, Reagent.MandrakeRoot, Reagent.Nightshade, Reagent.SulfurousAsh );

		public MindBlastSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
			if ( Core.AOS )
			{
				m_Info.LeftHandEffect = m_Info.RightHandEffect = 9002;
			}
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		private void AosDelay_Callback( object state )
		{
			object[] states = (object[]) state;
			Mobile caster = (Mobile) states[ 0 ];
			Mobile target = (Mobile) states[ 1 ];
			Mobile defender = (Mobile) states[ 2 ];
			int damage = (int) states[ 3 ];

			if ( caster.HarmfulCheck( defender ) )
			{
				SpellHelper.Damage( this, target, Utility.RandomMinMax( damage, damage + 4 ), 0, 0, 100, 0, 0 );

				target.FixedParticles( 0x374A, 10, 15, 5038, 1181, 2, EffectLayer.Head );
				target.PlaySound( 0x213 );
			}
		}

		public override bool DelayedDamage { get { return !Core.AOS; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( Core.AOS )
			{
				if ( Caster.CanBeHarmful( m ) && CheckSequence() )
				{
					Mobile from = Caster, target = m;

					SpellHelper.Turn( from, target );

					SpellHelper.CheckReflect( (int) this.Circle, ref from, ref target );

					int damage = (int) ((Caster.Skills[ SkillName.Magery ].Value + Caster.Int)/5);

					if ( damage > 60 )
					{
						damage = 60;
					}

					Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( AosDelay_Callback ), new object[] {Caster, target, m, damage} );
				}
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile from = Caster, target = m;

				SpellHelper.Turn( from, target );

				SpellHelper.CheckReflect( (int) this.Circle, ref from, ref target );

				// Algorithm: (highestStat - lowestStat) / 2 [- 50% if resisted]

				int highestStat = target.Str, lowestStat = target.Str;

				if ( target.Dex > highestStat )
				{
					highestStat = target.Dex;
				}

				if ( target.Dex < lowestStat )
				{
					lowestStat = target.Dex;
				}

				if ( target.Int > highestStat )
				{
					highestStat = target.Int;
				}

				if ( target.Int < lowestStat )
				{
					lowestStat = target.Int;
				}

				if ( highestStat > 150 )
				{
					highestStat = 150;
				}

				if ( lowestStat > 150 )
				{
					lowestStat = 150;
				}

				int damage = (highestStat - lowestStat)/4; //less damage

				if ( damage > 45 )
				{
					damage = 45;
				}

				if ( CheckResisted( target ) )
				{
					damage /= 2;
					target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}

				from.FixedParticles( 0x374A, 10, 15, 2038, EffectLayer.Head );

				target.FixedParticles( 0x374A, 10, 15, 5038, EffectLayer.Head );
				target.PlaySound( 0x213 );

				SpellHelper.Damage( this, target, damage, 0, 0, 100, 0, 0 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private MindBlastSpell m_Owner;

			public InternalTarget( MindBlastSpell owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile) o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}