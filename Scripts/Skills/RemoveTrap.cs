using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Factions;

namespace Server.SkillHandlers
{
	public class RemoveTrap
	{
		public static void Initialize()
		{
			SkillInfo.Table[ (int) SkillName.RemoveTrap ].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			if ( m.Skills[ SkillName.Lockpicking ].Value < 50 )
			{
				m.SendLocalizedMessage( 502366 ); // You do not know enough about locks.  Become better at picking locks.
			}
			else if ( m.Skills[ SkillName.DetectHidden ].Value < 50 )
			{
				m.SendLocalizedMessage( 502367 ); // You are not perceptive enough.  Become better at detect hidden.
			}
			else
			{
				m.Target = new InternalTarget();

				m.SendLocalizedMessage( 502368 ); // Wich trap will you attempt to disarm?

				if ( m.Hits <= m.Hits/10 )
				{
					m.SendLocalizedMessage( 502369 ); // You hesitate, and decide to start again.
				}
			}

			return TimeSpan.FromSeconds( 10.0 ); // 10 second delay before beign able to re-use a skill
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 2, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Mobile )
				{
					from.SendLocalizedMessage( 502816 ); // You feel that such an action would be inappropriate
				}
				else if ( targeted is TrapableContainer )
				{
					TrapableContainer targ = (TrapableContainer) targeted;

					from.Direction = from.GetDirectionTo( targ );

					if ( targ.TrapType == TrapType.None )
					{
						from.SendLocalizedMessage( 502373 ); // That doesn't appear to be trapped
						return;
					}

					from.PlaySound( 0x241 );

					if ( from.CheckTargetSkill( SkillName.RemoveTrap, targ, targ.TrapPower, targ.TrapPower + 30 ) )
					{
						switch ( targ.TrapType )
						{
							case TrapType.ExplosionTrap:
								from.LocalOverheadMessage( Network.MessageType.Regular, 0x78, 502374 ); // You carefully remove the trigger for the purple potion.
								break; 
							case TrapType.DartTrap:
								from.LocalOverheadMessage( Network.MessageType.Regular, 0x62, 502375 ); // You carefully remove the dart from the firing mechanism.
								break;
							case TrapType.PoisonTrap:
								from.LocalOverheadMessage( Network.MessageType.Regular, 0x44, 502376 ); // The poison leaks harmlessly away due to your deft touch.
								break;
							default:
								from.LocalOverheadMessage( Network.MessageType.Regular, 0x53, 502377 ); // You successfully render the trap harmless
								break;
						}

						targ.TrapPower = 0;
						targ.TrapType = TrapType.None;
						targ.Enabled = false;
					}
					else
					{
						if ( Utility.RandomDouble() <= 0.2 )
						{
							from.SendLocalizedMessage( 502370 ); // Oops.

							targ.ExecuteTrap( from );
						}
						else
						{
							from.SendLocalizedMessage( 502371 ); // You breathe a sigh of relief, as you fail to disarm the trap, but don't set it off.
						}
					}
				}
				else if ( targeted is BaseFactionTrap )
				{
					BaseFactionTrap trap = (BaseFactionTrap) targeted;
					Faction faction = Faction.Find( from );

					FactionTrapRemovalKit kit = (from.Backpack == null ? null : from.Backpack.FindItemByType( typeof( FactionTrapRemovalKit ) ) as FactionTrapRemovalKit);

					bool isOwner = (trap.Placer == from || (trap.Faction != null && trap.Faction.IsCommander( from )));

					if ( faction == null )
					{
						from.SendLocalizedMessage( 1010538 ); // You may not disarm faction traps unless you are in an opposing faction
					}
					else if ( faction == trap.Faction && trap.Faction != null && !isOwner )
					{
						from.SendLocalizedMessage( 1010537 ); // You may not disarm traps set by your own faction!
					}
					else if ( !isOwner && kit == null )
					{
						from.SendLocalizedMessage( 1042530 ); // You must have a trap removal kit at the base level of your pack to disarm a faction trap.
					}
					else
					{
						if ( from.CheckTargetSkill( SkillName.RemoveTrap, trap, 80.0, 100.0 ) && from.CheckTargetSkill( SkillName.Tinkering, trap, 80.0, 100.0 ) )
						{
							from.PrivateOverheadMessage( MessageType.Regular, trap.MessageHue, trap.DisarmMessage, from.NetState );

							if ( !isOwner )
							{
								int silver = faction.AwardSilver( from, trap.SilverFromDisarm );

								if ( silver > 0 )
								{
									from.SendLocalizedMessage( 1008113, true, silver.ToString( "N0" ) ); // You have been granted faction silver for removing the enemy trap :
								}
							}

							trap.Delete();
						}
						else
						{
							from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off
						}

						if ( !isOwner && kit != null )
						{
							kit.ConsumeCharge( from );
						}
					}
				}
				else
				{
					from.SendLocalizedMessage( 502373 ); // That does'nt appear to be trapped
				}
			}
		}
	}
}