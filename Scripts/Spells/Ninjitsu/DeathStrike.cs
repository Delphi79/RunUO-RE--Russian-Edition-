using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Ninjitsu
{
	public class DeathStrike : NinjaSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Death Strike", null, SpellCircle.Seventh, -1, 9002 );

		public override double RequiredSkill { get { return 85.0; } }
		public override int RequiredMana { get { return 30; } }

		public override int SpellNumber { get { return 0xF6; } }

		public static Hashtable m_Table = new Hashtable();

		public static Hashtable m_Table2 = new Hashtable();

		public static Spell GetSpell( Mobile m )
		{
			return (Spell) m_Table2[ m ];
		}

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		public DeathStrike( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

			Caster.Send( new SetNewSpell( SpellNumber, 1 ) );

			Caster.SendLocalizedMessage( 1063091 ); // You prepare to hit your opponent with a Death Strike.

			m_Table[ Caster ] = true;

			m_Table2[ Caster ] = this;
		}
	}
}