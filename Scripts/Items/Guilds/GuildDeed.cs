using System;
using Server.Network;
using Server.Prompts;
using Server.Guilds;
using Server.Multis;
using Server.Regions;

namespace Server.Items
{
	public class GuildDeed : Item
	{
		public override int LabelNumber { get { return 1041055; } } // a guild deed

		[Constructable]
		public GuildDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
		}

		public GuildDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 0.0 )
			{
				Weight = 1.0;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.Guild != null )
			{
				from.SendLocalizedMessage( 501137 ); // You must resign from your current guild before founding another!
			}
			else
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( house == null )
				{
					from.SendLocalizedMessage( 501138 ); // You can only place a guildstone in a house.
				}
				else if ( house.FindGuildstone() != null )
				{
					from.SendLocalizedMessage( 501142 ); //Only one guildstone may reside in a given house.
				}
				else if ( !house.IsOwner( from ) )
				{
					from.SendLocalizedMessage( 501141 ); // You can only place a guildstone in a house you own!
				}
				else
				{
					from.SendLocalizedMessage( 1013060 ); // Enter new guild name (40 characters max):
					from.Prompt = new InternalPrompt( this );
				}
			}
		}

		private class InternalPrompt : Prompt
		{
			private GuildDeed m_Deed;

			public InternalPrompt( GuildDeed deed )
			{
				m_Deed = deed;
			}

			public override void OnResponse( Mobile from, string text )
			{
				if ( m_Deed.Deleted )
				{
					return;
				}

				if ( !m_Deed.IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
				else if ( from.Guild != null )
				{
					from.SendLocalizedMessage( 501137 ); // You must resign from your current guild before founding another!
				}
				else
				{
					BaseHouse house = BaseHouse.FindHouseAt( from );

					if ( house == null )
					{
						from.SendLocalizedMessage( 501138 ); // You can only place a guildstone in a house.
					}
					else if ( house.FindGuildstone() != null )
					{
						from.SendLocalizedMessage( 501142 ); //Only one guildstone may reside in a given house.
					}
					else if ( !house.IsOwner( from ) )
					{
						from.SendLocalizedMessage( 501141 ); // You can only place a guildstone in a house you own!
					}
					else
					{
						m_Deed.Delete();

						if ( text.Length > 40 )
						{
							text = text.Substring( 0, 40 );
						}

						Guild guild = new Guild( from, text, "none" );

						from.Guild = guild;
						from.GuildTitle = "Guildmaster";

						Guildstone stone = new Guildstone( guild );

						stone.MoveToWorld( from.Location, from.Map );

						guild.Guildstone = stone;
					}
				}
			}

			public override void OnCancel( Mobile from )
			{
				from.SendLocalizedMessage( 501145 ); // Placement of guildstone cancelled.
			}
		}
	}
}