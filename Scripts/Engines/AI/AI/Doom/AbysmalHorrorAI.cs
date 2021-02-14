using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Targets;
using Server.Network;
using Server.ContextMenus;
using MoveImpl = Server.Movement.MovementImpl;

namespace Server.Mobiles
{
	public class AbysmalHorrorAI : BaseAI
	{
		private int blockcounter = 0;
		private FightMode curFightMode;
		private const double ChanceToDispel = 0.05;
		private const double ChanceToDetectHidden = 0.10;
		private const double CombatantChangeChance = 0.30;
		private const double ChanceToHide = 0.25;
		private const double TeleportChance = 0.10;

		public AbysmalHorrorAI( BaseCreature m ) : base( m )
		{
			curFightMode = m.FightMode;
		}

		public void SelectFightMode()
		{
			if ( Utility.RandomDouble() <= CombatantChangeChance )
			{
				curFightMode = (curFightMode == FightMode.Closest) ? FightMode.Weakest : FightMode.Closest;
			}
		}

		public override bool DoActionWander()
		{
			blockcounter = 0;

			m_Mobile.DebugSay( "no opponent" );

			SelectFightMode();

			if ( AquireFocusMob( m_Mobile.RangePerception, curFightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( " want to kill {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;

				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionWander();
			}

			return true;
		}

		private void TryTeleport()
		{
			try
			{
				Mobile combatant = m_Mobile.Combatant;

				if ( combatant == null )
				{
					return;
				}

				m_Mobile.DebugSay( "want to kill {0}", combatant.Name );

				if ( ++blockcounter > 10 )
				{
					blockcounter = 0;

					if ( Utility.RandomDouble() <= TeleportChance )
					{
						m_Mobile.DebugSay( "teleporting to {0}", combatant.Name );

						Point3D to = new Point3D( combatant.Location );

						m_Mobile.Location = to;

						m_Mobile.ProcessDelta();
					}
				}
			} 
			catch
			{
			}
		}

		public override bool DoActionCombat()
		{
			Mobile c = m_Mobile.Combatant;

			if ( c == null || c.Deleted || c.Map != m_Mobile.Map )
			{
				Action = ActionType.Wander;

				return true;
			}
			else
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo( c );
			}

			if ( ((BaseCreature) m_Mobile).Hidden )
			{
				if ( ((double) m_Mobile.Hits/(double) m_Mobile.HitsMax) >= 0.9 )
				{
					Action = ActionType.Guard;

					return true;
				}

				return true;
			}

			if ( m_Mobile.Hits < m_Mobile.HitsMax*20/100 )
			{
				m_Mobile.DebugSay( "bad.." );

				bool flee = false;

				if ( (((double) m_Mobile.Hits)/((double) m_Mobile.HitsMax)) <= 0.2 )
				{
					int diff = c.Hits - m_Mobile.Hits;

					flee = (Utility.Random( 0, 100 ) > (10 + diff));
				}
				else
				{
					flee = Utility.Random( 0, 100 ) > 10;
				}

				if ( flee )
				{
					m_Mobile.DebugSay( "must escape from {0}", c.Name );

					Action = ActionType.Flee;

					return true;
				}

				if ( ChanceToHide >= Utility.RandomDouble() )
				{
					m_Mobile.DebugSay( "hide mode..." );

					((BaseCreature) m_Mobile).Hidden = true;

					return true;
				}
			}

			if ( CanDispel( c ) && m_Mobile.GetDistanceToSqrt( c ) <= 1 && (ChanceToDispel >= Utility.RandomDouble()) )
			{
				m_Mobile.DebugSay( "dispel for {0}", c.Name );

				DispelTarget( c );

				Action = ActionType.Guard;

				return true;
			}
			else if ( ChanceToDetectHidden >= Utility.RandomDouble() )
			{
				m_Mobile.DebugSay( "check hide...", c.Name );

				DetectHiden();
			}

			if ( MoveTo( c, true, m_Mobile.RangeFight ) )
			{
				if ( blockcounter > 0 )
				{
					blockcounter--;
				}

				m_Mobile.Direction = m_Mobile.GetDirectionTo( c );
			}
			else if ( AquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, true, false, true ) )
			{
				TryTeleport();

				m_Mobile.DebugSay( "want to kill {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;

				Action = ActionType.Combat;

				return true;
			}
			else if ( m_Mobile.GetDistanceToSqrt( c ) > m_Mobile.RangePerception + 1 )
			{
				m_Mobile.DebugSay( "can't find {0}", c.Name );

				Action = ActionType.Guard;

				return true;
			}
			else
			{
				m_Mobile.DebugSay( "far away from {0}", c.Name );
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( AquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "i kill you {0}...", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;

				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionGuard();
			}
			return true;
		}

		public override bool DoActionFlee()
		{
			Mobile c = m_Mobile.Combatant;

			if ( m_Mobile.Hits > (m_Mobile.HitsMax/2) )
			{
				m_Mobile.DebugSay( "healing" );

				Action = ActionType.Guard;
			}
			else if ( AquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "kill to {0}", m_Mobile.FocusMob.Name );

				RunFrom( m_Mobile.FocusMob );

				m_Mobile.FocusMob = null;
			}
			else
			{
				m_Mobile.DebugSay( "strange silence" );

				Action = ActionType.Guard;

				m_Mobile.Warmode = true;
			}

			return true;
		}

		public override bool AquireFocusMob( int iRange, FightMode acqType, bool bPlayerOnly, bool bFacFriend, bool bFacFoe )
		{
			if ( m_Mobile.Deleted )
			{
				return false;
			}

			if ( m_Mobile.BardProvoked )
			{
				if ( m_Mobile.BardTarget == null || m_Mobile.BardTarget.Deleted )
				{
					m_Mobile.FocusMob = null;

					return false;
				}
				else
				{
					m_Mobile.FocusMob = m_Mobile.BardTarget;

					return (m_Mobile.FocusMob != null);
				}
			}
			else if ( m_Mobile.Controled )
			{
				if ( m_Mobile.ControlTarget == null || m_Mobile.ControlTarget.Deleted )
				{
					m_Mobile.FocusMob = null;

					return false;
				}
				else
				{
					m_Mobile.FocusMob = m_Mobile.ControlTarget;

					return (m_Mobile.FocusMob != null);
				}
			}

			if ( acqType == FightMode.None )
			{
				m_Mobile.FocusMob = null;

				return false;
			}

			if ( acqType == FightMode.Agressor && m_Mobile.Aggressors.Count == 0 && m_Mobile.Aggressed.Count == 0 )
			{
				m_Mobile.FocusMob = null;

				return false;
			}

			Map map = m_Mobile.Map;

			if ( map != null )
			{
				Mobile newFocusMob = null;

				double val = double.MinValue;

				IPooledEnumerable eable = map.GetMobilesInRange( m_Mobile.Location, iRange );

				foreach ( Mobile m in eable )
				{
					bool bCheckIt = false;

					//
					// Basic check
					//
					if ( (bPlayerOnly && m.Player) || !bPlayerOnly )
					{
						if ( m.AccessLevel == AccessLevel.Player && m.Alive && !m.Blessed && !m.Deleted && m != m_Mobile && m_Mobile.CanSee( m ) )
						{
							bCheckIt = true;
						}
					}

					if ( bCheckIt && !m_Mobile.Controled && m_Mobile.Summoned && m_Mobile.SummonMaster != null )
					{
						bCheckIt = (m != m_Mobile.SummonMaster);
					}

					//
					// Team check
					//
					if ( bCheckIt ) // alrealy passed the others tests
					{
						bCheckIt = ((bFacFriend && m_Mobile.IsFriend( m )) || (bFacFoe && m_Mobile.IsEnemy( m )));
					}

					if ( bCheckIt && bFacFoe && !bFacFriend && m_Mobile.Summoned && !m_Mobile.Controled && m_Mobile.SummonMaster != null )
					{
						bCheckIt = Server.Spells.SpellHelper.ValidIndirectTarget( m_Mobile.SummonMaster, m );
					}

					if ( bCheckIt )
					{
						if ( acqType == FightMode.Agressor || acqType == FightMode.Evil )
						{
							bCheckIt = false;

							for ( int a = 0; !bCheckIt && a < m_Mobile.Aggressors.Count; ++a )
							{
								bCheckIt = (((AggressorInfo) m_Mobile.Aggressors[ a ]).Attacker == m);
							}

							for ( int a = 0; !bCheckIt && a < m_Mobile.Aggressed.Count; ++a )
							{
								bCheckIt = (((AggressorInfo) m_Mobile.Aggressed[ a ]).Defender == m);
							}

							if ( acqType == FightMode.Evil && !bCheckIt )
							{
								bCheckIt = (m.Karma < 0);
							}
						}
					}

					if ( bCheckIt )
					{
						double theirVal = m_Mobile.GetValueFrom( m, acqType, bPlayerOnly );

						if ( theirVal > val && m_Mobile.InLOS( m ) )
						{
							newFocusMob = m;

							val = theirVal;
						}
					}
				}

				eable.Free();

				m_Mobile.FocusMob = newFocusMob;
			}

			return (m_Mobile.FocusMob != null);
		}

		/*
		 *
		 *  A Detect Hiden prototype
		 *
		 */

		public override void DetectHiden()
		{
			if ( m_Mobile.Deleted )
			{
				return;
			}

			Map map = m_Mobile.Map;

			if ( map != null )
			{
				double srcSkill = m_Mobile.Skills[ SkillName.DetectHidden ].Value;

				foreach ( Mobile trg in m_Mobile.GetMobilesInRange( m_Mobile.RangePerception ) )
				{
					if ( trg != m_Mobile && trg.Hidden && trg.AccessLevel < AccessLevel.GameMaster )
					{
						m_Mobile.DebugSay( "detecting {0}...", trg.Name );

						trg.RevealingAction();

						trg.SendLocalizedMessage( 500814 ); // You have been revealed!

					}
				}
			}
		}

		public bool CanDispel( Mobile m )
		{
			return (m is BaseCreature && ((BaseCreature) m).Summoned && m_Mobile.CanBeHarmful( m, false ));
		}

		public void DispelTarget( Mobile m )
		{
			Type t = m.GetType();

			bool dispellable = false;

			if ( m is BaseCreature )
			{
				dispellable = ((BaseCreature) m).Summoned;
			}

			if ( m_Mobile.CanSee( m ) && dispellable )
			{
				Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
				Effects.PlaySound( m, m.Map, 0x201 );

				m.Delete();
			}

		}

		public void OnFailedMove()
		{
			if ( !m_Mobile.DisallowAllMoves && (TeleportChance > Utility.RandomDouble()) )
			{
				if ( m_Mobile.Target != null )
				{
					m_Mobile.Target.Cancel( m_Mobile, TargetCancelType.Canceled );
				}

				m_Mobile.DebugSay( "teleporting..." );

				TryTeleport();
			}
			else if ( AquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "kill mode {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;

				Action = ActionType.Combat;
			}
			else
			{
				m_Mobile.DebugSay( "bad" );
			}
		}

		public void Run( Direction d )
		{
			if ( m_Mobile.Paralyzed || m_Mobile.Frozen || m_Mobile.DisallowAllMoves )
			{
				return;
			}

			m_Mobile.Direction = d | Direction.Running;

			if ( !DoMove( m_Mobile.Direction ) )
			{
				OnFailedMove();
			}
		}

		public void RunFrom( Mobile m )
		{
			Run( (m_Mobile.GetDirectionTo( m ) - 4) & Direction.Mask );
		}
	}
}