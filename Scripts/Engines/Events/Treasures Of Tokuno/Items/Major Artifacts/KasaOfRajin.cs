using System;
using Server;

namespace Server.Items
{
	public class KasaOfRajin : Kasa
	{
		public override int LabelNumber { get { return 1070969; } } // Kasa of the Raj-in

		public override int InitMinHits { get { return 255; } }

		public override int InitMaxHits { get { return 255; } }

		public override int BasePhysicalResistance { get { return 12; } }

		public override int BaseFireResistance { get { return 17; } }

		public override int BaseColdResistance { get { return 21; } }

		public override int BasePoisonResistance { get { return 17; } }

		public override int BaseEnergyResistance { get { return 17; } }

		[Constructable]
		public KasaOfRajin()
		{
			Attributes.SpellDamage = 12;
		}

		public KasaOfRajin( Serial serial ) : base( serial )
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