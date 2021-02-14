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
	public class BossMeleeAI : BaseAI
	{
		private int blockcounter = 0;
		private FightMode curFightMode;
		private const double ChanceToDispel = 0.25;
		private const double ChanceToDetectHidden = 0.10;
		private const double CombatantChangeChance = 0.3;
		private const double TeleportChance = 0.5;

		public BossMeleeAI( BaseCreature m ) : base( m )
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
			m_Mobile.DebugSay( "No opponent" );
			SelectFightMode();
			if ( AquireFocusMob( m_Mobile.RangePerception, curFightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "I want to kill {0}", m_Mobile.FocusMob.Name );
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
				if ( m_Mobile == null || m_Mobile.Deleted )
				{
					return;
				}

				m_Mobile.DebugSay( "Need teleport to {0}?", m_Mobile.Combatant.Name );
				if ( ++blockcounter > 10 )
				{
					blockcounter = 0;
					if ( Utility.RandomDouble() <= TeleportChance )
					{
						m_Mobile.DebugSay( "I am teleporting to {0}", m_Mobile.Combatant.Name );
						Point3D to = new Point3D( m_Mobile.Combatant.Location );
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

			if ( CanDispel( c ) && m_Mobile.GetDistanceToSqrt( c ) <= 1 && (ChanceToDispel >= Utility.RandomDouble()) )
			{
				m_Mobile.DebugSay( "Dispel for {0}", c.Name );

				DispelTarget( c );

				Action = ActionType.Guard;

				return true;
			}

			if ( ChanceToDetectHidden >= Utility.RandomDouble() )
			{
				m_Mobile.DebugSay( "Check hiders...", c.Name );

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
				m_Mobile.DebugSay( "Go to kill {0}", m_Mobile.FocusMob.Name );
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
				return true;
			}
			else if ( m_Mobile.GetDistanceToSqrt( c ) > m_Mobile.RangePerception + 1 )
			{
				m_Mobile.DebugSay( "Can't find {0}", c.Name );

				Action = ActionType.Guard;

				return true;
			}
			else
			{
				m_Mobile.DebugSay( "Far away from {0}", c.Name );
			}

			if ( m_Mobile.Hits < m_Mobile.HitsMax*20/100 )
			{
				m_Mobile.DebugSay( "Bad.." );

				bool flee = false;

				if ( m_Mobile.Hits < c.Hits )
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
					m_Mobile.DebugSay( "Flee from {0}", c.Name );
					Action = ActionType.Flee;
					return true;
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( AquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "{0} - must be dead..", m_Mobile.FocusMob.Name );
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
				m_Mobile.DebugSay( "Heal completed" );
				Action = ActionType.Guard;
			}
			else if ( AquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "You are bad guy, {0}", m_Mobile.FocusMob.Name );

				RunFrom( m_Mobile.FocusMob );
				m_Mobile.FocusMob = null;
			}
			else
			{
				m_Mobile.DebugSay( "Strange, nothing here" );

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
						m_Mobile.DebugSay( "Yes, you have been revealed, {0}", trg.Name );

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

				m_Mobile.DebugSay( "Bad Location..." );
				TryTeleport();
			}
			else if ( AquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "Give me a pass {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				m_Mobile.DebugSay( "Bad..." );
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