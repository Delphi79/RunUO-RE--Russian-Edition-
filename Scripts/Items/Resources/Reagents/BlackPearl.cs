using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BlackPearl : BaseReagent, ICommodity
	{
		string ICommodity.Description { get { return String.Format( "{0} black pearl", Amount ); } }

		[Constructable]
		public BlackPearl() : this( 1 )
		{
		}

		[Constructable]
		public BlackPearl( int amount ) : base( 0xF7A, amount )
		{
		}

		public BlackPearl( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new BlackPearl( amount ), amount );
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