using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Engines.Quests;

namespace Server.Engines.Quests.Collector
{
	public class Impresario : BaseQuester
	{
		[Constructable]
		public Impresario() : base( "the impresario" )
		{
		}

		public Impresario( Serial serial ) : base( serial )
		{
		}

		public override void InitBody()
		{
			InitStats( 100, 100, 25 );

			Hue = Utility.RandomSkinHue();

			Female = false;
			Body = 0x190;
			Name = NameList.RandomName( "male" );
		}

		public override void InitOutfit()
		{
			AddItem( new FancyShirt( Utility.RandomDyedHue() ) );
			AddItem( new LongPants( Utility.RandomNondyedHue() ) );
			AddItem( new Shoes( Utility.RandomNeutralHue() ) );

			Item hair = new Item( Utility.RandomList( 0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2049, 0x204A ) );
			hair.Hue = Utility.RandomHairHue();
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem( hair );

			Item beard = new Item( Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D ) );
			beard.Hue = hair.Hue;
			beard.Layer = Layer.FacialHair;
			beard.Movable = false;
			AddItem( beard );
		}

		public override bool CanTalkTo( PlayerMobile to )
		{
			QuestSystem qs = to.Quest as CollectorQuest;

			if ( qs == null )
			{
				return false;
			}

			return qs.IsObjectiveInProgress( typeof( FindSheetMusicObjective ) );
		}

		public override void OnTalk( PlayerMobile player, bool contextMenu )
		{
			QuestSystem qs = player.Quest;

			if ( qs is CollectorQuest )
			{
				FindSheetMusicObjective obj = qs.FindObjective( typeof( FindSheetMusicObjective ) ) as FindSheetMusicObjective;

				if ( obj != null && !obj.Completed )
				{
					Direction = GetDirectionTo( player );

					if ( obj.IsInRightTheater() )
					{
						player.CloseGump( typeof( SheetMusicOfferGump ) );
						player.SendGump( new SheetMusicOfferGump() );
					}
					else
					{
						qs.AddConversation( new NoSheetMusicConversation() );
					}
				}
			}
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
		}
	}

	public class SheetMusicOfferGump : BaseQuestGump
	{
		public SheetMusicOfferGump() : base( 75, 25 )
		{
			Closable = false;

			AddImage( 349, 10, 0x24B0 );
			AddImageTiled( 349, 130, 100, 120, 0x24B3 );
			AddImageTiled( 149, 10, 200, 140, 0x24AF );
			AddImageTiled( 149, 300, 200, 140, 0x24B5 );
			AddImage( 349, 300, 0x24B6 );
			AddImage( 35, 10, 0x24AE );
			AddImageTiled( 35, 150, 120, 100, 0x24B1 );
			AddImage( 35, 300, 0x24B4 );

			AddHtmlLocalized( 110, 60, 200, 20, 1049069, White, false, false ); // <STRONG>Conversation Event</STRONG>

			AddImage( 65, 14, 0x2776 );
			AddImageTiled( 81, 14, 349, 17, 0x2775 );
			AddImage( 426, 14, 0x2778 );

			AddImageTiled( 50, 37, 400, 376, 0xA40 );
			AddAlphaRegion( 50, 37, 400, 376 );

			AddImage( 0, 0, 0x28C8 );

			AddImageTiled( 75, 90, 200, 1, 0x238D );
			AddImage( 75, 58, 0x2635 );
			AddImage( 380, 45, 0xDF );

			AddHtmlLocalized( 98, 140, 312, 200, 1055107, LightGreen, false, true ); // Sure, I have some sheet music for a Gabriel Piete song. I'd be happy to sell you a copy for 10 gold.

			AddRadio( 85, 350, 0x25F8, 0x25FB, true, 1 );
			AddHtmlLocalized( 120, 356, 280, 20, 1014088, White, false, false ); // I accept.

			AddRadio( 85, 385, 0x25F8, 0x25FB, false, 0 );
			AddHtmlLocalized( 120, 391, 280, 20, 1049012, White, false, false ); // No thanks, I decline.

			AddButton( 340, 390, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 1 && info.IsSwitched( 1 ) )
			{
				PlayerMobile player = sender.Mobile as PlayerMobile;

				if ( player != null )
				{
					QuestSystem qs = player.Quest;

					if ( qs is CollectorQuest )
					{
						FindSheetMusicObjective obj = qs.FindObjective( typeof( FindSheetMusicObjective ) ) as FindSheetMusicObjective;

						if ( obj != null && !obj.Completed )
						{
							if ( player.BankBox != null && player.BankBox.ConsumeTotal( typeof( Gold ), 10 ) )
							{
								obj.Complete();
							}
							else
							{
								player.SendLocalizedMessage( 1055108 ); // You don't have enough gold to buy the sheet music.
							}
						}
					}
				}
			}
		}
	}
}