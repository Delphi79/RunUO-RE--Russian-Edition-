using System;
using Server;

namespace Server.Regions
{
	public class DoomLampRoom : DungeonRegion
	{
		public static void Initialize()
		{
			Region.AddRegion( new DoomLampRoom( "Doom Lamp Room" ) );
		}

		public DoomLampRoom( string name ) : base( name, Map.Malas )
		{
		}

		public override bool CanUseStuckMenu( Mobile m )
		{
			return false;
		}

		public override bool OnSkillUse( Mobile from, int Skill )
		{
			// at OSI for logout from Lamp Room, account may be banned
			if ( Skill == 10 ) // camping
			{
				return false;
			}

			return true;
		}
	}
}