using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells.Necromancy;

namespace Server.Spells.Chivalry
{
	public class DispelEvilSpell : PaladinSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Dispel Evil", "Dispiro Malas", SpellCircle.Second, -1, 9002 );

		public override double RequiredSkill { get { return 35.0; } }
		public override int RequiredMana { get { return 10; } }
		public override int RequiredTithing { get { return 10; } }
		public override int MantraNumber { get { return 1060721; } } // Dispiro Malas
		public override bool BlocksMovement { get { return false; } }

		public DispelEvilSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamage { get { return false; } }

		public override void SendCastEffect()
		{
			Caster.FixedEffect( 0x37C4, 10, 7, 4, 3 ); // At player
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				ArrayList targets = new ArrayList();

				foreach ( Mobile m in Caster.GetMobilesInRange( 8 ) )
				{
					if ( Caster != m && SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) )
					{
						targets.Add( m );
					}
				}

				Caster.PlaySound( 0x299 );
				Caster.FixedParticles( 0x37C4, 1, 25, 9922, 14, 3, EffectLayer.Head );

				int dispelSkill = ComputePowerValue( 2 );

				double chiv = Caster.Skills.Chivalry.Value;

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = (Mobile) targets[ i ];
					BaseCreature bc = m as BaseCreature;

					if ( bc != null )
					{
						bool dispellable = bc.Summoned && !bc.IsAnimatedDead;

						if ( dispellable )
						{
							double dispelChance = (50.0 + ((100*(chiv - bc.DispelDifficulty))/(bc.DispelFocus*2)))/100;
							dispelChance *= dispelSkill/100.0;

							if ( dispelChance > Utility.RandomDouble() )
							{
								Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
								Effects.PlaySound( m, m.Map, 0x201 );

								m.Delete();
								continue;
							}
						}

						bool evil = !bc.Controled && bc.Karma < 0;

						if ( evil )
						{
							// TODO: Is this right?
							double fleeChance = (100 - Math.Sqrt( m.Fame/2 ))*chiv*dispelSkill;
							fleeChance /= 1000000;

							if ( fleeChance > Utility.RandomDouble() )
							{
								// guide says 2 seconds, it's longer
								bc.BeginFlee( TimeSpan.FromSeconds( 30.0 ) );
							}
						}
					}

					if ( TransformationSpell.GetContext( m ) != null )
					{
						// transformed ..

						double drainChance = 0.5*(Caster.Skills.Chivalry.Value/Math.Max( m.Skills.Necromancy.Value, 1 ));

						if ( drainChance > Utility.RandomDouble() )
						{
							int drain = (5*dispelSkill)/100;

							m.Stam -= drain;
							m.Mana -= drain;
						}
					}
				}
			}

			FinishSequence();
		}
	}
}