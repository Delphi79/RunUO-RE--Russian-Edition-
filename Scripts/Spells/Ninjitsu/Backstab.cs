using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Ninjitsu
{
	public class Backstab : NinjaSpell
	{
		public static SpellInfo m_Info = new SpellInfo( "Backstab", null, SpellCircle.Seventh, -1, 9002 );

		public override double RequiredSkill { get { return 20.0; } }
		public override int RequiredMana { get { return 30; } }

		public override int SpellNumber { get { return 0xFA; } }

		public override bool RevealOnCast { get { return false; } }

		public static Hashtable m_Table = new Hashtable();

		public static Hashtable m_Table2 = new Hashtable();

		public static Hashtable m_Table3 = new Hashtable();

		public static Point3D CasterLocation( Mobile m )
		{
			return (Point3D) m_Table2[ m ];
		}

		public static Spell GetSpell( Mobile m )
		{
			return (Spell) m_Table3[ m ];
		}

		public static bool UnderEffect2( Mobile m )
		{
			return m_Table2.Contains( m );
		}

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		public Backstab( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			{
				return false;
			}

			if ( SurpriseAttack.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0xF9, 0 ) );

				SurpriseAttack.m_Table.Remove( Caster );
			}

			return true;
		}

		public override void OnCast()
		{
			if ( UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( SpellNumber, 0 ) );

				Caster.CanReveal = true;

				m_Table.Remove( Caster );

				FinishSequence();

				return;
			}

			if ( Caster.Hidden && Caster.AllowedStealthSteps != 0 )
			{
				Caster.Send( new SetNewSpell( SpellNumber, 1 ) );

				Caster.SendLocalizedMessage( 1063089 ); // You prepare to Backstab your opponent.

				Caster.CanReveal = false;

				m_Table[ Caster ] = true;

				Point3D location = Caster.Location;

				m_Table2[ Caster ] = location;

				m_Table3[ Caster ] = this;
			}
			else
			{
				Caster.SendLocalizedMessage( 1063087 ); // You must be in stealth mode to use this ability.

				FinishSequence();
			}
		}
	}
}