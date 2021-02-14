using System;
using Server;
using Server.Mobiles;

namespace Server.Regions
{
	public class StartPoint : Region
	{
		public static void Initialize()
		{
			Region.AddRegion( new StartPoint( "Samurai DE" ) );
			Region.AddRegion( new StartPoint( "Ninja DE" ) );
			Region.AddRegion( new StartPoint( "Mongbat Cave" ) );
		}

		public StartPoint( string name ) : base( "start point", name, Map.Malas )
		{
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

		public override void OnEnter( Mobile m )
		{
			//base.OnEnter( m ); // You have entered the dungeon {0}

			m.LightLevel = 50;
		}

		public override void OnExit( Mobile m )
		{
			//base.OnExit( m );
		}
	}
}