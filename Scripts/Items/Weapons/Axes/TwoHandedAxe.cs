using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x1443, 0x1442 )]
	public class TwoHandedAxe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility { get { return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility { get { return WeaponAbility.ShadowStrike; } }

		public override int AosStrengthReq { get { return 40; } }
		public override int AosMinDamage { get { return 16; } }
		public override int AosMaxDamage { get { return 17; } }
		public override int AosSpeed { get { return 31; } }

		public override int OldStrengthReq { get { return 35; } }
		public override int OldMinDamage { get { return 5; } }
		public override int OldMaxDamage { get { return 39; } }
		public override int OldSpeed { get { return 30; } }

		public override int InitMinHits { get { return 31; } }
		public override int InitMaxHits { get { return 90; } }

		[Constructable]
		public TwoHandedAxe() : base( 0x1443 )
		{
			Weight = 8.0;
		}

		public TwoHandedAxe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}