using System;
using Server.Items;

namespace Server.Items
{
	public class ScribeStone : Item
	{
		[Constructable]
		public ScribeStone() : base( 0xED4 )
		{
			Movable = false;
			Hue = 0x105;
			Name = "a Scribe Supply Stone";
		}

		public override void OnDoubleClick( Mobile from )
		{
			ScribeBag scribeBag = new ScribeBag();

			if ( !from.AddToBackpack( scribeBag ) )
			{
				scribeBag.Delete();
			}
		}

		public ScribeStone( Serial serial ) : base( serial )
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