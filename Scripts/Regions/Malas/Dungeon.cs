using System;
using Server;

namespace Server.Regions
{
	public class MalasDungeon : DungeonRegion
	{
		public static void Initialize()
		{
			Region.AddRegion( new MalasDungeon( "Doom" ) );
			Region.AddRegion( new MalasDungeon( "Doom Gauntlet" ) );
			Region.AddRegion( new MalasDungeon( "Fan Dancer Dojo" ) );
			Region.AddRegion( new MalasDungeon( "Yomotsu Mines" ) );
		}

		public MalasDungeon( string name ) : base( name, Map.Malas )
		{
		}
	}
}