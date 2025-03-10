using System;
using Server;

namespace Server.Items
{
	public class BlueBook : BaseBook
	{
		[Constructable]
		public BlueBook() : base( 0xFF2 )
		{
		}

		[Constructable]
		public BlueBook( int pageCount, bool writable ) : base( 0xFF2, pageCount, writable )
		{
		}

		[Constructable]
		public BlueBook( string title, string author, int pageCount, bool writable ) : base( 0xFF2, title, author, pageCount, writable )
		{
		}

		public BlueBook( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}
	}
}