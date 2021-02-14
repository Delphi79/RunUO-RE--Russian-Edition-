using System;
using Server;

namespace Server.Items
{
	public class AncientSamuraiDo : PlateDo
	{
		public override int LabelNumber { get { return 1070926; } } // Ancient Samurai Do

		public override int InitMinHits { get { return 255; } }

		public override int InitMaxHits { get { return 255; } }

		public override int BasePhysicalResistance { get { return 15; } }

		public override int BaseFireResistance { get { return 12; } }

		public override int BaseColdResistance { get { return 10; } }

		public override int BasePoisonResistance { get { return 11; } }

		public override int BaseEnergyResistance { get { return 8; } }

		[Constructable]
		public AncientSamuraiDo()
		{
			SkillBonuses.SetValues( 0, SkillName.Parry, 10.0 );
			ArmorAttributes.LowerStatReq = 100;
			ArmorAttributes.MageArmor = 1;
		}

		public AncientSamuraiDo( Serial serial ) : base( serial )
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