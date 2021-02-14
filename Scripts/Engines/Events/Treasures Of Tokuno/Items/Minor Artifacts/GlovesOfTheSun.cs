using System;
using Server;

namespace Server.Items
{
	public class GlovesOfTheSun : LeatherNinjaMitts
	{
		public override int LabelNumber { get { return 1070924; } } // Gloves of the Sun

		public override int InitMinHits { get { return 255; } }

		public override int InitMaxHits { get { return 255; } }

		public override int BasePhysicalResistance { get { return 2; } }

		public override int BaseFireResistance { get { return 24; } }

		public override int BaseColdResistance { get { return 3; } }

		public override int BasePoisonResistance { get { return 3; } }

		public override int BaseEnergyResistance { get { return 3; } }

		[Constructable]
		public GlovesOfTheSun()
		{
			Attributes.RegenHits = 2;
			Attributes.NightSight = 1;
			Attributes.LowerManaCost = 5;
			Attributes.LowerRegCost = 18;
		}

		public GlovesOfTheSun( Serial serial ) : base( serial )
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

			if ( Attributes.LowerRegCost == 6 )
			{
				Attributes.LowerRegCost = 18;
			}
		}
	}
}