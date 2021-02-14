using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Ninjitsu
{
	public class FocusAttack : NinjaSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Focus Attack", null, SpellCircle.Seventh, -1, 9002 );

		public override double RequiredSkill { get { return 60.0; } }
		public override int RequiredMana { get { return 20; } }

		public override int SpellNumber { get { return 0xF5; } }

		public static Hashtable m_Table = new Hashtable();

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		public static Hashtable m_Table2 = new Hashtable();

		public static Spell GetSpell( Mobile m )
		{
			return (Spell) m_Table2[ m ];
		}

		public double GetFocusBonus( double skill )
		{
			double result = 0;

			if ( skill < 60.0 || skill > 120.0 )
			{
				result = 0;
			}

			if ( skill >= 60.0 && skill <= 62.6 )
			{
				result = 1.25;
			}

			if ( skill >= 62.7 && skill <= 66.0 )
			{
				result = 1.275;
			}

			if ( skill >= 66.1 && skill <= 69.2 )
			{
				result = 1.3;
			}

			if ( skill >= 69.3 && skill <= 72.3 )
			{
				result = 1.325;
			}

			if ( skill >= 72.4 && skill <= 75.3 )
			{
				result = 1.35;
			}

			if ( skill >= 75.4 && skill <= 78.1 )
			{
				result = 1.4;
			}

			if ( skill >= 78.2 && skill <= 80.9 )
			{
				result = 1.425;
			}

			if ( skill >= 81.0 && skill <= 83.5 )
			{
				result = 1.45;
			}

			if ( skill >= 83.6 && skill <= 86.1 )
			{
				result = 1.475;
			}

			if ( skill >= 86.2 && skill <= 88.6 )
			{
				result = 1.5;
			}

			if ( skill >= 88.7 && skill <= 91.0 )
			{
				result = 1.55;
			}

			if ( skill >= 91.1 && skill <= 93.4 )
			{
				result = 1.575;
			}

			if ( skill >= 93.5 && skill <= 95.7 )
			{
				result = 1.6;
			}

			if ( skill >= 95.8 && skill <= 97.9 )
			{
				result = 1.625;
			}

			if ( skill >= 98.0 && skill <= 100.1 )
			{
				result = 1.65;
			}

			if ( skill >= 100.2 && skill <= 102.3 )
			{
				result = 1.7;
			}

			if ( skill >= 102.4 && skill <= 104.4 )
			{
				result = 1.725;
			}

			if ( skill >= 104.5 && skill <= 106.5 )
			{
				result = 1.75;
			}

			if ( skill >= 106.6 && skill <= 108.5 )
			{
				result = 1.775;
			}

			if ( skill >= 108.6 && skill <= 110.5 )
			{
				result = 1.8;
			}

			if ( skill >= 110.6 && skill <= 112.4 )
			{
				result = 1.85;
			}

			if ( skill >= 112.5 && skill <= 114.4 )
			{
				result = 1.875;
			}

			if ( skill >= 114.5 && skill <= 116.3 )
			{
				result = 1.9;
			}

			if ( skill >= 116.4 && skill <= 118.1 )
			{
				result = 1.925;
			}

			if ( skill >= 118.2 && skill <= 119.9 )
			{
				result = 1.95;
			}

			if ( skill == 120.0 )
			{
				result = 2.00;
			}

			return result;
		}

		public static void AddBonus( BaseWeapon weapon )
		{
			weapon.WeaponAttributes.HitLeechHits = (int) (weapon.WeaponAttributes.HitLeechHits*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitLeechStam = (int) (weapon.WeaponAttributes.HitLeechStam*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitLeechMana = (int) (weapon.WeaponAttributes.HitLeechMana*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitPhysicalArea = (int) (weapon.WeaponAttributes.HitPhysicalArea*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitFireArea = (int) (weapon.WeaponAttributes.HitFireArea*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitColdArea = (int) (weapon.WeaponAttributes.HitColdArea*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitPoisonArea = (int) (weapon.WeaponAttributes.HitPoisonArea*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitEnergyArea = (int) (weapon.WeaponAttributes.HitEnergyArea*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitMagicArrow = (int) (weapon.WeaponAttributes.HitMagicArrow*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitHarm = (int) (weapon.WeaponAttributes.HitHarm*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitFireball = (int) (weapon.WeaponAttributes.HitFireball*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitLightning = (int) (weapon.WeaponAttributes.HitLightning*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitDispel = (int) (weapon.WeaponAttributes.HitDispel*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitLowerAttack = (int) (weapon.WeaponAttributes.HitLowerAttack*weapon.FocusCoeff);

			weapon.WeaponAttributes.HitLowerDefend = (int) (weapon.WeaponAttributes.HitLowerDefend*weapon.FocusCoeff);

			weapon.InvalidateProperties();
		}

		public static void RemoveBonus( BaseWeapon weapon )
		{
			if ( weapon.FocusCoeff != 0 )
			{
				weapon.WeaponAttributes.HitLeechHits = (int) (Math.Ceiling( weapon.WeaponAttributes.HitLeechHits/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitLeechStam = (int) (Math.Ceiling( weapon.WeaponAttributes.HitLeechStam/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitLeechMana = (int) (Math.Ceiling( weapon.WeaponAttributes.HitLeechMana/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitPhysicalArea = (int) (Math.Ceiling( weapon.WeaponAttributes.HitPhysicalArea/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitFireArea = (int) (Math.Ceiling( weapon.WeaponAttributes.HitFireArea/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitColdArea = (int) (Math.Ceiling( weapon.WeaponAttributes.HitColdArea/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitPoisonArea = (int) (Math.Ceiling( weapon.WeaponAttributes.HitPoisonArea/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitEnergyArea = (int) (Math.Ceiling( weapon.WeaponAttributes.HitEnergyArea/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitMagicArrow = (int) (Math.Ceiling( weapon.WeaponAttributes.HitMagicArrow/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitHarm = (int) (Math.Ceiling( weapon.WeaponAttributes.HitHarm/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitFireball = (int) (Math.Ceiling( weapon.WeaponAttributes.HitFireball/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitLightning = (int) (Math.Ceiling( weapon.WeaponAttributes.HitLightning/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitDispel = (int) (Math.Ceiling( weapon.WeaponAttributes.HitDispel/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitLowerAttack = (int) (Math.Ceiling( weapon.WeaponAttributes.HitLowerAttack/weapon.FocusCoeff ));

				weapon.WeaponAttributes.HitLowerDefend = (int) (Math.Ceiling( weapon.WeaponAttributes.HitLowerDefend/weapon.FocusCoeff ));
			}

			weapon.FocusCoeff = 0;
		}

		public FocusAttack( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			BaseShield shield = Caster.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;

			BaseWeapon weapon1 = Caster.FindItemOnLayer( Layer.OneHanded ) as BaseWeapon;

			BaseWeapon weapon2 = Caster.FindItemOnLayer( Layer.TwoHanded ) as BaseWeapon;

			if ( UnderEffect( Caster ) )
			{
				Caster.Send( new SetNewSpell( SpellNumber, 0 ) );

				m_Table.Remove( Caster );

				if ( weapon1 != null )
				{
					RemoveBonus( weapon1 );
				}

				if ( weapon2 != null )
				{
					RemoveBonus( weapon2 );
				}

				FinishSequence();

				return;
			}

			if ( shield != null )
			{
				Caster.SendLocalizedMessage( 1063096 ); // You cannot use this ability while holding a shield.
				return;
			}

			if ( weapon1 == null && weapon2 == null )
			{
				Caster.SendLocalizedMessage( 1063097 ); // You must be wielding a melee weapon without a shield to use this ability.
				return;
			}

			Caster.Send( new SetNewSpell( SpellNumber, 1 ) );

			Caster.SendLocalizedMessage( 1063095 ); // You prepare to focus all of your abilities into your next strike.

			m_Table[ Caster ] = true;

			m_Table2[ Caster ] = this;

			if ( weapon1 != null )
			{
				weapon1.FocusCoeff = GetFocusBonus( Caster.Skills[ SkillName.Ninjitsu ].Value );

				AddBonus( weapon1 );
			}

			if ( weapon2 != null )
			{
				weapon2.FocusCoeff = GetFocusBonus( Caster.Skills[ SkillName.Ninjitsu ].Value );

				AddBonus( weapon2 );
			}
		}
	}
}