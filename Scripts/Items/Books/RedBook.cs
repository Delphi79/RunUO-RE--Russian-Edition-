using System;
using Server;

namespace Server.Items
{
	public class RedBook : BaseBook
	{
		public override double BookWeight { get { return 2.0; } }

		[Constructable]
		public RedBook() : base( 0xFF1 )
		{
		}

		[Constructable]
		public RedBook( int pageCount, bool writable ) : base( 0xFF1, pageCount, writable )
		{
		}

		[Constructable]
		public RedBook( string title, string author, int pageCount, bool writable ) : base( 0xFF1, title, author, pageCount, writable )
		{
		}

		public RedBook( Serial serial ) : base( serial )
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