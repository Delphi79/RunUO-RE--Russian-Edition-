using System;
using Server;
using Server.Targeting;
using Server.Items;

namespace Server.Engines.Craft
{
	public class Resmelt
	{
		public Resmelt()
		{
		}

		public static void Do( Mobile from, CraftSystem craftSystem, BaseTool tool )
		{
			int num = craftSystem.CanCraft( from, tool, null );

			if ( num > 0 )
			{
				from.SendGump( new CraftGump( from, craftSystem, tool, num ) );
			}
			else
			{
				from.Target = new InternalTarget( craftSystem, tool );
				from.SendLocalizedMessage( 1044273 ); // Target an item to recycle.
			}
		}

		private class InternalTarget : Target
		{
			private CraftSystem m_CraftSystem;
			private BaseTool m_Tool;

			public InternalTarget( CraftSystem craftSystem, BaseTool tool ) : base( 2, false, TargetFlags.None )
			{
				m_CraftSystem = craftSystem;
				m_Tool = tool;
			}

			public bool CheckResourceSkill( CraftResource resource, double skill )
			{
				double reqSkill = 0;

				switch ( resource )
				{
					case CraftResource.DullCopper:
						reqSkill = 65.0;
						break;
					case CraftResource.ShadowIron:
						reqSkill = 70.0;
						break;
					case CraftResource.Copper:
						reqSkill = 75.0;
						break;
					case CraftResource.Bronze:
						reqSkill = 80.0;
						break;
					case CraftResource.Gold:
						reqSkill = 85.0;
						break;
					case CraftResource.Agapite:
						reqSkill = 90.0;
						break;
					case CraftResource.Verite:
						reqSkill = 95.0;
						break;
					case CraftResource.Valorite:
						reqSkill = 99.0;
						break;
				}

				if ( skill >= reqSkill )
				{
					return true;
				}

				return false;
			}

			public int GetAmount( int skill, int[] iterations )
			{
				for ( int i = 0; i < iterations.Length; i++ )
				{
					if ( iterations[ 0 ] > skill )
					{
						return 1;
					}

					if ( iterations[ i ] == skill )
					{
						return i + 2;
					}

					if ( i != 0 && iterations[ i ] > skill && iterations[ i - 1 ] < skill )
					{
						return i + 1;
					}

					if ( iterations[ iterations.Length - 1 ] < skill )
					{
						return iterations.Length + 1;
					}
				}

				return 0;
			}

			public int GetAmount( Item item, int skill, int amount )
			{
				int result = 1;

				if ( item is DragonBardingDeed )
				{
					if ( skill < 1 )
					{
						return 1;
					}

					result = 4 + (skill - 1)*5;

					if ( skill >= 81 )
					{
						result -= 4;
					}
					else if ( skill >= 61 && skill < 81 )
					{
						result -= 3;
					}
					else if ( skill >= 41 && skill < 61 )
					{
						result -= 2;
					}
					else if ( skill >= 21 && skill < 41 )
					{
						result -= 1;
					}
				}
				else if ( (item is BaseArmor && ((BaseArmor) item).PlayerConstructed) || (item is BaseWeapon && ((BaseWeapon) item).PlayerConstructed) || (item is BaseClothing && ((BaseClothing) item).PlayerConstructed) )
				{
					int[] iterations = new int[0];

					switch ( amount )
					{
						case 3:
							iterations = new int[1] {101};
							break;
						case 5:
							iterations = new int[2] {61, 91};
							break;
						case 6:
							iterations = new int[2] {51, 76};
							break;
						case 8:
							iterations = new int[4] {38, 57, 76, 95};
							break;
						case 10:
							iterations = new int[5] {31, 46, 61, 76, 91};
							break;
						case 12:
							iterations = new int[6] {26, 38, 51, 64, 76, 89};
							break;
						case 14:
							iterations = new int[8] {22, 33, 44, 55, 66, 77, 88, 99};
							break;
						case 15:
							iterations = new int[8] {21, 31, 41, 51, 61, 71, 81, 91};
							break;
						case 16:
							iterations = new int[9] {19, 29, 39, 49, 59, 69, 79, 89, 99};
							break;
						case 18:
							iterations = new int[10] {17, 26, 35, 44, 53, 62, 71, 80, 89, 98};
							break;
						case 20:
							iterations = new int[12] {16, 23, 31, 38, 46, 54, 61, 69, 76, 84, 91, 99};
							break;
						case 25:
							iterations = new int[15] {13, 19, 25, 31, 37, 43, 49, 55, 61, 67, 73, 79, 85, 91, 97};
							break;
						case 28:
							iterations = new int[17] {11, 17, 22, 28, 33, 38, 44, 49, 55, 60, 65, 71, 76, 82, 87, 92, 98};
							break;
					}

					result = GetAmount( skill, iterations );
				}

				return result;
			}

