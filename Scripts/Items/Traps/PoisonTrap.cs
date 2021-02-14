using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class PoisonTrap : BaseCraftableTrap
	{
		[Constructable]
		public PoisonTrap() : base( 0x113E )
		{
		}

		public PoisonTrap( Serial serial ) : base( serial )
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