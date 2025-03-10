using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class PigIron : BaseReagent, ICommodity
	{
		string ICommodity.Description { get { return String.Format( "{0} pig iron", Amount ); } }

		[Constructable]
		public PigIron() : this( 1 )
		{
		}

		[Constructable]
		public PigIron( int amount ) : base( 0xF8A, amount )
		{
		}

		public PigIron( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new PigIron( amount ), amount );
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