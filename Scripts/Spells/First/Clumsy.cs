using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.First
{
	public class ClumsySpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo( "Clumsy", "Uus Jux", SpellCircle.First, 212, 9031, Reagent.Bloodmoss, Reagent.Nightshade );

		public ClumsySpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( (int) this.Circle, Caster, ref m );

				SpellHelper.AddStatCurse( Caster, m, StatType.Dex );

				if ( m.Spell != null )
				{
					m.Spell.OnCasterHurt();
				}

				m.Paralyzed = false;

				m.FixedParticles( 0x3779, 10, 15, 5002, EffectLayer.Head );
				m.PlaySound( 0x1DF );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ClumsySpell m_Owner;

			public InternalTarget( ClumsySpell owner ) : base( 12, false, TargetFlags.Harmful )
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