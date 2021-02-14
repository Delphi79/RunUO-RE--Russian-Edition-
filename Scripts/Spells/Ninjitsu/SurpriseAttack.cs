using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Ninjitsu
{
	public class SurpriseAttack : NinjaSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Surprise Attack", null, SpellCircle.Seventh, -1, 9002 );

		public override double RequiredSkill { get { return 30.0; } }
		public override int RequiredMana { get { return 20; } }

		public override int SpellNumber { get { return 0xF9; } }

		public override bool RevealOnCast { get { return false; } }

		public static Hashtable m_Table = new Hashtable();

		public static Hashtable m_Table2 = new Hashtable();

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		public static Spell GetSpell( Mobile m )
		{
			return (Spell) m_Table2[ m ];
		}

		public SurpriseAttack( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			{
				return false;
			}

			if ( Backstab.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0xFA, 0 ) );

				Backstab.m_Table.Remove( Caster );
			}

			return true;
		}

		public override void OnCast()
		{
			if ( UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( SpellNumber, 0 ) );

				m_Table.Remove( Caster );

				Caster.CanReveal = true;

				FinishSequence();

				return;
			}

			if ( Caster.Hidden && Caster.AllowedStealthSteps != 0 )
			{
				Caster.Send( new SetNewSpell( SpellNumber, 1 ) );

				Caster.SendLocalizedMessage( 1063128 ); // You prepare to surprise your prey.

				Caster.CanReveal = false;

				m_Table[ Caster ] = true;

				m_Table2[ Caster ] = this;
			}
			else
			{
				Caster.SendLocalizedMessage( 1063087 ); // You must be in stealth mode to use this ability.

				FinishSequence();
			}
		}
	}
}