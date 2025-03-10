using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Engines.Quests.Collector
{
	public class ElwoodMcCarrin : BaseQuester
	{
		[Constructable]
		public ElwoodMcCarrin() : base( "the well-known collector" )
		{
		}

		public ElwoodMcCarrin( Serial serial ) : base( serial )
		{
		}

		public override void InitBody()
		{
			InitStats( 100, 100, 25 );

			Hue = 0x83ED;

			Female = false;
			Body = 0x190;
			Name = "Elwood McCarrin";
		}

		public override void InitOutfit()
		{
			AddItem( new FancyShirt() );
			AddItem( new LongPants( 0x544 ) );
			AddItem( new Shoes( 0x454 ) );
			AddItem( new JesterHat( 0x4D2 ) );
			AddItem( new FullApron( 0x4D2 ) );

			AddItem( new PonyTail( 0x47D ) );
			AddItem( new Goatee( 0x47D ) );
		}

		public override void OnTalk( PlayerMobile player, bool contextMenu )
		{
			Direction = GetDirectionTo( player );

			QuestSystem qs = player.Quest;

			if ( qs is CollectorQuest )
			{
				if ( qs.IsObjectiveInProgress( typeof( FishPearlsObjective ) ) )
				{
					qs.AddConversation( new ElwoodDuringFishConversation() );
				}
				else
				{
					QuestObjective obj = qs.FindObjective( typeof( ReturnPearlsObjective ) );

					if ( obj != null && !obj.Completed )
					{
						obj.Complete();
					}
					else if ( qs.IsObjectiveInProgress( typeof( FindAlbertaObjective ) ) )
					{
						qs.AddConversation( new ElwoodDuringPainting1Conversation() );
					}
					else if ( qs.IsObjectiveInProgress( typeof( SitOnTheStoolObjective ) ) )
					{
						qs.AddConversation( new ElwoodDuringPainting2Conversation() );
					}
					else
					{
						obj = qs.FindObjective( typeof( ReturnPaintingObjective ) );

						if ( obj != null && !obj.Completed )
						{
							obj.Complete();
						}
						else if ( qs.IsObjectiveInProgress( typeof( FindGabrielObjective ) ) )
						{
							qs.AddConversation( new ElwoodDuringAutograph1Conversation() );
						}
						else if ( qs.IsObjectiveInProgress( typeof( FindSheetMusicObjective ) ) )
						{
							qs.AddConversation( new ElwoodDuringAutograph2Conversation() );
						}
						else if ( qs.IsObjectiveInProgress( typeof( ReturnSheetMusicObjective ) ) )
						{
							qs.AddConversation( new ElwoodDuringAutograph3Conversation() );
						}
						else
						{
							obj = qs.FindObjective( typeof( ReturnAutographObjective ) );

							if ( obj != null && !obj.Completed )
							{
								obj.Complete();
							}
							else if ( qs.IsObjectiveInProgress( typeof( FindTomasObjective ) ) )
							{
								qs.AddConversation( new ElwoodDuringToys1Conversation() );
							}
							else if ( qs.IsObjectiveInProgress( typeof( CaptureImagesObjective ) ) )
							{
								qs.AddConversation( new ElwoodDuringToys2Conversation() );
							}
							else if ( qs.IsObjectiveInProgress( typeof( ReturnImagesObjective ) ) )
							{
								qs.AddConversation( new ElwoodDuringToys3Conversation() );
							}
							else
							{
								obj = qs.FindObjective( typeof( ReturnToysObjective ) );

								if ( obj != null && !obj.Completed )
								{
									obj.Complete();

									if ( GiveReward( player ) )
									{
										qs.AddConversation( new EndConversation() );
									}
									else
									{
										qs.AddConversation( new FullEndConversation( true ) );
									}
								}
								else
								{
									obj = qs.FindObjective( typeof( MakeRoomObjective ) );

									if ( obj != null && !obj.Completed )
									{
										if ( GiveReward( player ) )
										{
											obj.Complete();
											qs.AddConversation( new EndConversation() );
										}
										else
										{
											qs.AddConversation( new FullEndConversation( false ) );
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				QuestSystem newQuest = new CollectorQuest( player );

				if ( qs == null && QuestSystem.CanOfferQuest( player, typeof( CollectorQuest ) ) )
				{
					newQuest.SendOffer();
				}
				else
				{
					newQuest.AddConversation( new DontOfferConversation() );
				}
			}
		}

		public bool GiveReward( Mobile to )
		{
			Bag bag = new Bag();

			bag.DropItem( new Gold( Utility.RandomMinMax( 500, 1000 ) ) );

			if ( Utility.RandomBool() )
			{
				BaseWeapon weapon = Loot.RandomWeapon();

				if ( Core.AOS )
				{
					BaseRunicTool.ApplyAttributesTo( weapon, 2, 20, 30 );
				}
				else
				{
					weapon.DamageLevel = (WeaponDamageLevel) BaseCreature.RandomMinMaxScaled( 2, 3 );
					weapon.AccuracyLevel = (WeaponAccuracyLevel) BaseCreature.RandomMinMaxScaled( 2, 3 );
					weapon.DurabilityLevel = (WeaponDurabilityLevel) BaseCreature.RandomMinMaxScaled( 2, 3 );
				}

				bag.DropItem( weapon );
			}
			else
			{
				Item item;

				if ( Core.AOS )
				{
					item = Loot.RandomArmorOrShieldOrJewelry();

					if ( item is BaseArmor )
					{
						BaseRunicTool.ApplyAttributesTo( (BaseArmor) item, 2, 20, 30 );
					}
					else if ( item is BaseJewel )
					{
						BaseRunicTool.ApplyAttributesTo( (BaseJewel) item, 2, 20, 30 );
					}
				}
				else
				{
					BaseArmor armor = Loot.RandomArmorOrShield();
					item = armor;

					armor.ProtectionLevel = (ArmorProtectionLevel) BaseCreature.RandomMinMaxScaled( 2, 3 );
					armor.Durability = (ArmorDurabilityLevel) BaseCreature.RandomMinMaxScaled( 2, 3 );
				}

				bag.DropItem( item );
			}

			bag.DropItem( new Obsidian() );

			return to.PlaceInBackpack( bag );
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
}