using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Spells.Bushido;
using Server.Spells.Ninjitsu;

namespace Server.Items
{
	public abstract class WeaponAbility
	{
		public virtual int BaseMana { get { return 0; } }

		public virtual double AccuracyScalar { get { return 1.0; } }
		public virtual double DamageScalar { get { return 1.0; } }

		public virtual void OnHit( Mobile attacker, Mobile defender, int damage )
		{
		}

		public virtual void OnMiss( Mobile attacker, Mobile defender )
		{
		}

		public virtual double GetRequiredSkill( Mobile from )
		{
			BaseWeapon weapon = from.Weapon as BaseWeapon;

			if ( weapon != null && weapon.PrimaryAbility == this )
			{
				return 70.0;
			}
			else if ( weapon != null && weapon.SecondaryAbility == this )
			{
				return 90.0;
			}

			return 200.0;
		}

		public virtual int CalculateMana( Mobile from )
		{
			int mana = BaseMana;

			double skillTotal = GetSkill( from, SkillName.Swords ) + GetSkill( from, SkillName.Macing ) + GetSkill( from, SkillName.Fencing ) + GetSkill( from, SkillName.Archery ) + GetSkill( from, SkillName.Parry ) + GetSkill( from, SkillName.Lumberjacking ) + GetSkill( from, SkillName.Stealth ) + GetSkill( from, SkillName.Poisoning ) + GetSkill( from, SkillName.Bushido ) + GetSkill( from, SkillName.Ninjitsu );

			if ( skillTotal >= 300.0 )
			{
				mana -= 10;
			}
			else if ( skillTotal >= 200.0 )
			{
				mana -= 5;
			}

			double scalar = 1.0;
			if ( !Server.Spells.Necromancy.MindRotSpell.GetMindRotScalar( from, ref scalar ) )
			{
				scalar = 1.0;
			}

			// Lower Mana Cost = 40%
			int lmc = AosAttributes.GetValue( from, AosAttribute.LowerManaCost );
			if ( lmc > 40 )
			{
				lmc = 40;
			}

			scalar -= (double) lmc/100;
			mana = (int) (mana*scalar);

			// Using a special move within 3 seconds of the previous special move costs double mana 
			if ( GetContext( from ) != null )
			{
				mana *= 2;
			}

			return mana;
		}

		public virtual bool CheckWeaponSkill( Mobile from )
		{
			BaseWeapon weapon = from.Weapon as BaseWeapon;

			if ( weapon == null )
			{
				return false;
			}

			Skill skill = from.Skills[ weapon.Skill ];
			double reqSkill = GetRequiredSkill( from );

			if ( skill != null && skill.Base >= reqSkill )
			{
				return true;
			}

			/* <UBWS> */
			if ( weapon.WeaponAttributes.UseBestSkill > 0 && (from.Skills[ SkillName.Swords ].Base >= reqSkill || from.Skills[ SkillName.Macing ].Base >= reqSkill || from.Skills[ SkillName.Fencing ].Base >= reqSkill) )
			{
				return true;
			}
			/* </UBWS> */

			from.SendLocalizedMessage( 1060182, reqSkill.ToString() ); // You need ~1_SKILL_REQUIREMENT~ weapon skill to perform that attack

			return false;
		}

		public virtual bool CheckSkills( Mobile from )
		{
			return CheckWeaponSkill( from );
		}

		public virtual double GetSkill( Mobile from, SkillName skillName )
		{
			Skill skill = from.Skills[ skillName ];

			if ( skill == null )
			{
				return 0.0;
			}

			return skill.Value;
		}

		public virtual bool CheckMana( Mobile from, bool consume )
		{
			int mana = CalculateMana( from );

			if ( from.Mana < mana )
			{
				from.SendLocalizedMessage( 1060181, mana.ToString() ); // You need ~1_MANA_REQUIREMENT~ mana to perform that attack
				return false;
			}

			if ( consume )
			{
				if ( GetContext( from ) == null )
				{
					Timer timer = new WeaponAbilityTimer( from );
					timer.Start();

					AddContext( from, new WeaponAbilityContext( timer ) );
				}

				from.Mana -= mana;
			}

			return true;
		}

