using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class SnowyTree : Item
	{
		[Constructable]
		public SnowyTree() : base( 0x2377 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public SnowyTree( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			LabelTo( from, 1070880 ); // Winter 2004
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1070880 ); // Winter 2004
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