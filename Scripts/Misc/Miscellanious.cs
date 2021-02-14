using System;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Engines.CannedEvil;

namespace Server.Misc
{
	public class Miscellanious
	{
		public static void Configure()
		{
			Core.Logging = true; // if you want to stop catch unhandled packets, set it in false

			Core.AllowLabelObjects = false; // if you want to allow labeling items or mobiles created or modified by GM, set it in true
		}

		public static bool ValidateLabeling( object o )
		{
			// add here all types of items or mobiles that shouldn't labeling
			// Players can be transferred from other servers, so i disabled "cheat" labeling at it
			if ( o is BaseVendor || o is PlayerMobile || o is Teleporter || o is Spawner || o is Static || o is ChampionSpawn )
			{
				return false;
			}

			return true;
		}
	}
}