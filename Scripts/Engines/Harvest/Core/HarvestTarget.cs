using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Multis;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;

namespace Server.Engines.Harvest
{
	public class HarvestTarget : Target
	{
		private Item m_Tool;
		private HarvestSystem m_System;

		public HarvestTarget( Item tool, HarvestSystem system ) : base( -1, true, TargetFlags.None )
		{
			m_Tool = tool;
			m_System = system;

			DisallowMultis = true;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if ( m_System is Mining && targeted is StaticTarget )
			{
				int itemID = ((StaticTarget) targeted).ItemID;

				// grave
				if ( itemID == 0xED3 || itemID == 0xEDF || itemID == 0xEE0 || itemID == 0xEE1 || itemID == 0xEE2 || itemID == 0xEE8 )
				{
					PlayerMobile player = from as PlayerMobile;

					if ( player != null )
					{
						QuestSystem qs = player.Quest;

						if ( qs is WitchApprenticeQuest )
						{
							FindIngredientObjective obj = qs.FindObjective( typeof( FindIngredientObjective ) ) as FindIngredientObjective;

							if ( obj != null && !obj.Completed && obj.Ingredient == Ingredient.Bones )
							{
								player.SendLocalizedMessage( 1055037 ); // You finish your grim work, finding some of the specific bones listed in the Hag's recipe.
								obj.Complete();

								return;
							}
						}
					}
				}
			}

			if ( m_System is Lumberjacking && targeted is IChopable )
			{
				((IChopable) targeted).OnChop( from );
			}
			else if ( m_System is Lumberjacking && FurnitureAttribute.Check( targeted as Item ) )
			{
				DestroyFurniture( from, (Item) targeted );
			}
			else if ( m_System is Mining && targeted is TreasureMap )
			{
				((TreasureMap) targeted).OnBeginDig( from );
			}
			else
			{
				m_System.StartHarvesting( from, m_Tool, targeted );
			}
		}

		private void DestroyFurniture( Mobile from, Item item )
		{
			if ( !from.InRange( item.GetWorldLocation(), 3 ) )
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}
			else if ( !item.IsChildOf( from.Backpack ) && !item.Movable )
			{
				from.SendLocalizedMessage( 500462 ); // You can't destroy that while it is here.
				return;
			}

			from.SendLocalizedMessage( 500461 ); // You destroy the item.
			Effects.PlaySound( item.GetWorldLocation(), item.Map, 0x3B3 );

			if ( item is Container )
			{
				((Container) item).Destroy();
			}
			else
			{
				item.Delete();
			}
		}
	}
}