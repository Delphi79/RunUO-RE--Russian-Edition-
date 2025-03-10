using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class DeadWood : BaseReagent, ICommodity
	{
		string ICommodity.Description { get { return String.Format( "{0} dead wood", Amount ); } }

		[Constructable]
		public DeadWood() : this( 1 )
		{
		}

		[Constructable]
		public DeadWood( int amount ) : base( 0xF90, amount )
		{
		}

		public DeadWood( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new DeadWood( amount ), amount );
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