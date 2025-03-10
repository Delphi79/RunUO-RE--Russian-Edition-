using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Factions;

namespace Server.Engines.Craft
{
	public enum ConsumeType
	{
		All,
		Half,
		None
	}

	public interface ICraftable
	{
		int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue );
	}

	public class CraftItem
	{
		private CraftResCol m_arCraftRes;
		private CraftSkillCol m_arCraftSkill;
		private Type m_Type;

		private string m_GroupNameString;
		private int m_GroupNameNumber;

		private string m_NameString;
		private int m_NameNumber;

		private int m_Mana;
		private int m_Hits;
		private int m_Stam;

		private bool m_UseAllRes;

		private bool m_NeedHeat;
		private bool m_NeedOven;

		private bool m_UseSubRes2;

		private bool m_RequiresSE;

		private static Hashtable m_ItemIDs = new Hashtable();

		public static int ItemIDOf( Type type )
		{
			object obj = m_ItemIDs[ type ];

			if ( obj != null )
			{
				return (int) obj;
			}

			int itemID = 0;

			if ( type == typeof( FactionExplosionTrap ) )
			{
				itemID = 14034;
			}
			else if ( type == typeof( FactionGasTrap ) )
			{
				itemID = 4523;
			}
			else if ( type == typeof( FactionSawTrap ) )
			{
				itemID = 4359;
			}
			else if ( type == typeof( FactionSpikeTrap ) )
			{
				itemID = 4517;
			}

			if ( itemID == 0 )
			{
				Item item = null;

				try
				{
					item = Activator.CreateInstance( type ) as Item;
				} 
				catch
				{
				}

				if ( item != null )
				{
					itemID = item.ItemID;
					item.Delete();
				}
			}

			m_ItemIDs[ type ] = itemID;

			return itemID;
		}

		public CraftItem( Type type, TextDefinition groupName, TextDefinition name )
		{
			m_arCraftRes = new CraftResCol();
			m_arCraftSkill = new CraftSkillCol();

			m_Type = type;

			m_GroupNameString = groupName;
			m_NameString = name;

			m_GroupNameNumber = groupName;
			m_NameNumber = name;
		}

		public void AddRes( Type type, TextDefinition name, int amount )
		{
			AddRes( type, name, amount, "" );
		}

		public void AddRes( Type type, TextDefinition name, int amount, TextDefinition message )
		{
			CraftRes craftRes = new CraftRes( type, name, amount, message );
			m_arCraftRes.Add( craftRes );
		}


		public void AddSkill( SkillName skillToMake, double minSkill, double maxSkill )
		{
			CraftSkill craftSkill = new CraftSkill( skillToMake, minSkill, maxSkill );
			m_arCraftSkill.Add( craftSkill );
		}

		public int Mana { get { return m_Mana; } set { m_Mana = value; } }

		public int Hits { get { return m_Hits; } set { m_Hits = value; } }

		public int Stam { get { return m_Stam; } set { m_Stam = value; } }

		public bool UseSubRes2 { get { return m_UseSubRes2; } set { m_UseSubRes2 = value; } }

		public bool UseAllRes { get { return m_UseAllRes; } set { m_UseAllRes = value; } }

		public bool NeedHeat { get { return m_NeedHeat; } set { m_NeedHeat = value; } }

		public bool NeedOven { get { return m_NeedOven; } set { m_NeedOven = value; } }

		public Type ItemType { get { return m_Type; } }

		public string GroupNameString { get { return m_GroupNameString; } }

		public int GroupNameNumber { get { return m_GroupNameNumber; } }

		public string NameString { get { return m_NameString; } }

		public int NameNumber { get { return m_NameNumber; } }

		public CraftResCol Ressources { get { return m_arCraftRes; } }

		public CraftSkillCol Skills { get { return m_arCraftSkill; } }


		public bool RequiresSE { get { return m_RequiresSE; } set { m_RequiresSE = value; } }

		public bool ConsumeAttributes( Mobile from, ref object message, bool consume )
		{
			bool consumMana = false;
			bool consumHits = false;
			bool consumStam = false;

			if ( Hits > 0 && from.Hits < Hits )
			{
				message = "You lack the required hit points to make that.";
				return false;
			}
			else
			{
				consumHits = consume;
			}

			if ( Mana > 0 && from.Mana < Mana )
			{
				message = "You lack the required mana to make that.";
				return false;
			}
			else
			{
				consumMana = consume;
			}

			if ( Stam > 0 && from.Stam < Stam )
			{
				message = "You lack the required stamina to make that.";
				return false;
			}
			else
			{
				consumStam = consume;
			}

			if ( consumMana )
			{
				from.Mana -= Mana;
			}

			if ( consumHits )
			{
				from.Hits -= Hits;
			}

			if ( consumStam )
			{
				from.Stam -= Stam;
			}

			return true;
		}

		private static Type[][] m_TypesTable = new Type[][] {new Type[] {typeof( Log ), typeof( Board )}, new Type[] {typeof( Leather ), typeof( Hides )}, new Type[] {typeof( SpinedLeather ), typeof( SpinedHides )}, new Type[] {typeof( HornedLeather ), typeof( HornedHides )}, new Type[] {typeof( BarbedLeather ), typeof( BarbedHides )}, new Type[] {typeof( BlankMap ), typeof( BlankScroll )}, new Type[] {typeof( Cloth ), typeof( UncutCloth )}, new Type[] {typeof( CheeseWheel ), typeof( CheeseWedge )}, new Type[] {typeof( Pumpkin ), typeof( SmallPumpkin )}, new Type[] {typeof( BowlOfPeas ), typeof( PewterBowl )}};

		private static Type[] m_ColoredItemTable = new Type[] {typeof( BaseWeapon ), typeof( BaseArmor ), typeof( BaseClothing ), typeof( BaseJewel ), typeof( DragonBardingDeed ), typeof( CutUpCloth ), typeof( CombineCloth )};

		private static Type[] m_ColoredResourceTable = new Type[] {typeof( BaseIngot ), typeof( BaseOre ), typeof( BaseLeather ), typeof( BaseHides ), typeof( UncutCloth ), typeof( Cloth ), typeof( BoltOfCloth ), typeof( BaseGranite ), typeof( BaseScales )};

		private static Type[] m_MarkableTable = new Type[] {typeof( BaseArmor ), typeof( BaseWeapon ), typeof( BaseClothing ), typeof( BaseInstrument ), typeof( DragonBardingDeed ), typeof( BaseTool ), typeof( FukiyaDart ), typeof( Shuriken ), typeof( Fukiya ), typeof( LeatherNinjaBelt ), typeof( Spellbook ), typeof( Runebook ), typeof( KeyRing ), typeof( IronKey )};

		public bool IsMarkable( Type type )
		{
			for ( int i = 0; i < m_MarkableTable.Length; ++i )
			{
				if ( type == m_MarkableTable[ i ] || type.IsSubclassOf( m_MarkableTable[ i ] ) )
				{
					return true;
				}
			}

			return false;
		}

		public bool RetainsColorFrom( CraftSystem system, Type type )
		{
			if ( system.RetainsColorFrom( this, type ) )
			{
				return true;
			}

			bool inItemTable = false, inResourceTable = false;

			for ( int i = 0; !inItemTable && i < m_ColoredItemTable.Length; ++i )
			{
				inItemTable = (m_Type == m_ColoredItemTable[ i ] || m_Type.IsSubclassOf( m_ColoredItemTable[ i ] ));
			}

			for ( int i = 0; inItemTable && !inResourceTable && i < m_ColoredResourceTable.Length; ++i )
			{
				inResourceTable = (type == m_ColoredResourceTable[ i ] || type.IsSubclassOf( m_ColoredResourceTable[ i ] ));
			}

			return (inItemTable && inResourceTable);
		}

		private static int[] m_HeatSources = new int[] {0x461, 0x48E, // Sandstone oven/fireplace
			0x92B, 0x96C, // Stone oven/fireplace
			0xDE3, 0xDE9, // Campfire
			0xFAC, 0xFAC, // Firepit
			0x184A, 0x184C, // Heating stand (left)
			0x184E, 0x1850, // Heating stand (right)
			0x398C, 0x399F // Fire field
		};

		private static int[] m_Ovens = new int[] {0x461, 0x46F, // Sandstone oven
			0x92B, 0x93F // Stone oven
		};

		public bool Find( Mobile from, int[] itemIDs )
		{
			Map map = from.Map;

			if ( map == null )
			{
				return false;
			}

			IPooledEnumerable eable = map.GetItemsInRange( from.Location, 2 );

			foreach ( Item item in eable )
			{
				if ( (item.Z + 16) > from.Z && (from.Z + 16) > item.Z && Find( item.ItemID, itemIDs ) )
				{
					eable.Free();
					return true;
				}
			}

			eable.Free();

			for ( int x = -2; x <= 2; ++x )
			{
				for ( int y = -2; y <= 2; ++y )
				{
					int vx = from.X + x;
					int vy = from.Y + y;

					Tile[] tiles = map.Tiles.GetStaticTiles( vx, vy, true );

					for ( int i = 0; i < tiles.Length; ++i )
					{
						int z = tiles[ i ].Z;
						int id = tiles[ i ].ID & 0x3FFF;

						if ( (z + 16) > from.Z && (from.Z + 16) > z && Find( id, itemIDs ) )
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public bool Find( int itemID, int[] itemIDs )
		{
			bool contains = false;

			for ( int i = 0; !contains && i < itemIDs.Length; i += 2 )
			{
				contains = (itemID >= itemIDs[ i ] && itemID <= itemIDs[ i + 1 ]);
			}

			return contains;
		}

		public bool IsQuantityType( Type[][] types )
		{
			for ( int i = 0; i < types.Length; ++i )
			{
				Type[] check = types[ i ];

				for ( int j = 0; j < check.Length; ++j )
				{
					if ( typeof( IHasQuantity ).IsAssignableFrom( check[ j ] ) )
					{
						return true;
					}
				}
			}

			return false;
		}

		public int ConsumeQuantity( Container cont, Type[][] types, int[] amounts )
		{
			if ( types.Length != amounts.Length )
			{
				throw new ArgumentException();
			}

			Item[][] items = new Item[types.Length][];
			int[] totals = new int[types.Length];

			for ( int i = 0; i < types.Length; ++i )
			{
				items[ i ] = cont.FindItemsByType( types[ i ], true );

				for ( int j = 0; j < items[ i ].Length; ++j )
				{
					IHasQuantity hq = items[ i ][ j ] as IHasQuantity;

					if ( hq == null )
					{
						totals[ i ] += items[ i ][ j ].Amount;
					}
					else
					{
						if ( hq is BaseBeverage && ((BaseBeverage) hq).Content != BeverageType.Water )
						{
							continue;
						}

						totals[ i ] += hq.Quantity;
					}
				}

				if ( totals[ i ] < amounts[ i ] )
				{
					return i;
				}
			}

			for ( int i = 0; i < types.Length; ++i )
			{
				int need = amounts[ i ];

				for ( int j = 0; j < items[ i ].Length; ++j )
				{
					Item item = items[ i ][ j ];
					IHasQuantity hq = item as IHasQuantity;

					if ( hq == null )
					{
						int theirAmount = item.Amount;

						if ( theirAmount < need )
						{
							item.Delete();
							need -= theirAmount;
						}
						else
						{
							item.Consume( need );
							break;
						}
					}
					else
					{
						if ( hq is BaseBeverage && ((BaseBeverage) hq).Content != BeverageType.Water )
						{
							continue;
						}

						int theirAmount = hq.Quantity;

						if ( theirAmount < need )
						{
							hq.Quantity -= theirAmount;
							need -= theirAmount;
						}
						else
						{
							hq.Quantity -= need;
							break;
						}
					}
				}
			}

			return -1;
		}

		public int GetQuantity( Container cont, Type[] types )
		{
			Item[] items = cont.FindItemsByType( types, true );

			int amount = 0;

			for ( int i = 0; i < items.Length; ++i )
			{
				IHasQuantity hq = items[ i ] as IHasQuantity;

				if ( hq == null )
				{
					amount += items[ i ].Amount;
				}
				else
				{
					if ( hq is BaseBeverage && ((BaseBeverage) hq).Content != BeverageType.Water )
					{
						continue;
					}

					amount += hq.Quantity;
				}
			}

			return amount;
		}

		public bool ConsumeRes( Mobile from, Type typeRes, CraftSystem craftSystem, ref int resHue, ref int maxAmount, ConsumeType consumeType, ref object message )
		{
			return ConsumeRes( from, typeRes, craftSystem, ref resHue, ref maxAmount, consumeType, ref message, false );
		}

		public bool ConsumeRes( Mobile from, Type typeRes, CraftSystem craftSystem, ref int resHue, ref int maxAmount, ConsumeType consumeType, ref object message, bool isFailure )
		{
			Container ourPack = from.Backpack;

			if ( ourPack == null )
			{
				return false;
			}

			if ( m_NeedHeat && !Find( from, m_HeatSources ) )
			{
				message = 1044487; // You must be near a fire source to cook.
				return false;
			}

			if ( m_NeedOven && !Find( from, m_Ovens ) )
			{
				message = 1044493; // You must be near an oven to bake that.
				return false;
			}

			Type[][] types = new Type[m_arCraftRes.Count][];
			int[] amounts = new int[m_arCraftRes.Count];

			maxAmount = int.MaxValue;

			CraftSubResCol resCol = (m_UseSubRes2 ? craftSystem.CraftSubRes2 : craftSystem.CraftSubRes);

			for ( int i = 0; i < types.Length; ++i )
			{
				CraftRes craftRes = m_arCraftRes.GetAt( i );
				Type baseType = craftRes.ItemType;

				// Resource Mutation
				if ( (baseType == resCol.ResType) && (typeRes != null) )
				{
					baseType = typeRes;

					CraftSubRes subResource = resCol.SearchFor( baseType );

					if ( subResource != null && from.Skills[ craftSystem.MainSkill ].Base < subResource.RequiredSkill )
					{
						message = subResource.Message;
						return false;
					}
				}
				// ******************

				for ( int j = 0; types[ i ] == null && j < m_TypesTable.Length; ++j )
				{
					if ( m_TypesTable[ j ][ 0 ] == baseType )
					{
						types[ i ] = m_TypesTable[ j ];
					}
				}

				if ( types[ i ] == null )
				{
					types[ i ] = new Type[] {baseType};
				}

				/*if ( !retainedColor && RetainsColorFrom( craftSystem, baseType ) )
				{
					retainedColor = true;
					Item resItem = ourPack.FindItemByType( types[i] );

					if ( resItem != null )
						resHue = resItem.Hue;
				}*/

				amounts[ i ] = craftRes.Amount;

				// For stackable items that can ben crafted more than one at a time
				if ( UseAllRes )
				{
					int tempAmount = ourPack.GetAmount( types[ i ] );
					tempAmount /= amounts[ i ];
					if ( tempAmount < maxAmount )
					{
						maxAmount = tempAmount;

						if ( maxAmount == 0 )
						{
							CraftRes res = m_arCraftRes.GetAt( i );

							if ( res.MessageNumber > 0 )
							{
								message = res.MessageNumber;
							}
							else if ( res.MessageString != null && res.MessageString != String.Empty )
							{
								message = res.MessageString;
							}
							else
							{
								message = 502925; // You don't have the resources required to make that item.
							} 

							return false;
						}
					}
				}
				// ****************************

				if ( isFailure && !craftSystem.ConsumeOnFailure( from, types[ i ][ 0 ], this ) )
				{
					amounts[ i ] = 0;
				}
			}

			// We adjust the amount of each resource to consume the max posible
			if ( UseAllRes )
			{
				for ( int i = 0; i < amounts.Length; ++i )
				{
					amounts[ i ] *= maxAmount;
				}
			}
			else
			{
				maxAmount = -1;
			}

			Item consumeExtra = null;

			if ( m_NameNumber == 1041267 )
			{
				// Runebooks are a special case, they need a blank recall rune

				Item[] runes = ourPack.FindItemsByType( typeof( RecallRune ) );

				for ( int i = 0; i < runes.Length; ++i )
				{
					RecallRune rune = runes[ i ] as RecallRune;

					if ( rune != null && !rune.Marked )
					{
						consumeExtra = rune;
						break;
					}
				}

				if ( consumeExtra == null )
				{
					message = 1044253; // You don't have the components needed to make that.
					return false;
				}
			}

			int index = 0;

			// Consume ALL
			if ( consumeType == ConsumeType.All )
			{
				m_ResHue = 0;
				m_ResAmount = 0;
				m_System = craftSystem;

				if ( IsQuantityType( types ) )
				{
					index = ConsumeQuantity( ourPack, types, amounts );
				}
				else
				{
					index = ourPack.ConsumeTotalGrouped( types, amounts, true, new OnItemConsumed( OnResourceConsumed ), new CheckItemGroup( CheckHueGrouping ) );
				}

				resHue = m_ResHue;
			}

				// Consume Half ( for use all resource craft type )
			else if ( consumeType == ConsumeType.Half )
			{
				for ( int i = 0; i < amounts.Length; i++ )
				{
					amounts[ i ] /= 2;

					if ( amounts[ i ] < 1 )
					{
						amounts[ i ] = 1;
					}
				}

				m_ResHue = 0;
				m_ResAmount = 0;
				m_System = craftSystem;

				if ( IsQuantityType( types ) )
				{
					index = ConsumeQuantity( ourPack, types, amounts );
				}
				else
				{
					index = ourPack.ConsumeTotalGrouped( types, amounts, true, new OnItemConsumed( OnResourceConsumed ), new CheckItemGroup( CheckHueGrouping ) );
				}

				resHue = m_ResHue;
			}

			else // ConstumeType.None ( it's basicaly used to know if the crafter has enough resource before starting the process )
			{
				index = -1;

				if ( IsQuantityType( types ) )
				{
					for ( int i = 0; i < types.Length; i++ )
					{
						if ( GetQuantity( ourPack, types[ i ] ) < amounts[ i ] )
						{
							index = i;
							break;
						}
					}
				}
				else
				{
					for ( int i = 0; i < types.Length; i++ )
					{
						if ( ourPack.GetBestGroupAmount( types[ i ], true, new CheckItemGroup( CheckHueGrouping ) ) < amounts[ i ] )
						{
							index = i;
							break;
						}
					}
				}
			}

			if ( index == -1 )
			{
				if ( consumeType != ConsumeType.None )
				{
					if ( consumeExtra != null )
					{
						consumeExtra.Delete();
					}
				}

				return true;
			}
			else
			{
				CraftRes res = m_arCraftRes.GetAt( index );

				if ( res.MessageNumber > 0 )
				{
					message = res.MessageNumber;
				}
				else if ( res.MessageString != null && res.MessageString != String.Empty )
				{
					message = res.MessageString;
				}
				else
				{
					message = 502925; // You don't have the resources required to make that item.
				} 

				return false;
			}
		}

		private int m_ResHue;
		private int m_ResAmount;
		private CraftSystem m_System;

		private void OnResourceConsumed( Item item, int amount )
		{
			if ( !RetainsColorFrom( m_System, item.GetType() ) )
			{
				return;
			}

			if ( amount >= m_ResAmount )
			{
				m_ResHue = item.Hue;
				m_ResAmount = amount;
			}
		}

		private int CheckHueGrouping( Item a, Item b )
		{
			return b.Hue.CompareTo( a.Hue );
		}

		public double GetExceptionalChance( CraftSystem system, double chance, Mobile from )
		{
			switch ( system.ECA )
			{
				default:
				case CraftECA.ChanceMinusSixty:
					return chance - 0.6;
				case CraftECA.FiftyPercentChanceMinusTenPercent:
					return (chance*0.5) - 0.1;
				case CraftECA.ChanceMinusSixtyToFourtyFive:
					{
						double offset = 0.60 - ((from.Skills[ system.MainSkill ].Value - 95.0)*0.03);

						if ( offset < 0.45 )
						{
							offset = 0.45;
						}
						else if ( offset > 0.60 )
						{
							offset = 0.60;
						}

						return chance - offset;
					}
			}
		}

		public bool CheckSkills( Mobile from, Type typeRes, CraftSystem craftSystem, ref int quality, ref bool allRequiredSkills )
		{
			return CheckSkills( from, typeRes, craftSystem, ref quality, ref allRequiredSkills, true );
		}

		public bool CheckSkills( Mobile from, Type typeRes, CraftSystem craftSystem, ref int quality, ref bool allRequiredSkills, bool gainSkills )
		{
			double chance = GetSuccessChance( from, typeRes, craftSystem, gainSkills, ref allRequiredSkills );

			if ( GetExceptionalChance( craftSystem, chance, from ) > Utility.RandomDouble() )
			{
				quality = 2;
			}

			return (chance > Utility.RandomDouble());
		}

		public double GetSuccessChance( Mobile from, Type typeRes, CraftSystem craftSystem, bool gainSkills, ref bool allRequiredSkills )
		{
			double minMainSkill = 0.0;
			double maxMainSkill = 0.0;
			double valMainSkill = 0.0;

			allRequiredSkills = true;

			for ( int i = 0; i < m_arCraftSkill.Count; i++ )
			{
				CraftSkill craftSkill = m_arCraftSkill.GetAt( i );

				double minSkill = craftSkill.MinSkill;
				double maxSkill = craftSkill.MaxSkill;
				double valSkill = from.Skills[ craftSkill.SkillToMake ].Value;

				if ( valSkill < minSkill )
				{
					allRequiredSkills = false;
				}

				if ( craftSkill.SkillToMake == craftSystem.MainSkill )
				{
					minMainSkill = minSkill;
					maxMainSkill = maxSkill;
					valMainSkill = valSkill;
				}

				if ( gainSkills ) // This is a passive check. Success chance is entirely dependant on the main skill
				{
					from.CheckSkill( craftSkill.SkillToMake, minSkill, maxSkill );
				}
			}

			double chance;

			if ( allRequiredSkills )
			{
				chance = craftSystem.GetChanceAtMin( this ) + ((valMainSkill - minMainSkill)/(maxMainSkill - minMainSkill)*(1.0 - craftSystem.GetChanceAtMin( this )));
			}
			else
			{
				chance = 0.0;
			}

			if ( allRequiredSkills && valMainSkill == maxMainSkill )
			{
				chance = 1.0;
			}

			return chance;
		}

		public void Craft( Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool )
		{
			if ( from.BeginAction( typeof( CraftSystem ) ) )
			{
				Item item = Activator.CreateInstance( ItemType ) as Item;

				if ( item is BaseCraftableTrap )
				{
					BaseCraftableTrap trap = item as BaseCraftableTrap;

					trap.OnCraft( 0, false, from, craftSystem, typeRes, tool, this, 0 );

					return;
				}

				if ( item is CutUpCloth )
				{
					CutUpCloth cutup = item as CutUpCloth;

					Item[] items = from.Backpack.FindItemsByType( typeof( BoltOfCloth ) );

					if ( items.Length > 0 )
					{
						cutup.CutUp( from, items );

						bool toolBroken = false;

						tool.UsesRemaining--;

						if ( tool.UsesRemaining < 1 )
						{
							toolBroken = true;
						}

						if ( toolBroken )
						{
							tool.Delete();

							from.SendLocalizedMessage( 1044038 ); // You have worn out your tool!

							from.SendLocalizedMessage( 1044154 ); // // You create the item.
						}

						from.EndAction( typeof( CraftSystem ) );

						if ( !toolBroken )
						{
							// You create the item.
							from.SendGump( new CraftGump( from, craftSystem, tool, 1044154 ) );
						} 
					}
					else
					{
						from.EndAction( typeof( CraftSystem ) );

						from.SendGump( new CraftGump( from, craftSystem, tool, 1044253 ) ); // You don't have the components needed to make that.
					}

					cutup.Delete();

					return;
				}

				if ( item is CombineCloth )
				{
					CombineCloth combine = item as CombineCloth;

					Item[] items = from.Backpack.FindItemsByType( typeof( Cloth ) );

					if ( items.Length > 0 )
					{
						combine.Combine( from, items );

						bool toolBroken = false;

						tool.UsesRemaining--;

						if ( tool.UsesRemaining < 1 )
						{
							toolBroken = true;
						}

						if ( toolBroken )
						{
							tool.Delete();

							from.SendLocalizedMessage( 1044038 ); // You have worn out your tool!

							from.SendLocalizedMessage( 1044154 ); // // You create the item.
						}

						from.EndAction( typeof( CraftSystem ) );

						if ( !toolBroken )
						{
							// You create the item.
							from.SendGump( new CraftGump( from, craftSystem, tool, 1044154 ) );
						} 
					}
					else
					{
						from.EndAction( typeof( CraftSystem ) );

						from.SendGump( new CraftGump( from, craftSystem, tool, 1044253 ) ); // You don't have the components needed to make that.
					}

					combine.Delete();

					return;
				}

				item.Delete();

				int flags = from.NetState == null ? 0 : from.NetState.Flags;

				if ( !RequiresSE || (flags & 0x10) != 0 ) // SE 2D = 0x1F SE 3D = 0x11F
				{
					bool allRequiredSkills = true;
					double chance = GetSuccessChance( from, typeRes, craftSystem, false, ref allRequiredSkills );

					if ( allRequiredSkills && chance >= 0.0 )
					{
						int badCraft = craftSystem.CanCraft( from, tool, m_Type );

						if ( badCraft <= 0 )
						{
							int resHue = 0;
							int maxAmount = 0;
							object message = null;

							if ( ConsumeRes( from, typeRes, craftSystem, ref resHue, ref maxAmount, ConsumeType.None, ref message ) )
							{
								message = null;

								if ( ConsumeAttributes( from, ref message, false ) )
								{
									CraftContext context = craftSystem.GetContext( from );

									if ( context != null )
									{
										context.OnMade( this );
									}

									int iMin = craftSystem.MinCraftEffect;
									int iMax = (craftSystem.MaxCraftEffect - iMin) + 1;
									int iRandom = Utility.Random( iMax );
									iRandom += iMin + 1;
									new InternalTimer( from, craftSystem, this, typeRes, tool, iRandom ).Start();
								}
								else
								{
									from.EndAction( typeof( CraftSystem ) );
									from.SendGump( new CraftGump( from, craftSystem, tool, message ) );
								}
							}
							else
							{
								from.EndAction( typeof( CraftSystem ) );
								from.SendGump( new CraftGump( from, craftSystem, tool, message ) );
							}
						}
						else
						{
							from.EndAction( typeof( CraftSystem ) );
							from.SendGump( new CraftGump( from, craftSystem, tool, badCraft ) );
						}
					}
					else
					{
						from.EndAction( typeof( CraftSystem ) );
						from.SendGump( new CraftGump( from, craftSystem, tool, 1044153 ) ); // You don't have the required skills to attempt this item.
					}

				}
				else
				{
					from.EndAction( typeof( CraftSystem ) );
					from.SendGump( new CraftGump( from, craftSystem, tool, 1063307 ) ); //The "Samurai Empire" expansion is required to attempt this item.
				}
			}
			else
			{
				from.SendLocalizedMessage( 500119 ); // You must wait to perform another action
			}
		}

		public void CompleteCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool )
		{
			int badCraft = craftSystem.CanCraft( from, tool, m_Type );

			if ( badCraft > 0 )
			{
				if ( tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
				{
					from.SendGump( new CraftGump( from, craftSystem, tool, badCraft ) );
				}
				else
				{
					from.SendLocalizedMessage( badCraft );
				}

				return;
			}

			int checkResHue = 0, checkMaxAmount = 0;
			object checkMessage = null;

			// Not enough resource to craft it
			if ( !ConsumeRes( from, typeRes, craftSystem, ref checkResHue, ref checkMaxAmount, ConsumeType.None, ref checkMessage ) )
			{
				if ( tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
				{
					from.SendGump( new CraftGump( from, craftSystem, tool, checkMessage ) );
				}
				else if ( checkMessage is int && (int) checkMessage > 0 )
				{
					from.SendLocalizedMessage( (int) checkMessage );
				}
				else if ( checkMessage is string )
				{
					from.SendMessage( (string) checkMessage );
				}

				return;
			}
			else if ( !ConsumeAttributes( from, ref checkMessage, false ) )
			{
				if ( tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
				{
					from.SendGump( new CraftGump( from, craftSystem, tool, checkMessage ) );
				}
				else if ( checkMessage is int && (int) checkMessage > 0 )
				{
					from.SendLocalizedMessage( (int) checkMessage );
				}
				else if ( checkMessage is string )
				{
					from.SendMessage( (string) checkMessage );
				}

				return;
			}

			bool toolBroken = false;

			int ignored = 1;
			int endquality = 1;

			bool allRequiredSkills = true;

			if ( CheckSkills( from, typeRes, craftSystem, ref ignored, ref allRequiredSkills ) )
			{
				// Resource
				int resHue = 0;
				int maxAmount = 0;

				object message = null;

				// Not enough resource to craft it
				if ( !ConsumeRes( from, typeRes, craftSystem, ref resHue, ref maxAmount, ConsumeType.All, ref message ) )
				{
					if ( tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
					{
						from.SendGump( new CraftGump( from, craftSystem, tool, message ) );
					}
					else if ( message is int && (int) message > 0 )
					{
						from.SendLocalizedMessage( (int) message );
					}
					else if ( message is string )
					{
						from.SendMessage( (string) message );
					}

					return;
				}
				else if ( !ConsumeAttributes( from, ref message, true ) )
				{
					if ( tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
					{
						from.SendGump( new CraftGump( from, craftSystem, tool, message ) );
					}
					else if ( message is int && (int) message > 0 )
					{
						from.SendLocalizedMessage( (int) message );
					}
					else if ( message is string )
					{
						from.SendMessage( (string) message );
					}

					return;
				}

				tool.UsesRemaining--;

				if ( craftSystem is DefBlacksmithy )
				{
					AncientSmithyHammer hammer = from.FindItemOnLayer( Layer.OneHanded ) as AncientSmithyHammer;
					if ( hammer != null && hammer != tool )
					{
						hammer.UsesRemaining--;
						if ( hammer.UsesRemaining < 1 )
						{
							hammer.Delete();
						}
					}
				}

				if ( tool.UsesRemaining < 1 )
				{
					toolBroken = true;
				}

				if ( toolBroken )
				{
					tool.Delete();
				}

				Item item;
				if ( typeof( MapItem ).IsAssignableFrom( ItemType ) && from.Map != Map.Trammel && from.Map != Map.Felucca )
				{
					item = new IndecipherableMap();
					from.SendLocalizedMessage( 1070800 ); // The map you create becomes mysteriously indecipherable.
				}
				else
				{
					item = Activator.CreateInstance( ItemType ) as Item;
				}

				if ( item != null )
				{
					if ( item is ICraftable )
					{
						endquality = ((ICraftable) item).OnCraft( quality, makersMark, from, craftSystem, typeRes, tool, this, resHue );
					}
					else if ( item.Hue == 0 )
					{
						item.Hue = resHue;
					}

					if ( maxAmount > 0 )
					{
						if ( !item.Stackable && item is IUsesRemaining )
						{
							((IUsesRemaining) item).UsesRemaining *= maxAmount;
						}
						else
						{
							item.Amount = maxAmount;
						}
					}

					// TODO: Mark when cheater mark is on resource or on ancient hammer
					if ( tool.Cheater_Name != null )
					{
						item.Cheater_Name = tool.Cheater_Name;
					}

					if ( from.AccessLevel > AccessLevel.Player )
					{
						item.Cheater_Name = String.Format( "This item crafted by GM {0}", from.Name );
					}

					from.AddToBackpack( item );

					//from.PlaySound( 0x57 );
				}

				int num = craftSystem.PlayEndingEffect( from, false, true, toolBroken, endquality, makersMark, this );

				bool queryFactionImbue = false;
				int availableSilver = 0;
				FactionItemDefinition def = null;
				Faction faction = null;

				if ( item is IFactionItem )
				{
					def = FactionItemDefinition.Identify( item );

					if ( def != null )
					{
						faction = Faction.Find( from );

						if ( faction != null )
						{
							Town town = Town.FromRegion( from.Region );

							if ( town != null && town.Owner == faction )
							{
								Container pack = from.Backpack;

								if ( pack != null )
								{
									availableSilver = pack.GetAmount( typeof( Silver ) );

									if ( availableSilver >= def.SilverCost )
									{
										queryFactionImbue = Faction.IsNearType( from, def.VendorType, 12 );
									}
								}
							}
						}
					}
				}

				// TODO: Scroll imbuing

				if ( queryFactionImbue )
				{
					from.SendGump( new FactionImbueGump( quality, item, from, craftSystem, tool, num, availableSilver, faction, def ) );
				}
				else if ( tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
				{
					from.SendGump( new CraftGump( from, craftSystem, tool, num ) );
				}
				else if ( num > 0 )
				{
					from.SendLocalizedMessage( num );
				}
			}
			else if ( !allRequiredSkills )
			{
				if ( tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
				{
					from.SendGump( new CraftGump( from, craftSystem, tool, 1044153 ) );
				}
				else
				{
					// You don't have the required skills to attempt this item.
					from.SendLocalizedMessage( 1044153 );
				} 
			}
			else
			{
				ConsumeType consumeType = (UseAllRes ? ConsumeType.Half : ConsumeType.All);
				int resHue = 0;
				int maxAmount = 0;

				object message = null;

				// Not enough resource to craft it
				if ( !ConsumeRes( from, typeRes, craftSystem, ref resHue, ref maxAmount, consumeType, ref message, true ) )
				{
					if ( tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
					{
						from.SendGump( new CraftGump( from, craftSystem, tool, message ) );
					}
					else if ( message is int && (int) message > 0 )
					{
						from.SendLocalizedMessage( (int) message );
					}
					else if ( message is string )
					{
						from.SendMessage( (string) message );
					}

					return;
				}

				tool.UsesRemaining--;

				if ( tool.UsesRemaining < 1 )
				{
					toolBroken = true;
				}

				if ( toolBroken )
				{
					tool.Delete();
				}

				// SkillCheck failed.
				int num = craftSystem.PlayEndingEffect( from, true, true, toolBroken, endquality, false, this );

				if ( tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
				{
					from.SendGump( new CraftGump( from, craftSystem, tool, num ) );
				}
				else if ( num > 0 )
				{
					from.SendLocalizedMessage( num );
				}
			}
		}

		private class InternalTimer : Timer
		{
			private Mobile m_From;
			private int m_iCount;
			private int m_iCountMax;
			private CraftItem m_CraftItem;
			private CraftSystem m_CraftSystem;
			private Type m_TypeRes;
			private BaseTool m_Tool;

			public InternalTimer( Mobile from, CraftSystem craftSystem, CraftItem craftItem, Type typeRes, BaseTool tool, int iCountMax ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( craftSystem.Delay ), iCountMax )
			{
				m_From = from;
				m_CraftItem = craftItem;
				m_iCount = 0;
				m_iCountMax = iCountMax;
				m_CraftSystem = craftSystem;
				m_TypeRes = typeRes;
				m_Tool = tool;
			}

			protected override void OnTick()
			{
				m_iCount++;

				m_From.DisruptiveAction();

				if ( m_iCount < m_iCountMax )
				{
					m_CraftSystem.PlayCraftEffect( m_From );
				}
				else
				{
					m_From.EndAction( typeof( CraftSystem ) );

					int badCraft = m_CraftSystem.CanCraft( m_From, m_Tool, m_CraftItem.m_Type );

					if ( badCraft > 0 )
					{
						if ( m_Tool != null && !m_Tool.Deleted && m_Tool.UsesRemaining > 0 )
						{
							m_From.SendGump( new CraftGump( m_From, m_CraftSystem, m_Tool, badCraft ) );
						}
						else
						{
							m_From.SendLocalizedMessage( badCraft );
						}

						return;
					}

					int quality = 1;
					bool allRequiredSkills = true;

					m_CraftItem.CheckSkills( m_From, m_TypeRes, m_CraftSystem, ref quality, ref allRequiredSkills, false );

					CraftContext context = m_CraftSystem.GetContext( m_From );

					if ( context == null )
					{
						return;
					}

					bool makersMark = false;

					if ( quality == 2 && m_From.Skills[ m_CraftSystem.MainSkill ].Base >= 100.0 )
					{
						makersMark = m_CraftItem.IsMarkable( m_CraftItem.ItemType );
					}

					if ( makersMark && context.MarkOption == CraftMarkOption.PromptForMark )
					{
						m_From.SendGump( new QueryMakersMarkGump( quality, m_From, m_CraftItem, m_CraftSystem, m_TypeRes, m_Tool ) );
					}
					else
					{
						if ( context.MarkOption == CraftMarkOption.DoNotMark )
						{
							makersMark = false;
						}

						m_CraftItem.CompleteCraft( quality, makersMark, m_From, m_CraftSystem, m_TypeRes, m_Tool );
					}
				}
			}
		}
	}
}