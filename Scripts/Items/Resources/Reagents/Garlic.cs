using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class Garlic : BaseReagent, ICommodity
	{
		string ICommodity.Description { get { return String.Format( "{0} garlic", Amount ); } }

		[Constructable]
		public Garlic() : this( 1 )
		{
		}

		[Constructable]
		public Garlic( int amount ) : base( 0xF84, amount )
		{
		}

		public Garlic( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Garlic( amount ), amount );
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