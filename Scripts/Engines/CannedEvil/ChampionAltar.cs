using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Engines.CannedEvil
{
	public class ChampionAltar : PentagramAddon
	{
		private ChampionSpawn m_Spawn;

		public ChampionAltar( ChampionSpawn spawn )
		{
			m_Spawn = spawn;

			Hue = 0x455;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_Spawn != null )
			{
				m_Spawn.Delete();
			}
		}

		public ChampionAltar( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Spawn );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
					{
						m_Spawn = reader.ReadItem() as ChampionSpawn;

						if ( m_Spawn == null )
						{
							Delete();
						}

						break;
					}
			}

			if ( Hue == 0 )
			{
				Hue = 0x455;
			}
		}
	}
}