using System;
using Server;

namespace Server.Items
{
	public class BlackLotusHood : LeatherNinjaHood
	{
		public override int LabelNumber { get { return 1070919; } } // Black Lotus Hood

		public override int InitMinHits { get { return 255; } }

		public override int InitMaxHits { get { return 255; } }

		public override int BasePhysicalResistance { get { return 0; } }

		public override int BaseFireResistance { get { return 11; } }

		public override int BaseColdResistance { get { return 15; } }

		public override int BasePoisonResistance { get { return 11; } }

		public override int BaseEnergyResistance { get { return 11; } }

		[Constructable]
		public BlackLotusHood()
		{
			ItemID = 0x278F;
			ArmorAttributes.SelfRepair = 5;
			Attributes.AttackChance = 6;
			Attributes.LowerManaCost = 6;
		}

		public BlackLotusHood( Serial serial ) : base( serial )
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

			if ( Hue != 0 )
			{
				Hue = 0;
			}

			if ( ItemID != 0x278F )
			{
				ItemID = 0x278F;
			}
		}
	}
}