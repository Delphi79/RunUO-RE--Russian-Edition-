using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;
using Server.Factions;

namespace Server
{
	public class SEGuildGump : MyGuildGump
	{
		public static void Initialize()
		{
			EventSink.GuildGumpRequest += new GuildGumpRequestHandler( EventSink_GuildGumpRequest );
		}

		private static void EventSink_GuildGumpRequest( GuildGumpRequestArgs e )
		{
			Mobile beholder = e.Mobile;

			if ( Core.SE )
			{
				Guild guild = beholder.Guild as Guild;

				if ( beholder.Map == beholder.Map && beholder.InRange( beholder, 12 ) )
				{
					if ( guild == null || guild.Disbanded )
					{
						beholder.CloseGump( typeof( JoinGuildGump ) );

						beholder.SendGump( new JoinGuildGump( beholder, guild ) );
					}
					else if ( guild.Accepted.Contains( beholder ) )
					{
						#region Factions
						PlayerState guildState = PlayerState.Find( guild.Leader );

						PlayerState targetState = PlayerState.Find( beholder );

						Faction guildFaction = (guildState == null ? null : guildState.Faction);

						Faction targetFaction = (targetState == null ? null : targetState.Faction);

						if ( guildFaction != targetFaction || (targetState != null && targetState.IsLeaving) )
						{
							return;
						}

						if ( guildState != null && targetState != null )
						{
							targetState.Leaving = guildState.Leaving;
						}
						#endregion

						guild.Accepted.Remove( beholder );

						guild.AddMember( beholder );

						(beholder as PlayerMobile).GuildRank = 1;

						beholder.CloseGump( typeof( SEGuildGump ) );

						beholder.SendGump( new SEGuildGump( beholder, guild ) );
					}
					else
					{
						beholder.SendGump( new SEGuildGump( beholder, guild ) );
					}
				}
			}
			else
			{
				beholder.SendLocalizedMessage( 1063363 ); // * Requires the "Samurai Empire" expansion

				return;
			}
		}

		public SEGuildGump( Mobile from, Guild guild ) : base( from, guild )
		{
		}

		protected override void Design()
		{
		}
	}
}