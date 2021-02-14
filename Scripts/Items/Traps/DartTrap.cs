using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class DartTrap : BaseCraftableTrap
	{
		[Constructable]
		public DartTrap() : base( 0x1BFC )
		{
		}

		public DartTrap( Serial serial ) : base( serial )
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