using System;
using Server;

namespace Server.Items
{
	public class RuneBeetleCarapace : PlateDo
	{
		public override int LabelNumber { get { return 1070968; } } // Rune Beetle Carapace

		public override int InitMinHits { get { return 255; } }

		public override int InitMaxHits { get { return 255; } }

		public override int BasePhysicalResistance { get { return 5; } }

		public override int BaseFireResistance { get { return 3; } }

		public override int BaseColdResistance { get { return 14; } }

		public override int BasePoisonResistance { get { return 3; } }

		public override int BaseEnergyResistance { get { return 14; } }

		[Constructable]
		public RuneBeetleCarapace()
		{
			Attributes.BonusMana = 10;
			Attributes.RegenMana = 3;
			Attributes.LowerManaCost = 15;
			ArmorAttributes.LowerStatReq = 100;
			ArmorAttributes.MageArmor = 1;
		}

		public RuneBeetleCarapace( Serial serial ) : base( serial )
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