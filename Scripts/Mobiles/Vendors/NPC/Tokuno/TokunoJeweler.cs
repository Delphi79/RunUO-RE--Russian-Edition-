using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class TokunoJeweler : Jeweler
	{
		[Constructable]
		public TokunoJeweler()
		{
		}

		public override VendorShoeType ShoeType { get { return VendorShoeType.SamuraiTabi; } }

		public override void InitOutfit()
		{
			InitFacial();
			AddItem( new Obi() );

			if ( Female )
			{
				AddItem( new FemaleKimono( Utility.RandomBlueHue() ) );
			}
			else
			{
				AddItem( new MaleKimono( Utility.RandomBlueHue() ) );
			}
		}

		public TokunoJeweler( Serial serial ) : base( serial )
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