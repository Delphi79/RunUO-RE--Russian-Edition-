using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class MandrakeRoot : BaseReagent, ICommodity
	{
		string ICommodity.Description { get { return String.Format( "{0} mandrake root", Amount ); } }

		[Constructable]
		public MandrakeRoot() : this( 1 )
		{
		}

		[Constructable]
		public MandrakeRoot( int amount ) : base( 0xF86, amount )
		{
		}

		public MandrakeRoot( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new MandrakeRoot( amount ), amount );
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