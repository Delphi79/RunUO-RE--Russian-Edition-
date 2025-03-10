using System;

namespace Server.Items
{
	public class Flowers : StealableArtifact
	{
		public override int ArtifactRarity { get { return 7; } }

		[Constructable]
		public Flowers() : base( 0x284A )
		{
		}

		public Flowers( Serial serial ) : base( serial )
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
	}
}