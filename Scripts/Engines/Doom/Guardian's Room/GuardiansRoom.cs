using System;
using Server;

namespace Server.Regions
{
	public class DoomDarkGuardiansRoom : DungeonRegion
	{
		public static void Initialize()
		{
			Region.AddRegion( new DoomDarkGuardiansRoom( "Doom Dark Guardians Room" ) );
		}

		public DoomDarkGuardiansRoom( string name ) : base( name, Map.Malas )
		{
		}

		public override void OnExit( Mobile m )
		{
			if ( m.Player && !m.Alive )
			{
				Rectangle2D rect = new Rectangle2D( 342, 168, 16, 16 );

				int x = Utility.Random( rect.X, rect.Width );
				int y = Utility.Random( rect.Y, rect.Height );

				if ( x >= 345 && x <= 352 && y >= 173 && y <= 179 )
				{
					x = 353;
					y = 172;
				}

				m.MoveToWorld( new Point3D( x, y, -1 ), Map.Malas );

				if ( m.Corpse != null )
				{
					Region region = Region.Find( m.Corpse.Location, Map.Malas );

					if ( region.Name == "Doom Dark Guardians Room" )
					{
						m.Corpse.MoveToWorld( new Point3D( x, y, -1 ), Map.Malas );
					}
				}
			}
		}
	}
}