using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Ninjitsu
{
	public class KiAttack : NinjaSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Ki Attack", null, SpellCircle.Seventh, -1, 9002 );

		public override double RequiredSkill { get { return 80.0; } }
		public override int RequiredMana { get { return 25; } }

		public override int SpellNumber { get { return 0xF8; } }

		public static Hashtable m_Table = new Hashtable();

		public static Hashtable m_Table2 = new Hashtable();

		public static Hashtable m_Table3 = new Hashtable();

		public static Spell GetSpell( Mobile m )
		{
			return (Spell) m_Table3[ m ];
		}

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		public static Point3D CasterLocation( Mobile m )
		{
			return (Point3D) m_Table2[ m ];
		}

		public static bool UnderEffect2( Mobile m )
		{
			return m_Table2.Contains( m );
		}

		private void Expire_Callback( object state )
		{
			Mobile m = (Mobile) state;

			if ( UnderEffect( m ) )
			{
				m.Send( new SetNewSpell( 0xF8, 0 ) );

				if ( UnderEffect2( m ) )
				{
					m.SendLocalizedMessage( 1063102 ); // You failed to complete your Ki Attack in time.
				}

				FinishSequence();

				m_Table.Remove( m );
			}
		}

		public KiAttack( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if ( UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( SpellNumber, 0 ) );

				m_Table.Remove( Caster );

				FinishSequence();

				return;
			}

			if ( Caster.Hidden && Caster.AllowedStealthSteps != 0 )
			{
				Caster.SendLocalizedMessage( 1063127 ); // You cannot use this ability while in stealth mode.
			}
			else
			{
				Caster.Send( new SetNewSpell( SpellNumber, 1 ) );

				Caster.SendLocalizedMessage( 1063099 ); // Your Ki Attack must be complete within 2 seconds for the damage bonus!

				Timer t = (Timer) m_Table[ Caster ];

				Point3D location = Caster.Location;

				m_Table2[ Caster ] = location;

				m_Table3[ Caster ] = this;

				if ( t != null )
				{
					t.Stop();
				}

				m_Table[ Caster ] = t = Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( Expire_Callback ), Caster );
			}
		}
	}
}