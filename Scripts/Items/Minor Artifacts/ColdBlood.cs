using System;
using Server;

namespace Server.Items
{
	public class ColdBlood : Cleaver
	{
		public override int LabelNumber { get { return 1070818; } } // Cold Blood

		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		[Constructable]
		public ColdBlood()
		{
			Hue = 0x4F2;

			Attributes.WeaponSpeed = 40;

			Attributes.BonusHits = 6;
			Attributes.BonusStam = 6;
			Attributes.BonusMana = 6;
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy )
		{
			cold = 100;

			fire = phys = pois = nrgy = 0;
		}

		public ColdBlood( Serial serial ) : base( serial )
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