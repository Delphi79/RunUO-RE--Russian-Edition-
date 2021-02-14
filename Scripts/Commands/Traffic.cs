using System;
using Server;
using Server.Targeting;
using Server.Gumps;
using Server.Scripts.Gumps;

namespace Server.Scripts.Commands
{
	public class Traffic
	{
		public static void Initialize()
		{
			Register();
		}

		public static void Register()
		{
			Server.Commands.Register( "Traffic", AccessLevel.Player, new CommandEventHandler( Traffic_OnCommand ) );
		}

		[Usage( "Traffic" )]
		[Description( "Showing incoming and outgoing traffic for your session" )]
		private static void Traffic_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "Incoming traffic: " + AdminGump.FormatByteAmount( e.Mobile.NetState.Incoming ) );
			e.Mobile.SendMessage( "Outgoing traffic: " + AdminGump.FormatByteAmount( e.Mobile.NetState.Outgoing ) );
		}
	}
}