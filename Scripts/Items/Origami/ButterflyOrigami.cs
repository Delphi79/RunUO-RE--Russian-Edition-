using System;
using Server;

namespace Server.Items
{
	public class ButterflyOrigami : Item
	{
		public override int LabelNumber { get { return 1030296; } } // a delicate origami butterfly

		[Constructable]
		public ButterflyOrigami() : base( 0x2838 )
		{
			LootType = LootType.Blessed;

			Weight = 1.0;
		}

		public ButterflyOrigami( Serial serial ) : base( serial )
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