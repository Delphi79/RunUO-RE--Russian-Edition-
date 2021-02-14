using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class SoulStoneFragment : SoulStone, IUsesRemaining
	{
		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set
			{
				m_UsesRemaining = value;
				InvalidateProperties();
			}
		}

		public bool ShowUsesRemaining
		{
			get { return true; }
			set
			{
			}
		}

		[Constructable]
		public SoulStoneFragment( int itemid, string account ) : base( account )
		{
			ItemID = itemid;

			Light = LightType.Circle300;

			Weight = 1.0;

			Name = "A Soulstone Fragment";

			m_UsesRemaining = 5;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public SoulStoneFragment( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}