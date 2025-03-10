using System;
using Server;

namespace Server.Items
{
	public class HolyKnightsBreastplate : PlateChest
	{
		public override int LabelNumber { get { return 1061097; } } // Holy Knight's Breastplate
		public override int ArtifactRarity { get { return 11; } }

		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		[Constructable]
		public HolyKnightsBreastplate()
		{
			Hue = 0x47E;
			Attributes.BonusHits = 10;
			Attributes.ReflectPhysical = 15;
			PhysicalBonus = 30;
		}

		public HolyKnightsBreastplate( Serial serial ) : base( serial )
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