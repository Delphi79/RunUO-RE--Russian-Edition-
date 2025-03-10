using System;
using Server.Network;

namespace Server.Misc
{
	public class LoginStats
	{
		public static void Initialize()
		{
			// Register our event handler
			EventSink.Login += new LoginEventHandler( EventSink_Login );
		}

		private static void EventSink_Login( LoginEventArgs args )
		{
			int userCount = NetState.Instances.Count;
			int itemCount = World.Items.Count;
			int mobileCount = World.Mobiles.Count;

			Mobile m = args.Mobile;

			m.SendMessage( "Welcome, {0}! There {1} currently {2} user{3} online, with {4} item{5} and {6} mobile{7} in the world.", args.Mobile.Name, userCount == 1 ? "is" : "are", userCount, userCount == 1 ? "" : "s", itemCount, itemCount == 1 ? "" : "s", mobileCount, mobileCount == 1 ? "" : "s" );

			m.SendMessage( String.Format( "Server Version: RunUO-RE {0}.{1}.{2}.{3}", Core.Core_Version.Major, Core.Core_Version.Minor, Core.Core_Version.Revision, Core.Core_Version.Build ) );
		}
	}
}