using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Spells;

namespace Server.Regions
{
	public class TokunoTown : GuardedRegion
	{
		new public static void Initialize()
		{
			Region.AddRegion( new TokunoTown( "Zento" ) );
			Region.AddRegion( GuardedRegion.Disable( new TokunoTown( "Fan Dancer's Dojo" ) ) );
			Region.AddRegion( new GuardedRegion( "", "Moongates", Map.Tokuno, typeof( WarriorGuard ) ) );
		}

		public TokunoTown( string name ) : this( name, typeof( WarriorGuard ) )
		{
		}

		public TokunoTown( string name, Type guardType ) : base( "the town of", name, Map.Tokuno, guardType )
		{
		}
	}
}