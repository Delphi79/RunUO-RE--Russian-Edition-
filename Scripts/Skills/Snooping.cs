using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Server.SkillHandlers
{
	public class Snooping
	{
		public static void Configure()
		{
			Container.SnoopHandler = new ContainerSnoopHandler( Container_Snoop );
		}

		public static bool CheckSnoopAllowed( Mobile from, Mobile to )
		{
			Map map = from.Map;

			if ( to.Player )
			{
				return from.CanBeHarmful( to, false, true ); // normal restrictions
			}

			if ( map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0 )
			{
				return true; // felucca you can snoop anybody
			}

			GuardedRegion reg = to.Region as GuardedRegion;

			if ( reg == null || reg.IsDisabled() )
			{
				return true; // not in town? we can snoop any npc
			}

			BaseCreature cret = to as BaseCreature;

			if ( to.Body.IsHuman && (cret == null || (!cret.AlwaysAttackable && !cret.AlwaysMurderer)) )
			{
				return false; // in town we cannot snoop blue human npcs
			}

			return true;
		}

		public static void Container_Snoop( Container cont, Mobile from )
		{
			if ( from.AccessLevel > AccessLevel.Player || from.InRange( cont.GetWorldLocation(), 1 ) )
			{
				Mobile root = cont.RootParent as Mobile;

				if ( root != null && !root.Alive )
				{
					return;
				}

				if ( root != null && root.AccessLevel > AccessLevel.Player && from.AccessLevel == AccessLevel.Player )
				{
					from.SendLocalizedMessage( 500209 ); // You can not peek into the container.
					return;
				}

				if ( root != null && from.AccessLevel == AccessLevel.Player && !CheckSnoopAllowed( from, root ) )
				{
					from.SendLocalizedMessage( 1001018 ); // You cannot perform negative acts on your target.
					return;
				}

				if ( root != null && from.AccessLevel == AccessLevel.Player && from.Skills[ SkillName.Snooping ].Value < Utility.Random( 100 ) )
				{
					Map map = from.Map;

					if ( map != null )
					{
						string message = String.Format( "You notice {0} attempting to peek into {1}'s belongings.", from.Name, root.Name );

						IPooledEnumerable eable = map.GetClientsInRange( from.Location, 8 );

						foreach ( NetState ns in eable )
						{
							if ( ns != from.NetState )
							{
								ns.Mobile.SendMessage( message );
							}
						}

						eable.Free();
					}
				}

				if ( from.AccessLevel == AccessLevel.Player )
				{
					Titles.AwardKarma( from, -4, true );
				}

				if ( from.AccessLevel > AccessLevel.Player || from.CheckTargetSkill( SkillName.Snooping, cont, 0.0, 100.0 ) )
				{
					if ( cont is TrapableContainer && ((TrapableContainer) cont).ExecuteTrap( from ) )
					{
						return;
					}

					cont.DisplayTo( from );
				}
				else
				{
					from.SendLocalizedMessage( 500210 ); // You failed to peek into the container.
				}
			}
			else
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			}
		}
	}
}