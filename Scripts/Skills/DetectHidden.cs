using System;
using Server.Multis;
using Server.Targeting;
using Server.Items;
using Server.Regions;
using Server.Factions;

namespace Server.SkillHandlers
{
	public class DetectHidden
	{
		public static void Initialize()
		{
			SkillInfo.Table[ (int) SkillName.DetectHidden ].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile src )
		{
			src.SendLocalizedMessage( 500819 ); //Where will you search?
			src.Target = new InternalTarget();

			return TimeSpan.FromSeconds( 1.0 );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 12, true, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile src, object targ )
			{
				bool foundAnyone = false;

				if ( targ is TrapableContainer )
				{
					TrapableContainer cont = targ as TrapableContainer;

					if ( cont.Enabled && cont.TrapType != TrapType.None && cont.TrapPower > 0 && src.CheckSkill( SkillName.DetectHidden, 0.0, 100.0 ) )
					{
						int hue = 0;

						switch ( cont.TrapType )
						{
							case TrapType.ExplosionTrap:
								hue = 0x78;
								break;
							case TrapType.DartTrap:
								hue = 0x5A;
								break;
							case TrapType.PoisonTrap:
								hue = 0x44;
								break;
						}

						cont.SendLocalizedMessageTo( src, 500813, hue ); // [trapped]
					}
				}

				Point3D p;
				if ( targ is Mobile )
				{
					p = ((Mobile) targ).Location;
				}
				else if ( targ is Item )
				{
					p = ((Item) targ).Location;
				}
				else if ( targ is IPoint3D )
				{
					p = new Point3D( (IPoint3D) targ );
				}
				else
				{
					p = src.Location;
				}

				double srcSkill = src.Skills[ SkillName.DetectHidden ].Value;
				int range = (int) (srcSkill/10.0);

				if ( !src.CheckSkill( SkillName.DetectHidden, 0.0, 100.0 ) )
				{
					range /= 2;
				}

				BaseHouse house = BaseHouse.FindHouseAt( p, src.Map, 16 );

				bool inHouse = (house != null && house.IsFriend( src ));

				if ( inHouse )
				{
					range = 22;
				}

				if ( range > 0 )
				{
					IPooledEnumerable inRange = src.Map.GetMobilesInRange( p, range );

					foreach ( Mobile trg in inRange )
					{
						if ( trg.Hidden && src != trg )
						{
							double ss = srcSkill + Utility.Random( 21 ) - 10;
							double ts = trg.Skills[ SkillName.Hiding ].Value + Utility.Random( 21 ) - 10;

							if ( src.AccessLevel >= trg.AccessLevel && (ss >= ts || (inHouse && house.IsInside( trg ))) )
							{
								if ( trg is Mobiles.ShadowKnight && (trg.X != p.X || trg.Y != p.Y) )
								{
									continue;
								}

								trg.RevealingAction();
								trg.SendLocalizedMessage( 500814 ); // You have been revealed!
								foundAnyone = true;
							}
						}
					}

					inRange.Free();

					if ( Faction.Find( src ) != null )
					{
						IPooledEnumerable itemsInRange = src.Map.GetItemsInRange( p, range );

						foreach ( Item item in itemsInRange )
						{
							if ( item is BaseFactionTrap )
							{
								BaseFactionTrap trap = (BaseFactionTrap) item;

								if ( src.CheckTargetSkill( SkillName.DetectHidden, trap, 80.0, 100.0 ) )
								{
									src.SendLocalizedMessage( 1042712, true, " " + (trap.Faction == null ? "" : trap.Faction.Definition.FriendlyName) ); // You reveal a trap placed by a faction:

									trap.Visible = true;
									trap.BeginConceal();

									foundAnyone = true;
								}
							}
						}

						itemsInRange.Free();
					}
				}

				if ( !foundAnyone )
				{
					src.SendLocalizedMessage( 500817 ); // You can see nothing hidden there.
				}
			}
		}
	}
}