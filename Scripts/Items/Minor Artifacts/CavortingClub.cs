using System;
using Server;

namespace Server.Items
{
	public class CavortingClub : Club
	{
		public override int LabelNumber { get { return 1063472; } } // Cavorting Club

		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		[Constructable]
		public CavortingClub()
		{
			Hue = 0x593;
			WeaponAttributes.SelfRepair = 3;
			Attributes.WeaponSpeed = 25;
			Attributes.WeaponDamage = 35;
			WeaponAttributes.ResistFireBonus = 8;
			WeaponAttributes.ResistColdBonus = 8;
			WeaponAttributes.ResistPoisonBonus = 8;
			WeaponAttributes.ResistEnergyBonus = 8;
		}

		public CavortingClub( Serial serial ) : base( serial )
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