		public virtual bool Validate( Mobile from )
		{
			if ( !from.Player )
			{
				return true;
			}

			if ( AnimalForm.UnderEffect( from ) )
			{
				from.SendLocalizedMessage( 1063218  ); // You cannot use that ability in this form.
				return false;
			}

			return CheckSkills( from ) && CheckMana( from, false );
		}

		private static WeaponAbility[] m_Abilities = new WeaponAbility[24] {null, new ArmorIgnore(), new BleedAttack(), new ConcussionBlow(), new CrushingBlow(), new Disarm(), new Dismount(), new DoubleStrike(), new InfectiousStrike(), new MortalStrike(), new MovingShot(), new ParalyzingBlow(), new ShadowStrike(), new WhirlwindAttack(), new RidingSwipe(), new FrenziedWhirlwind(), new Block(), new DefenseMastery(), new NerveStrike(), new TalonStrike(), new Feint(), new DualWield(), new DoubleShot(), new ArmorPierce()};

		public static WeaponAbility[] Abilities { get { return m_Abilities; } }

		private static Hashtable m_Table = new Hashtable();

		public static Hashtable Table { get { return m_Table; } }

		public static readonly WeaponAbility ArmorIgnore = m_Abilities[ 1 ];
		public static readonly WeaponAbility BleedAttack = m_Abilities[ 2 ];
		public static readonly WeaponAbility ConcussionBlow = m_Abilities[ 3 ];
		public static readonly WeaponAbility CrushingBlow = m_Abilities[ 4 ];
		public static readonly WeaponAbility Disarm = m_Abilities[ 5 ];
		public static readonly WeaponAbility Dismount = m_Abilities[ 6 ];
		public static readonly WeaponAbility DoubleStrike = m_Abilities[ 7 ];
		public static readonly WeaponAbility InfectiousStrike = m_Abilities[ 8 ];
		public static readonly WeaponAbility MortalStrike = m_Abilities[ 9 ];
		public static readonly WeaponAbility MovingShot = m_Abilities[ 10 ];
		public static readonly WeaponAbility ParalyzingBlow = m_Abilities[ 11 ];
		public static readonly WeaponAbility ShadowStrike = m_Abilities[ 12 ];
		public static readonly WeaponAbility WhirlwindAttack = m_Abilities[ 13 ];
		public static readonly WeaponAbility RidingSwipe = m_Abilities[ 14 ];
		public static readonly WeaponAbility FrenziedWhirlwind = m_Abilities[ 15 ];
		public static readonly WeaponAbility Block = m_Abilities[ 16 ];
		public static readonly WeaponAbility DefenseMastery = m_Abilities[ 17 ];
		public static readonly WeaponAbility NerveStrike = m_Abilities[ 18 ];
		public static readonly WeaponAbility TalonStrike = m_Abilities[ 19 ];
		public static readonly WeaponAbility Feint = m_Abilities[ 20 ];
		public static readonly WeaponAbility DualWield = m_Abilities[ 21 ];
		public static readonly WeaponAbility DoubleShot = m_Abilities[ 22 ];
		public static readonly WeaponAbility ArmorPierce = m_Abilities[ 23 ];

		public static bool IsWeaponAbility( Mobile m, WeaponAbility a )
		{
			if ( a == null )
			{
				return true;
			}

			if ( !m.Player )
			{
				return true;
			}

			BaseWeapon weapon = m.Weapon as BaseWeapon;

			return (weapon != null && (weapon.PrimaryAbility == a || weapon.SecondaryAbility == a));
		}

		public static WeaponAbility GetCurrentAbility( Mobile m )
		{
			if ( !Core.AOS )
			{
				ClearCurrentAbility( m );
				return null;
			}

			WeaponAbility a = (WeaponAbility) m_Table[ m ];

			if ( !IsWeaponAbility( m, a ) )
			{
				ClearCurrentAbility( m );
				return null;
			}

			if ( a != null && !a.Validate( m ) )
			{
				ClearCurrentAbility( m );
				return null;
			}

			return a;
		}

