using System;
using Server;

namespace Server.Items
{
	public class GwennosHarp : LapHarp
	{
		public override int LabelNumber { get { return 1063480; } } // Gwenno's Harp

		public override int InitMinUses { get { return 1600; } }
		public override int InitMaxUses { get { return 1600; } }

		[Constructable]
		public GwennosHarp()
		{
			Hue = 0x47E;
			Slayer = SlayerName.Repond;
			Slayer2 = SlayerName.ReptilianDeath;
		}

		public GwennosHarp( Serial serial ) : base( serial )
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