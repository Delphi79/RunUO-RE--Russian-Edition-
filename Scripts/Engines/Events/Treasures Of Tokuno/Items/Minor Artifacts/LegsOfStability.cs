using System;
using Server;

namespace Server.Items
{
	public class LegsOfStability : PlateSuneate
	{
		public override int LabelNumber { get { return 1070923; } } // Legs of Stability

		public override int InitMinHits { get { return 255; } }

		public override int InitMaxHits { get { return 255; } }

		public override int BasePhysicalResistance { get { return 20; } }

		public override int BaseFireResistance { get { return 3; } }

		public override int BaseColdResistance { get { return 2; } }

		public override int BasePoisonResistance { get { return 18; } }

		public override int BaseEnergyResistance { get { return 2; } }

		[Constructable]
		public LegsOfStability()
		{
			ArmorAttributes.SelfRepair = 3;
			Attributes.BonusStam = 5;
			ArmorAttributes.LowerStatReq = 100;
			ArmorAttributes.MageArmor = 1;
		}

		public LegsOfStability( Serial serial ) : base( serial )
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