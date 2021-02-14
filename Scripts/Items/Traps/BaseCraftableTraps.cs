using System;
using Server.Targeting;
using Server.Engines.Craft;

namespace Server.Items
{
	public class BaseCraftableTrap : BaseTrap, ICraftable
	{
		public override bool PassivelyTriggered { get { return false; } }
		public override TimeSpan PassiveTriggerDelay { get { return TimeSpan.Zero; } }
		public override int PassiveTriggerRange { get { return 0; } }
		public override TimeSpan ResetDelay { get { return TimeSpan.FromSeconds( 0.0 ); } }

		public BaseCraftableTrap( int itemID ) : base( itemID )
		{
		}

		public BaseCraftableTrap( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		#region ICraftable Members
		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			TrapType type = TrapType.DartTrap;

			if ( this is DartTrap )
			{
				type = TrapType.DartTrap;
			}
			else if ( this is PoisonTrap )
			{
				type = TrapType.PoisonTrap;
			}
			else if ( this is ExplosionTrap )
			{
				type = TrapType.ExplosionTrap;
			}

			from.EndAction( typeof( CraftSystem ) );

			from.Target = new TrapTarget( this, type, craftSystem, tool, typeRes, craftItem, resHue );

			from.SendLocalizedMessage( 502923 ); // What would you like to set a trap on?

			return quality;
		}
		#endregion

		private class TrapTarget : Target
		{
			private BaseCraftableTrap m_Trap;
			private TrapType m_Type;
			private CraftSystem m_CraftSystem;
			private BaseTool m_Tool;
			private Type m_typeRes;
			private CraftItem m_craftItem;
			private int m_resHue;

			public TrapTarget( BaseCraftableTrap trap, TrapType type, CraftSystem craftSystem, BaseTool tool, Type typeRes, CraftItem craftItem, int resHue ) : base( 2, false, TargetFlags.None )
			{
				m_Trap = trap;
				m_Type = type;
				m_CraftSystem = craftSystem;
				m_Tool = tool;
				m_typeRes = typeRes;
				m_craftItem = craftItem;
				m_resHue = resHue;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is LockableContainer )
				{
					LockableContainer lc = targeted as LockableContainer;

					if ( lc.Locked )
					{
						from.EndAction( typeof( CraftSystem ) );

						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 502943 ) ); // You can only trap an unlocked object.
					}
					else if ( lc.IsLockedDown )
					{
						from.EndAction( typeof( CraftSystem ) );

						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 502944 ) ); // You cannot trap this item because it is locked down.
					}
					else if ( lc.TrapType != TrapType.None )
					{
						from.EndAction( typeof( CraftSystem ) );

						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 502945 ) ); // You can only place one trap on an object at a time.
					}
					else if ( lc.RootParent != from )
					{
						from.EndAction( typeof( CraftSystem ) );

						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 502946 ) ); // That belongs to someone else.
					}
					else
					{
						object message = null;

						int maxAmount = 0;

						bool AllRequiredSkills = true;

						if ( m_craftItem.ConsumeRes( from, m_typeRes, m_CraftSystem, ref m_resHue, ref maxAmount, ConsumeType.All, ref message ) )
						{
							double chance = m_craftItem.GetSuccessChance( from, m_typeRes, m_CraftSystem, true, ref AllRequiredSkills );

							if ( AllRequiredSkills )
							{
								if ( chance > Utility.RandomDouble() )
								{
									lc.Enabled = true;
									lc.TrapType = m_Type;

									switch ( m_Type )
									{
										case TrapType.DartTrap:
											lc.TrapPower = Utility.RandomMinMax( 50, 70 );
											break;
										case TrapType.ExplosionTrap:
											lc.TrapPower = Utility.RandomMinMax( 30, 50 );
											break;
											// at OSI Poison Trap possible no causes direct damage, only poison effect
									}

									bool toolBroken = false;

									m_Tool.UsesRemaining--;

									if ( m_Tool.UsesRemaining < 1 )
									{
										toolBroken = true;
									}

									if ( toolBroken )
									{
										m_Tool.Delete();

										from.SendLocalizedMessage( 1044038 ); // You have worn out your tool!

										from.SendLocalizedMessage( 1005639 ); // Trap is disabled until you lock the chest.
									}

									if ( !toolBroken )
									{
										from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 1005639 ) ); // Trap is disabled until you lock the chest.	
									}
								}
								else
								{
									from.EndAction( typeof( CraftSystem ) );

									from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 1044043 ) ); // You failed to create the item, and some of your materials are lost.
								}
							}
							else
							{
								from.EndAction( typeof( CraftSystem ) );

								from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 1044153 ) ); // You don't have the required skills to attempt this item.
							}
						}
						else
						{
							from.EndAction( typeof( CraftSystem ) );

							from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 1044253 ) ); // You don't have the components needed to make that.
						}
					}
				}
				else
				{
					from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 1005638 ) ); // You can only trap lockable chests.
				}

				m_Trap.Delete();
			}

			protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
			{
				from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 1005638 ) ); // You can only trap lockable chests.
			}
		}
	}
}