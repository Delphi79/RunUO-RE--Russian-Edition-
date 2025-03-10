using System;
using System.Collections;
using Server;
using Server.Network;

namespace Server.Engines.MyRunUO
{
	public class MyRunUOStatus
	{
		public static void Initialize()
		{
			if ( Config.Enabled )
			{
				Timer.DelayCall( TimeSpan.FromSeconds( 20.0 ), Config.StatusUpdateInterval, new TimerCallback( Begin ) );
				Commands.Register( "UpdateWebStatus", AccessLevel.Administrator, new CommandEventHandler( UpdateWebStatus_OnCommand ) );
			}
		}

		[Usage( "UpdateWebStatus" )]
		[Description( "Starts the process of updating the MyRunUO online status database." )]
		public static void UpdateWebStatus_OnCommand( CommandEventArgs e )
		{
			if ( m_Command == null || m_Command.HasCompleted )
			{
				Begin();
				e.Mobile.SendMessage( "Web status update process has been started." );
			}
			else
			{
				e.Mobile.SendMessage( "Web status database is already being updated." );
			}
		}

		private static DatabaseCommandQueue m_Command;

		public static void Begin()
		{
			if ( m_Command != null && !m_Command.HasCompleted )
			{
				return;
			}

			DateTime start = DateTime.Now;
			Console.WriteLine( "MyRunUO: Updating status database" );

			try
			{
				m_Command = new DatabaseCommandQueue( "MyRunUO: Status database updated in {0:F1} seconds", "MyRunUO Status Database Thread" );

				m_Command.Enqueue( "DELETE FROM myrunuo_status" );

				ArrayList online = NetState.Instances;

				for ( int i = 0; i < online.Count; ++i )
				{
					NetState ns = (NetState) online[ i ];
					Mobile mob = ns.Mobile;

					if ( mob != null )
					{
						m_Command.Enqueue( String.Format( "INSERT INTO myrunuo_status VALUES ({0}, '{1}', {2}, {3}, {4})", mob.Serial.Value.ToString(), mob.Location.ToString(), mob.Map.ToString(), mob.Karma.ToString(), mob.Fame.ToString() ) );
					}
					//m_Command.Enqueue( String.Format( "INSERT INTO myrunuo_status VALUES ({0})", mob.Serial.Value.ToString() ) );
				}
			} 
			catch ( Exception e )
			{
				Console.WriteLine( "MyRunUO: Error updating status database" );
				Console.WriteLine( e );
			}

			if ( m_Command != null )
			{
				m_Command.Enqueue( null );
			}
		}
	}
}