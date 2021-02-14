using System;
using Server;

namespace Server.Items
{
	public class AncientFarmersKasa : Kasa
	{
		public override int LabelNumber { get { return 1070922; } } // Ancient Farmer's Kasa

		public override int InitMinHits { get { return 255; } }

		public override int InitMaxHits { get { return 255; } }

		public override int BasePhysicalResistance { get { return 0; } }

		public override int BaseFireResistance { get { return 5; } }

		public override int BaseColdResistance { get { return 19; } }

		public override int BasePoisonResistance { get { return 5; } }

		public override int BaseEnergyResistance { get { return 5; } }

		[Constructable]
		public AncientFarmersKasa()
		{
			SkillBonuses.SetValues( 0, SkillName.AnimalLore, 5.0 );
			Attributes.BonusStr = 5;
			Attributes.BonusStam = 5;
			Attributes.RegenStam = 5;
		}

		public AncientFarmersKasa( Serial serial ) : base( serial )
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