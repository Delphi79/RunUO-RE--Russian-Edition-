using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SpidersSilk : BaseReagent, ICommodity
	{
		string ICommodity.Description { get { return String.Format( "{0} spiders' silk", Amount ); } }

		[Constructable]
		public SpidersSilk() : this( 1 )
		{
		}

		[Constructable]
		public SpidersSilk( int amount ) : base( 0xF8D, amount )
		{
		}

		public SpidersSilk( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new SpidersSilk( amount ), amount );
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