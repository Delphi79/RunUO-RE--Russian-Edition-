using System;
using Server.Targeting;
using Server.Items;
using Server.Network;

namespace Server.SkillHandlers
{
	public class Poisoning
	{
		public static void Initialize()
		{
			SkillInfo.Table[ (int) SkillName.Poisoning ].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.Target = new InternalTargetPoison();

			m.SendLocalizedMessage( 502137 ); // Select the poison you wish to use

			return TimeSpan.FromSeconds( 10.0 ); // 10 second delay before beign able to re-use a skill
		}

		private class InternalTargetPoison : Target
		{
			public InternalTargetPoison() : base( 2, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is BasePoisonPotion )
				{
					from.SendLocalizedMessage( 502142 ); // To what do you wish to apply the poison?
					from.Target = new InternalTarget( (BasePoisonPotion) targeted );
				}
				else // Not a Poison Potion
				{
					from.SendLocalizedMessage( 502139 ); // That is not a poison potion.
				}
			}

			private class InternalTarget : Target
			{
				private BasePoisonPotion m_Potion;

				public InternalTarget( BasePoisonPotion potion ) : base( 2, false, TargetFlags.None )
				{
					m_Potion = potion;
				}

				protected override void OnTarget( Mobile from, object targeted )
				{
					if ( m_Potion.Deleted )
					{
						return;
					}

					bool startTimer = false;

					if ( targeted is Food )
					{
						startTimer = true;
					}
					else if ( targeted is BaseWeapon )
					{
						BaseWeapon weapon = (BaseWeapon) targeted;

						if ( Core.AOS )
						{
							startTimer = (weapon.PrimaryAbility == WeaponAbility.InfectiousStrike || weapon.SecondaryAbility == WeaponAbility.InfectiousStrike);
						}
						else if ( weapon.Layer == Layer.OneHanded )
						{
							// Only Bladed or Piercing weapon can be poisoned
							startTimer = (weapon.Type == WeaponType.Slashing || weapon.Type == WeaponType.Piercing);
						}
					}
					else if ( targeted is Shuriken || targeted is FukiyaDart )
					{
						startTimer = true;
					}

					if ( startTimer )
					{
						new InternalTimer( from, (Item) targeted, m_Potion ).Start();

						from.PlaySound( 0x4F );
						m_Potion.Delete();

						from.AddToBackpack( new Bottle() );
					}
					else // Target can't be poisoned
					{
						if ( Core.AOS )
						{
							from.SendLocalizedMessage( 1060204 ); // You cannot poison that! You can only poison infectious weapons, food or drink.
						}
						else
						{
							from.SendLocalizedMessage( 502145 ); // You cannot poison that! You can only poison bladed or piercing weapons, food or drink.
						}
					}
				}

				private class InternalTimer : Timer
				{
					private Mobile m_From;
					private Item m_Target;
					private Poison m_Poison;
					private double m_MinSkill, m_MaxSkill;

					public InternalTimer( Mobile from, Item target, BasePoisonPotion potion ) : base( TimeSpan.FromSeconds( 2.0 ) )
					{
						m_From = from;
						m_Target = target;
						m_Poison = potion.Poison;
						m_MinSkill = potion.MinPoisoningSkill;
						m_MaxSkill = potion.MaxPoisoningSkill;
						Priority = TimerPriority.TwoFiftyMS;
					}

					protected override void OnTick()
					{
						if ( m_From.CheckTargetSkill( SkillName.Poisoning, m_Target, m_MinSkill, m_MaxSkill ) )
						{
							if ( m_Target is Food )
							{
								((Food) m_Target).Poison = m_Poison;
							}
							else if ( m_Target is BaseWeapon )
							{
								((BaseWeapon) m_Target).Poison = m_Poison;
								((BaseWeapon) m_Target).PoisonCharges = 18 - (m_Poison.Level*2);
							}
							else if ( m_Target is Shuriken )
							{
								((Shuriken) m_Target).Poison = m_Poison;
								((Shuriken) m_Target).PoisonCharges = 18 - (m_Poison.Level*2);
							}
							else if ( m_Target is FukiyaDart )
							{
								((FukiyaDart) m_Target).Poison = m_Poison;
								((FukiyaDart) m_Target).PoisonCharges = 18 - (m_Poison.Level*2);
							}

							m_From.SendLocalizedMessage( 1010517 ); // You apply the poison

							Misc.Titles.AwardKarma( m_From, -20, true );
						}
						else // Failed
						{
							// 5% of chance of getting poisoned if failed
							if ( m_From.Skills[ SkillName.Poisoning ].Base < 80.0 && Utility.Random( 20 ) == 0 )
							{
								m_From.SendLocalizedMessage( 502148 ); // You make a grave mistake while applying the poison.
								m_From.ApplyPoison( m_From, m_Poison );
							}
							else
							{
								if ( m_Target is Food )
								{
									m_From.SendLocalizedMessage( 1010518 ); // You fail to apply a sufficient dose of poison
								}
								else if ( m_Target is BaseWeapon )
								{
									BaseWeapon weapon = (BaseWeapon) m_Target;

									if ( weapon.Type == WeaponType.Slashing )
									{
										m_From.SendLocalizedMessage( 1010516 ); // You fail to apply a sufficient dose of poison on the blade
									}
									else
									{
										m_From.SendLocalizedMessage( 1010518 ); // You fail to apply a sufficient dose of poison
									}
								}
								else if ( m_Target is Shuriken )
								{
									m_From.SendLocalizedMessage( 1010518 ); // You fail to apply a sufficient dose of poison									
								}
								else if ( m_Target is FukiyaDart )
								{
									m_From.SendLocalizedMessage( 1010518 ); // You fail to apply a sufficient dose of poison									
								}
							}
						}
					}
				}
			}
		}
	}
}