		public static bool SetCurrentAbility( Mobile m, WeaponAbility a )
		{
			if ( !Core.AOS )
			{
				ClearCurrentAbility( m );
				return false;
			}

			if ( !IsWeaponAbility( m, a ) )
			{
				ClearCurrentAbility( m );
				return false;
			}

			if ( a != null && !a.Validate( m ) )
			{
				ClearCurrentAbility( m );
				return false;
			}

			if ( a == null )
			{
				m_Table.Remove( m );
			}
			else
			{
				m_Table[ m ] = a;
			}

			m.CanReveal = true;

			if ( HonorableExecution.UnderEffect( m ) )
			{
				m.Send( new SetNewSpell( 0x91, 0 ) );

				HonorableExecution.m_Table.Remove( m );
			}

			if ( LightningStrike.UnderEffect( m ) )
			{
				m.Send( new SetNewSpell( 0x95, 0 ) );

				LightningStrike.m_Table.Remove( m );
			}

			if ( MomentumStrike.UnderEffect( m ) )
			{
				m.Send( new SetNewSpell( 0x96, 0 ) );

				MomentumStrike.m_Table.Remove( m );
			}

			if ( FocusAttack.UnderEffect( m ) )
			{
				m.Send( new SetNewSpell( 0xF5, 0 ) );

				FocusAttack.m_Table.Remove( m );

				BaseWeapon weapon = m.Weapon as BaseWeapon;

				if ( weapon != null )
				{
					FocusAttack.RemoveBonus( weapon );
				}
			}

			if ( DeathStrike.UnderEffect( m ) )
			{
				m.Send( new SetNewSpell( 0xF6, 0 ) );

				DeathStrike.m_Table.Remove( m );
			}

			if ( KiAttack.UnderEffect( m ) )
			{
				m.Send( new SetNewSpell( 0xF8, 0 ) );

				KiAttack.m_Table.Remove( m );
			}

			if ( SurpriseAttack.UnderEffect( m ) )
			{
				m.Send( new SetNewSpell( 0xF9, 0 ) );

				SurpriseAttack.m_Table.Remove( m );
			}

			if ( Backstab.UnderEffect( m ) )
			{
				m.Send( new SetNewSpell( 0xFA, 0 ) );

				Backstab.m_Table.Remove( m );
			}

			return true;
		}

		public static void ClearCurrentAbility( Mobile m )
		{
			m_Table.Remove( m );

			if ( m.NetState != null )
			{
				m.Send( ClearWeaponAbility.Instance );
			}
		}

		public static void Initialize()
		{
			EventSink.SetAbility += new SetAbilityEventHandler( EventSink_SetAbility );
		}

		public WeaponAbility()
		{
		}

		private static void EventSink_SetAbility( SetAbilityEventArgs e )
		{
			int index = e.Index;

			if ( index == 0 )
			{
				ClearCurrentAbility( e.Mobile );
			}
			else if ( index >= 1 && index < m_Abilities.Length )
			{
				SetCurrentAbility( e.Mobile, m_Abilities[ index ] );
			}
		}


		private static Hashtable m_PlayersTable = new Hashtable();

		private static void AddContext( Mobile m, WeaponAbilityContext context )
		{
			m_PlayersTable[ m ] = context;
		}

		private static void RemoveContext( Mobile m )
		{
			WeaponAbilityContext context = GetContext( m );

			if ( context != null )
			{
				RemoveContext( m, context );
			}
		}

		private static void RemoveContext( Mobile m, WeaponAbilityContext context )
		{
			m_PlayersTable.Remove( m );

			context.Timer.Stop();
		}

		private static WeaponAbilityContext GetContext( Mobile m )
		{
			return (m_PlayersTable[ m ] as WeaponAbilityContext);
		}

		private class WeaponAbilityTimer : Timer
		{
			private Mobile m_Mobile;

			public WeaponAbilityTimer( Mobile from ) : base( TimeSpan.FromSeconds( 3.0 ) )
			{
				m_Mobile = from;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				RemoveContext( m_Mobile );
			}
		}

		private class WeaponAbilityContext
		{
			private Timer m_Timer;

			public Timer Timer { get { return m_Timer; } }

			public WeaponAbilityContext( Timer timer )
			{
				m_Timer = timer;
			}
		}
	}
}