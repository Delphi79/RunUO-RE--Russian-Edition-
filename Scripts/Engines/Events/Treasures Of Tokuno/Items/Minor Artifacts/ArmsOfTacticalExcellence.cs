using System;
using Server;

namespace Server.Items
{
	public class ArmsOfTacticalExcellence : LeatherHiroSode
	{
		public override int LabelNumber { get { return 1070921; } } // Arms of Tactical Excellence

		public override int InitMinHits { get { return 255; } }

		public override int InitMaxHits { get { return 255; } }

		public override int BasePhysicalResistance { get { return 2; } }

		public override int BaseFireResistance { get { return 9; } }

		public override int BaseColdResistance { get { return 13; } }

		public override int BasePoisonResistance { get { return 8; } }

		public override int BaseEnergyResistance { get { return 3; } }

		[Constructable]
		public ArmsOfTacticalExcellence()
		{
			SkillBonuses.SetValues( 0, SkillName.Tactics, 12.0 );
			Attributes.BonusDex = 5;
		}

		public ArmsOfTacticalExcellence( Serial serial ) : base( serial )
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