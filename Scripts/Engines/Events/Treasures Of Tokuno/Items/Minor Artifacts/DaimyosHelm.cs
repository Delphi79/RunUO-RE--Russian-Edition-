using System;
using Server;

namespace Server.Items
{
	public class DaimyosHelm : PlateBattleKabuto
	{
		public override int LabelNumber { get { return 1070920; } } // Daimyo's Helm

		public override int InitMinHits { get { return 255; } }

		public override int InitMaxHits { get { return 255; } }

		public override int BasePhysicalResistance { get { return 6; } }

		public override int BaseFireResistance { get { return 2; } }

		public override int BaseColdResistance { get { return 10; } }

		public override int BasePoisonResistance { get { return 2; } }

		public override int BaseEnergyResistance { get { return 3; } }

		[Constructable]
		public DaimyosHelm()
		{
			ArmorAttributes.SelfRepair = 3;
			Attributes.WeaponSpeed = 10;
			ArmorAttributes.LowerStatReq = 100;
			ArmorAttributes.MageArmor = 1;
		}

		public DaimyosHelm( Serial serial ) : base( serial )
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