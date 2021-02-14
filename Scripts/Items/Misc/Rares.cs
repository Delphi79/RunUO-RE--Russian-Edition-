using System;

namespace Server.Items
{
	public class Rope : Item
	{
		[Constructable]
		public Rope() : this( 1 )
		{
		}

		[Constructable]
		public Rope( int amount ) : base( 0x14F8 )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Rope( amount ), amount );
		}

		public Rope( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class IronWire : Item
	{
		[Constructable]
		public IronWire() : this( 1 )
		{
		}

		[Constructable]
		public IronWire( int amount ) : base( 0x1876 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new IronWire( amount ), amount );
		}

		public IronWire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 && Weight == 2.0 )
			{
				Weight = 5.0;
			}
		}
	}

	public class SilverWire : Item
	{
		[Constructable]
		public SilverWire() : this( 1 )
		{
		}

		[Constructable]
		public SilverWire( int amount ) : base( 0x1877 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new SilverWire( amount ), amount );
		}

		public SilverWire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 && Weight == 2.0 )
			{
				Weight = 5.0;
			}
		}
	}

	public class GoldWire : Item
	{
		[Constructable]
		public GoldWire() : this( 1 )
		{
		}

		[Constructable]
		public GoldWire( int amount ) : base( 0x1878 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new GoldWire( amount ), amount );
		}

		public GoldWire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 && Weight == 2.0 )
			{
				Weight = 5.0;
			}
		}
	}

	public class CopperWire : Item
	{
		[Constructable]
		public CopperWire() : this( 1 )
		{
		}

		[Constructable]
		public CopperWire( int amount ) : base( 0x1879 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new CopperWire( amount ), amount );
		}

		public CopperWire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 && Weight == 2.0 )
			{
				Weight = 5.0;
			}
		}
	}

	public class WhiteDriedFlowers : Item
	{
		[Constructable]
		public WhiteDriedFlowers() : this( 1 )
		{
		}

		[Constructable]
		public WhiteDriedFlowers( int amount ) : base( 0xC3C )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new WhiteDriedFlowers( amount ), amount );
		}

		public WhiteDriedFlowers( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class GreenDriedFlowers : Item
	{
		[Constructable]
		public GreenDriedFlowers() : this( 1 )
		{
		}

		[Constructable]
		public GreenDriedFlowers( int amount ) : base( 0xC3E )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new GreenDriedFlowers( amount ), amount );
		}

		public GreenDriedFlowers( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class DriedOnions : Item
	{
		[Constructable]
		public DriedOnions() : this( 1 )
		{
		}

		[Constructable]
		public DriedOnions( int amount ) : base( 0xC40 )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new DriedOnions( amount ), amount );
		}

		public DriedOnions( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class DriedHerbs : Item
	{
		[Constructable]
		public DriedHerbs() : this( 1 )
		{
		}

		[Constructable]
		public DriedHerbs( int amount ) : base( 0xC42 )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new DriedHerbs( amount ), amount );
		}

		public DriedHerbs( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class HorseShoes : Item
	{
		[Constructable]
		public HorseShoes() : base( 0xFB6 )
		{
			Weight = 3.0;
		}

		public HorseShoes( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class ForgedMetal : Item
	{
		[Constructable]
		public ForgedMetal() : base( 0xFB8 )
		{
			Weight = 5.0;
		}

		public ForgedMetal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Whip : Item
	{
		[Constructable]
		public Whip() : base( 0x166E )
		{
			Weight = 1.0;
		}

		public Whip( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class PaintsAndBrush : Item
	{
		[Constructable]
		public PaintsAndBrush() : base( 0xFC1 )
		{
			Weight = 1.0;
		}

		public PaintsAndBrush( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class PenAndInk : Item
	{
		[Constructable]
		public PenAndInk() : base( 0xFBF )
		{
			Weight = 1.0;
		}

		public PenAndInk( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}