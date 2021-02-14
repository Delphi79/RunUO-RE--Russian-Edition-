using System;
using Server;
using Server.Misc;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class GuardianTreasureChest : BaseTreasureChest
	{
		public int[] ItemIDs = new int[]
		{
			0xE41, 0xE43, 0x9AB
		};

		public void Fill()
		{
			Map map = this.Map;

			Item item = (map == Map.Tokuno) ? Loot.RandomSEArmorOrShieldOrWeaponOrJewelry() : Loot.RandomArmorOrShieldOrWeaponOrJewelry();

			if ( item is BaseArmor )
			{
				BaseRunicTool.ApplyAttributesTo( (BaseArmor) item, 1, 10, 30 );
			}
			else if ( item is BaseWeapon )
			{
				BaseRunicTool.ApplyAttributesTo( (BaseWeapon) item, 1, 10, 30 );
			}
			else if ( item is BaseJewel )
			{
				BaseRunicTool.ApplyAttributesTo( (BaseJewel) item, 1, 10, 30 );
			}

			AddItem( item );
		}

		[Constructable]
		public GuardianTreasureChest( int itemID ) : base( itemID )
		{
			for ( int i = 0; i < 2; i++ )
			{
				Fill();
			}

			InternalTimer timer = new InternalTimer( this );

			timer.Start();
		}

		public GuardianTreasureChest( Serial serial ) : base( serial )
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

			InternalTimer timer = new InternalTimer( this );

			timer.Start();
		}

		public class InternalTimer : Timer
		{
			public GuardianTreasureChest m_Chest;

			public InternalTimer( GuardianTreasureChest chest ) : base( TimeSpan.FromMinutes( Utility.Random( 1, 2 ) ) )
			{
				m_Chest = chest;
			}

			protected override void OnTick()
			{
				if ( m_Chest != null )
				{
					m_Chest.Delete();
				}

				Rectangle2D rect = new Rectangle2D( 356, 6, 19, 19 );

				int x = Utility.Random( rect.X, rect.Width );
				int y = Utility.Random( rect.Y, rect.Height );
				int itemID = Utility.RandomList( m_Chest.ItemIDs );

				GuardianTreasureChest chest = new GuardianTreasureChest( itemID );
				chest.MoveToWorld( new Point3D( x, y, -1 ), Map.Malas );
			}
		}
	}
}