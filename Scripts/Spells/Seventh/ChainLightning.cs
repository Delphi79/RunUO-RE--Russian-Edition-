using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Seventh
{
	public class ChainLightningSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo( "Chain Lightning", "Vas Ort Grav", SpellCircle.Seventh, 209, 9022, false, Reagent.BlackPearl, Reagent.Bloodmoss, Reagent.MandrakeRoot, Reagent.SulfurousAsh );

		public ChainLightningSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage { get { return true; } }

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				if ( p is Item )
				{
					p = ((Item) p).GetWorldLocation();
				}

				ArrayList targets = new ArrayList();

				Map map = Caster.Map;

				bool playerVsPlayer = false;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 2 );

					foreach ( Mobile m in eable )
					{
						if ( Core.AOS && m == Caster )
						{
							continue;
						}

						if ( SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) )
						{
							if ( Core.AOS && !Caster.InLOS( m ) )
							{
								continue;
							}

							targets.Add( m );

							if ( m.Player )
							{
								playerVsPlayer = true;
							}
						}
					}

					eable.Free();
				}

				double damage;

				if ( Core.AOS )
				{
					damage = GetNewAosDamage( 48, 1, 5, Caster.Player && playerVsPlayer );
				}
				else
				{
					damage = Utility.Random( 27, 22 );
				}

				if ( targets.Count > 0 )
				{
					if ( Core.AOS && targets.Count > 1 )
					{
						damage = (damage*2)/targets.Count;
					}
					else if ( !Core.AOS )
					{
						damage /= targets.Count;
					}

					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = (Mobile) targets[ i ];

						double toDeal = damage;

						if ( !Core.AOS && CheckResisted( m ) )
						{
							toDeal *= 0.5;

							m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}

						Caster.DoHarmful( m );
						SpellHelper.Damage( this, m, toDeal, 0, 0, 0, 0, 100 );

						m.BoltEffect( 0 );
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ChainLightningSpell m_Owner;

			public InternalTarget( ChainLightningSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if ( p != null )
				{
					m_Owner.Target( p );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}