			private bool Resmelt( Mobile from, Item item, CraftResource resource )
			{
				try
				{
					if ( CraftResources.GetType( resource ) != CraftResourceType.Metal )
					{
						return false;
					}

					CraftResourceInfo info = CraftResources.GetInfo( resource );

					if ( info == null || info.ResourceTypes.Length == 0 )
					{
						return false;
					}

					if ( !CheckResourceSkill( info.Resource, from.Skills[ SkillName.Mining ].Value ) )
					{
						return false;
					}

					CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor( item.GetType() );

					if ( craftItem == null || craftItem.Ressources.Count == 0 )
					{
						return false;
					}

					CraftRes craftResource = craftItem.Ressources.GetAt( 0 );

					if ( craftResource.Amount < 2 )
					{
						return false; // Not enough metal to resmelt
					} 

					Type resourceType = info.ResourceTypes[ 0 ];
					Item ingot = (Item) Activator.CreateInstance( resourceType );

					ingot.Amount = GetAmount( item, (int) (from.Skills[ SkillName.Mining ].Value), craftResource.Amount );

					item.Delete();
					from.AddToBackpack( ingot );

					from.PlaySound( 0x2A );
					from.PlaySound( 0x240 );
					return true;
				} 
				catch
				{
				}

				return false;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				int num = m_CraftSystem.CanCraft( from, m_Tool, null );

				if ( num > 0 )
				{
					from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, num ) );
				}
				else
				{
					bool success = false;
					bool isStoreBought = false;
					bool lackMining = false;

					if ( targeted is BaseArmor )
					{
						success = Resmelt( from, (BaseArmor) targeted, ((BaseArmor) targeted).Resource );
						isStoreBought = !((BaseArmor) targeted).PlayerConstructed;
						lackMining = !(CheckResourceSkill( ((BaseArmor) targeted).Resource, from.Skills[ SkillName.Mining ].Value ));
					}
					else if ( targeted is BaseWeapon )
					{
						success = Resmelt( from, (BaseWeapon) targeted, ((BaseWeapon) targeted).Resource );
						isStoreBought = !((BaseWeapon) targeted).PlayerConstructed;
						lackMining = !(CheckResourceSkill( ((BaseWeapon) targeted).Resource, from.Skills[ SkillName.Mining ].Value ));
					}
					else if ( targeted is DragonBardingDeed )
					{
						success = Resmelt( from, (DragonBardingDeed) targeted, ((DragonBardingDeed) targeted).Resource );
						isStoreBought = false;
						lackMining = false;
					}

					if ( lackMining )
					{
						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 1044269 ) ); // You have no idea how to work this metal.

						return;
					}

					if ( success )
					{
						// You melt the item down into ingots.
						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, isStoreBought ? 500418 : 1044270 ) );
					} 
					else
					{
						// You can't melt that down into ingots.
						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 1044272 ) );
					} 
				}
			}
		}
	}
}