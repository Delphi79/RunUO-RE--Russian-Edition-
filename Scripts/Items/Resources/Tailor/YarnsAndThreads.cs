using System;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public abstract class BaseClothMaterial : Item, IDyable
	{
		public BaseClothMaterial( int itemID ) : this( itemID, 1 )
		{
		}

		public BaseClothMaterial( int itemID, int amount ) : base( itemID )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		public BaseClothMaterial( Serial serial ) : base( serial )
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

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
			{
				return false;
			}

			Hue = sender.DyedHue;

			return true;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 500366 ); // Select a loom to use that on.
				from.Target = new PickLoomTarget( this );
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

		private class PickLoomTarget : Target
		{
			private BaseClothMaterial m_Material;

			public PickLoomTarget( BaseClothMaterial material ) : base( 3, false, TargetFlags.None )
			{
				m_Material = material;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Material.Deleted )
				{
					return;
				}

				ILoom loom = targeted as ILoom;

				if ( loom == null && targeted is AddonComponent )
				{
					loom = ((AddonComponent) targeted).Addon as ILoom;
				}

				if ( loom != null )
				{
					if ( !m_Material.IsChildOf( from.Backpack ) )
					{
						from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
					}
					else if ( loom.Phase < 4 )
					{
						m_Material.Consume();

						if ( targeted is Item )
						{
							((Item) targeted).SendLocalizedMessageTo( from, 1010001 + loom.Phase++ );
						}
					}
					else
					{
						Item create = new BoltOfCloth();
						create.Hue = m_Material.Hue;

						m_Material.Consume();
						loom.Phase = 0;
						from.SendLocalizedMessage( 500368 ); // You create some cloth and put it in your backpack.
						from.AddToBackpack( create );
					}
				}
				else
				{
					from.SendLocalizedMessage( 500367 ); // Try using that on a loom.
				}
			}
		}
	}

	public class DarkYarn : BaseClothMaterial
	{
		[Constructable]
		public DarkYarn() : this( 1 )
		{
		}

		[Constructable]
		public DarkYarn( int amount ) : base( 0xE1D, amount )
		{
		}

		public DarkYarn( Serial serial ) : base( serial )
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

		public override Item Dupe( int amount )
		{
			return base.Dupe( new DarkYarn(), amount );
		}
	}

	public class LightYarn : BaseClothMaterial
	{
		[Constructable]
		public LightYarn() : this( 1 )
		{
		}

		[Constructable]
		public LightYarn( int amount ) : base( 0xE1E, amount )
		{
		}

		public LightYarn( Serial serial ) : base( serial )
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

		public override Item Dupe( int amount )
		{
			return base.Dupe( new LightYarn(), amount );
		}
	}

	public class LightYarnUnraveled : BaseClothMaterial
	{
		[Constructable]
		public LightYarnUnraveled() : this( 1 )
		{
		}

		[Constructable]
		public LightYarnUnraveled( int amount ) : base( 0xE1F, amount )
		{
		}

		public LightYarnUnraveled( Serial serial ) : base( serial )
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

		public override Item Dupe( int amount )
		{
			return base.Dupe( new LightYarnUnraveled(), amount );
		}
	}

	public class SpoolOfThread : BaseClothMaterial
	{
		[Constructable]
		public SpoolOfThread() : this( 1 )
		{
		}

		[Constructable]
		public SpoolOfThread( int amount ) : base( 0xFA0, amount )
		{
		}

		public SpoolOfThread( Serial serial ) : base( serial )
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

		public override Item Dupe( int amount )
		{
			return base.Dupe( new SpoolOfThread(), amount );
		}
	}
}