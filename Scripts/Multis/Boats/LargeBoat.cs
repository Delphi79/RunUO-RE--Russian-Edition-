using System;
using Server;
using Server.Items;

namespace Server.Multis
{
	public class LargeBoat : BaseBoat
	{
		public override int NorthID { get { return 0x4010; } }
		public override int EastID { get { return 0x4011; } }
		public override int SouthID { get { return 0x4012; } }
		public override int WestID { get { return 0x4013; } }

		public override int HoldDistance { get { return 5; } }
		public override int TillerManDistance { get { return -5; } }

		public override Point2D StarboardOffset { get { return new Point2D( 2, -1 ); } }
		public override Point2D PortOffset { get { return new Point2D( -2, -1 ); } }

		public override Point3D MarkOffset { get { return new Point3D( 0, 0, 3 ); } }

		public override BaseDockedBoat DockedBoat { get { return new LargeDockedBoat( this ); } }

		[Constructable]
		public LargeBoat()
		{
		}

		public LargeBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
	}

	public class LargeBoatDeed : BaseBoatDeed
	{
		public override int LabelNumber { get { return 1041209; } } // large ship deed
		public override BaseBoat Boat { get { return new LargeBoat(); } }

		[Constructable]
		public LargeBoatDeed() : base( 0x4010, new Point3D( 0, -1, 0 ) )
		{
		}

		public LargeBoatDeed( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
	}

	public class LargeDockedBoat : BaseDockedBoat
	{
		public override BaseBoat Boat { get { return new LargeBoat(); } }

		public LargeDockedBoat( BaseBoat boat ) : base( 0x4010, new Point3D( 0, -1, 0 ), boat )
		{
		}

		public LargeDockedBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
	}
}