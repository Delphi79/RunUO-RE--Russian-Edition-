using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class ExplosionTrap : BaseCraftableTrap
	{
		[Constructable]
		public ExplosionTrap() : base( 0x370C )
		{
		}

		public ExplosionTrap( Serial serial ) : base( serial )
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