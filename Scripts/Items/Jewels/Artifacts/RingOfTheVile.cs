using System;
using Server;

namespace Server.Items
{
	public class RingOfTheVile : GoldRing
	{
		public override int LabelNumber { get { return 1061102; } } // Ring of the Vile
		public override int ArtifactRarity { get { return 11; } }

		[Constructable]
		public RingOfTheVile()
		{
			Hue = 0x4F7;
			Attributes.BonusDex = 8;
			Attributes.RegenStam = 6;
			Attributes.AttackChance = 15;
			Resistances.Poison = 20;
		}

		public RingOfTheVile( Serial serial ) : base( serial )
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

			if ( Hue == 0x4F4 )
			{
				Hue = 0x4F7;
			}
		}
	}
}