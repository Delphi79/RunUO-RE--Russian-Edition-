using System;
using Server;

namespace Server.Items
{
	public class InquisitorsResolutionA : StealablePlateGlovesArtifact
	{
		public override int LabelNumber { get { return 1060206; } } // The Inquisitor's Resolution
		public override int ArtifactRarity { get { return 10; } }

		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		[Constructable]
		public InquisitorsResolutionA()
		{
			Hue = 0x4F2;
			Attributes.CastRecovery = 3;
			Attributes.LowerManaCost = 8;
			ArmorAttributes.MageArmor = 1;
			ColdBonus = 20;
			EnergyBonus = 15;
		}

		public InquisitorsResolutionA( Serial serial ) : base( serial )
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