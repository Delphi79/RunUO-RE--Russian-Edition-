using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.SkillHandlers
{
	public class Peacemaking
	{
		public static void Initialize()
		{
			SkillInfo.Table[ (int) SkillName.Peacemaking ].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.RevealingAction();

			BaseInstrument.PickInstrument( m, new InstrumentPickedCallback( OnPickedInstrument ) );

			return TimeSpan.FromSeconds( 1.0 ); // Cannot use another skill for 1 second
		}

		public static void OnPickedInstrument( Mobile from, BaseInstrument instrument )
		{
			from.RevealingAction();
			from.SendLocalizedMessage( 1049525 ); // Whom do you wish to calm?
			from.Target = new InternalTarget( from, instrument );
			from.NextSkillTime = DateTime.Now + TimeSpan.FromHours( 6.0 );
		}

		private class InternalTarget : Target
		{
			private BaseInstrument m_Instrument;
			private bool m_SetSkillTime = true;

			public InternalTarget( Mobile from, BaseInstrument instrument ) : base( BaseInstrument.GetBardRange( from, SkillName.Peacemaking ), false, TargetFlags.None )
			{
				m_Instrument = instrument;
			}

			protected override void OnTargetFinish( Mobile from )
			{
				if ( m_SetSkillTime )
				{
					from.NextSkillTime = DateTime.Now;
				}
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				from.RevealingAction();

				if ( !(targeted is Mobile) )
				{
					from.SendLocalizedMessage( 1049528 ); // You cannot calm that!
				}
				else
				{
					m_SetSkillTime = false;

					from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 10.0 );

					if ( targeted == from )
					{
						// Standard mode : reset combatants for everyone in the area

						if ( !BaseInstrument.CheckMusicianship( from ) )
						{
							from.SendLocalizedMessage( 500612 ); // You play poorly, and there is no effect.
							m_Instrument.PlayInstrumentBadly( from );
							m_Instrument.ConsumeUse( from );
						}
						else if ( !from.CheckSkill( SkillName.Peacemaking, 0.0, 120.0 ) )
						{
							from.SendLocalizedMessage( 500613 ); // You attempt to calm everyone, but fail.
							m_Instrument.PlayInstrumentBadly( from );
							m_Instrument.ConsumeUse( from );
						}
						else
						{
							m_Instrument.PlayInstrumentWell( from );
							m_Instrument.ConsumeUse( from );

							from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 5.0 );

							Map map = from.Map;

							if ( map != null )
							{
								int range = BaseInstrument.GetBardRange( from, SkillName.Peacemaking );

								bool calmed = false;

								foreach ( Mobile m in from.GetMobilesInRange( range ) )
								{
									if ( (m is BaseCreature && ((BaseCreature) m).Uncalmable) || m == from || !from.CanBeHarmful( m, false ) )
									{
										continue;
									}

									calmed = true;

									m.SendLocalizedMessage( 500616 ); // You hear lovely music, and forget to continue battling!
									m.Combatant = null;
									m.Warmode = false;

									if ( m is BaseCreature && !((BaseCreature) m).BardPacified )
									{
										((BaseCreature) m).Pacify( from, DateTime.Now + TimeSpan.FromSeconds( 1.0 ) );
									}
								}

								if ( !calmed )
								{
									from.SendLocalizedMessage( 1049648 ); // You play hypnotic music, but there is nothing in range for you to calm.
								}
								else
								{
									from.SendLocalizedMessage( 500615 ); // You play your hypnotic music, stopping the battle.
								}
							}
						}
					}
					else
					{
						// Target mode : pacify a single target for a longer duration

						Mobile targ = (Mobile) targeted;

						if ( !from.CanBeHarmful( targ, false ) )
						{
							from.SendLocalizedMessage( 1049528 );
							m_SetSkillTime = true;
						}
						else if ( targ is BaseCreature && ((BaseCreature) targ).Uncalmable )
						{
							from.SendLocalizedMessage( 1049526 ); // You have no chance of calming that creature.
							m_SetSkillTime = true;
						}
						else if ( targ is BaseCreature && ((BaseCreature) targ).BardPacified )
						{
							from.SendLocalizedMessage( 1049527 ); // That creature is already being calmed.
							m_SetSkillTime = true;
						}
						else if ( !BaseInstrument.CheckMusicianship( from ) )
						{
							from.SendLocalizedMessage( 500612 ); // You play poorly, and there is no effect.
							m_Instrument.PlayInstrumentBadly( from );
							m_Instrument.ConsumeUse( from );
						}
						else
						{
							double diff = m_Instrument.GetDifficultyFor( targ ) - 10.0;
							double music = from.Skills[ SkillName.Musicianship ].Value;

							if ( music > 100.0 )
							{
								diff -= (music - 100.0)*0.5;
							}

							if ( !from.CheckTargetSkill( SkillName.Peacemaking, targ, diff - 25.0, diff + 25.0 ) )
							{
								from.SendLocalizedMessage( 1049531 ); // You attempt to calm your target, but fail.
								m_Instrument.PlayInstrumentBadly( from );
								m_Instrument.ConsumeUse( from );
							}
							else
							{
								m_Instrument.PlayInstrumentWell( from );
								m_Instrument.ConsumeUse( from );

								from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 5.0 );

								if ( targ is BaseCreature )
								{
									BaseCreature bc = (BaseCreature) targ;

									from.SendLocalizedMessage( 1049532 ); // You play hypnotic music, calming your target.

									targ.Combatant = null;
									targ.Warmode = false;

									double seconds = 100 - (diff/1.5);

									if ( seconds > 120 )
									{
										seconds = 120;
									}
									else if ( seconds < 10 )
									{
										seconds = 10;
									}

									bc.Pacify( from, DateTime.Now + TimeSpan.FromSeconds( seconds ) );
								}
								else
								{
									from.SendLocalizedMessage( 1049532 ); // You play hypnotic music, calming your target.

									targ.SendLocalizedMessage( 500616 ); // You hear lovely music, and forget to continue battling!
									targ.Combatant = null;
									targ.Warmode = false;
								}
							}
						}
					}
				}
			}
		}
	}
}