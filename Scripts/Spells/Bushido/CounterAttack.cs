using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Bushido
{
	public class CounterAttack : SamuraiSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Counter Attack", null, SpellCircle.Seventh, -1, 9002 );

		public override double RequiredSkill { get { return 40.0; } }
		public override int RequiredMana { get { return 5; } }

		public override int SpellNumber { get { return 0x94; } }

		public static Hashtable m_Table = new Hashtable();

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		private void Expire_Callback( object state )
		{
			Mobile m = (Mobile) state;

			if ( UnderEffect( m ) )
			{
				m.Send( new SetNewSpell( 0x94, 0 ) );

				m.SendLocalizedMessage( 1063119 ); // You return to your normal stance.

				FinishSequence();

				m_Table.Remove( m );
			}
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			{
				return false;
			}

			if ( Confidence.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0x92, 0 ) );

				Confidence.m_Table.Remove( Caster );
			}

			if ( Evasion.UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( 0x93, 0 ) );

				Evasion.m_Table.Remove( Caster );
			}

			BaseShield shield = Caster.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;

			BaseWeapon weapon1 = Caster.FindItemOnLayer( Layer.OneHanded ) as BaseWeapon;

			BaseWeapon weapon2 = Caster.FindItemOnLayer( Layer.TwoHanded ) as BaseWeapon;

			if ( shield == null && weapon1 == null && weapon2 == null )
			{
				Caster.SendLocalizedMessage( 1062944 ); // You must have a weapon or a shield equipped to use this ability!

				return false;
			}

			return true;
		}

		public CounterAttack( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				Caster.Send( new SetNewSpell( SpellNumber, 1 ) );

				Caster.SendLocalizedMessage( 1063118 ); // You prepare to respond immediately to the next blocked blow.

				Timer t = (Timer) m_Table[ Caster ];

				if ( t != null )
				{
					t.Stop();
				}

				m_Table[ Caster ] = t = Timer.DelayCall( TimeSpan.FromSeconds( 60.0 ), new TimerStateCallback( Expire_Callback ), Caster );
			}

			FinishSequence();
		}
	}
}