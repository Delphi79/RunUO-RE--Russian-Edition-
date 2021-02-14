using System;
using Server.Items;
using Server.ContextMenus;
using Server.Targeting;
using System.Collections;
using Server.Engines.Craft;

namespace Server.Items
{
	public enum BeltQuality
	{
		Low,
		Regular,
		Exceptional
	}

	[FlipableAttribute( 0x2790, 0x27DB )]
	public class LeatherNinjaBelt : Item, IUsesRemaining, ICraftable, IScissorable
	{
		private int m_UsesRemaining;
		private BeltQuality m_Quality;
		private Mobile m_Crafter;
		private Poison m_Poison;
		private int m_PoisonCharges;
		private string m_Crafter_Name;

		private int m_StrReq = 10; // at this moment all clothings have 10 str req. So don't serialize now; Is it correct OSI?

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrRequirement
		{
			get { return m_StrReq; }
			set
			{
				m_StrReq = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set
			{
				m_UsesRemaining = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public BeltQuality Quality { get { return m_Quality; } set { m_Quality = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get { return m_Crafter; }
			set
			{
				m_Crafter = value;
				CheckName();
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonCharges
		{
			get { return m_PoisonCharges; }
			set
			{
				m_PoisonCharges = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Poison Poison
		{
			get { return m_Poison; }
			set
			{
				m_Poison = value;
				InvalidateProperties();
			}
		}

		public bool ShowUsesRemaining
		{
			get { return true; }
			set
			{
			}
		}

		[Constructable]
		public LeatherNinjaBelt() : base( 0x2790 )
		{
			Layer = Layer.Waist;

			Weight = 1.0;

			m_UsesRemaining = 0;
		}

		public LeatherNinjaBelt( Serial serial ) : base( serial )
		{
		}

		public void CheckName()
		{
			string name = m_Crafter != null ? m_Crafter.Name : "";

			if ( m_Crafter != null && m_Crafter.Fame >= 10000 )
			{
				string title = m_Crafter.Female ? "Lady" : "Lord";

				name = title + " " + name;
			}

			if ( name != "" )
			{
				m_Crafter_Name = name;
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Crafter != null && m_Crafter_Name != null && m_Crafter_Name != "" )
			{
				list.Add( 1050043, m_Crafter_Name ); // crafted by ~1_NAME~
			}

			if ( m_Quality == BeltQuality.Exceptional )
			{
				list.Add( 1060636 ); // exceptional
			}

			if ( m_Poison != null && m_PoisonCharges > 0 )
			{
				list.Add( 1062412 + m_Poison.Level, m_PoisonCharges.ToString() );
			}

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~

			int strReq = StrRequirement;

			if ( strReq > 0 )
			{
				list.Add( 1061170, strReq.ToString() ); // strength requirement ~1_val~
			}
		}

		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 502437 ); // Items you wish to cut must be in your backpack.
				return false;
			}

			CraftSystem system = DefTailoring.CraftSystem;

			CraftItem item = system.CraftItems.SearchFor( GetType() );

			if ( item != null && item.Ressources.Count == 1 && item.Ressources.GetAt( 0 ).Amount >= 2 )
			{
				try
				{
					Item res = (Item) Activator.CreateInstance( CraftResources.GetInfo( CraftResource.RegularLeather ).ResourceTypes[ 0 ] );

					ScissorHelper( from, res, (item.Ressources.GetAt( 0 ).Amount/2) );
					return true;
				} 
				catch
				{
				}
			}

			from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
			return false;
		}

		public override bool CanEquip( Mobile from )
		{
			if ( from.Str < StrRequirement )
			{
				from.SendLocalizedMessage( 500213 ); // You are not strong enough to equip that.
				return false;
			}
			else
			{
				return base.CanEquip( from );
			}
		}

		public override bool OnEquip( Mobile from )
		{
			from.SendLocalizedMessage( 1070785 ); // Double click this item each time you wish to throw a shuriken.

			return true;
		}

		public override void OnDoubleClick( Mobile from )
		{
			Item item = from.FindItemOnLayer( Layer.OneHanded );

			Item item2 = from.FindItemOnLayer( Layer.TwoHanded );

			if ( item != null || item2 != null )
			{
				from.SendLocalizedMessage( 1063299 ); // You must have a free hand to throw shuriken.

				return;
			}

			if ( UsesRemaining > 0 )
			{
				InternalTarget t = new InternalTarget( this );
				from.Target = t;
			}
			else
			{
				from.SendLocalizedMessage( 1063297 ); // You have no shuriken in your ninja belt!
			}
		}

		private class InternalTarget : Target
		{
			private LeatherNinjaBelt m_belt;

			public InternalTarget( LeatherNinjaBelt belt ) : base( 10, false, TargetFlags.Harmful )
			{
				m_belt = belt;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_belt.Deleted )
				{
					return;
				}
				else if ( targeted is Mobile )
				{
					Mobile m = (Mobile) targeted;

					double dist = from.GetDistanceToSqrt( m.Location );

					if ( m.Map != from.Map || dist > 11 )
					{
						from.SendLocalizedMessage( 500446 ); // That is too far away.
						return;
					}

					if ( m != from && from.HarmfulCheck( m ) )
					{
						Direction to = from.GetDirectionTo( m );

						from.Direction = to;

						from.RevealingAction();

						from.Animate( from.Mounted ? 26 : 9, 7, 1, true, false, 0 );

						if ( Fukiya.CheckHitChance( from, m ) )
						{
							from.MovingEffect( m, 0x27AC, 7, 1, false, false, 0x23A, 0 );

							AOS.Damage( m, from, Utility.Random( 3, 5 ), 100, 0, 0, 0, 0 );

							if ( m_belt.Poison != null && m_belt.PoisonCharges > 0 )
							{
								--m_belt.PoisonCharges;

								m.ApplyPoison( from, m_belt.Poison );
							}
						}
						else
						{
							from.MovingEffect( new Shuriken(), 0x27AC, 7, 1, false, false, 0x23A, 0 );

							from.SendMessage( "You miss." );
						}

						m_belt.UsesRemaining--;
					}
				}
			}
		}

		public override void GetContextMenuEntries( Mobile from, ArrayList list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from == this.Parent || IsChildOf( from.Backpack ) )
			{
				list.Add( new LoadEntry( from, this ) );

				list.Add( new UnloadEntry( from, this, (m_UsesRemaining > 0) ) );
			}
		}

		private class LoadEntry : ContextMenuEntry
		{
			private Mobile mobile;

			private LeatherNinjaBelt belt;

			public LoadEntry( Mobile from, LeatherNinjaBelt m_belt ) : base( 6222, 3 )
			{
				mobile = from;

				belt = m_belt;
			}

			public override void OnClick()
			{
				mobile.Target = new InternalTarget( belt );
			}

			private class InternalTarget : Target
			{
				private LeatherNinjaBelt m_belt;

				public InternalTarget( LeatherNinjaBelt belt ) : base( 1, false, TargetFlags.None )
				{
					m_belt = belt;
				}

				protected override void OnTarget( Mobile from, object targeted )
				{
					if ( targeted is Shuriken )
					{
						Shuriken shuriken = targeted as Shuriken;

						if ( shuriken.Poison != null && shuriken.PoisonCharges > 0 )
						{
							m_belt.Poison = shuriken.Poison;

							m_belt.PoisonCharges = shuriken.PoisonCharges;
						}

						if ( m_belt.UsesRemaining < 10 )
						{
							if ( shuriken.UsesRemaining + m_belt.UsesRemaining >= 10 )
							{
								int need = 10 - m_belt.UsesRemaining;

								m_belt.UsesRemaining += need;

								shuriken.UsesRemaining -= need;

								if ( shuriken.UsesRemaining < 1 )
								{
									shuriken.Delete();
								}
							}
							else
							{
								m_belt.UsesRemaining += shuriken.UsesRemaining;

								shuriken.Delete();
							}
						}
						else
						{
							from.SendLocalizedMessage( 1063302 ); // You cannot add any more shuriken.
						}
					}
					else
					{
						from.SendLocalizedMessage( 1063301 ); // You can only place shuriken in a ninja belt.
					}
				}
			}
		}

		private class UnloadEntry : ContextMenuEntry
		{
			private Mobile mobile;

			private LeatherNinjaBelt belt;

			public UnloadEntry( Mobile from, LeatherNinjaBelt m_belt, bool enabled ) : base( 6223, 3 )
			{
				mobile = from;

				belt = m_belt;

				if ( !enabled )
				{
					Flags |= Network.CMEFlags.Disabled;
				}
			}

			public override void OnClick()
			{
				Shuriken shuriken = new Shuriken( belt.UsesRemaining );

				belt.UsesRemaining = 0;

				mobile.AddToBackpack( shuriken );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );

			writer.Write( (string) m_Crafter_Name );

			Poison.Serialize( m_Poison, writer );

			writer.Write( m_PoisonCharges );

			writer.Write( (int) m_UsesRemaining );

			writer.Write( m_Crafter );

			writer.WriteEncodedInt( (int) m_Quality );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
					{
						m_Crafter_Name = reader.ReadString();

						goto case 0;
					}
				case 0:
					{
						m_Poison = Poison.Deserialize( reader );

						m_PoisonCharges = reader.ReadInt();

						m_UsesRemaining = reader.ReadInt();

						m_Crafter = reader.ReadMobile();

						m_Quality = (BeltQuality) reader.ReadEncodedInt();

						break;
					}
			}

			if ( UsesRemaining > 10 )
			{
				UsesRemaining = 10;
			}
		}

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (BeltQuality) quality;

			if ( makersMark )
			{
				Crafter = from;
			}

			return quality;
		}
	}
}