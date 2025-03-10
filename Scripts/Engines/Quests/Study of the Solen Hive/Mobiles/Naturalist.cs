using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Plants;
using Server.Engines.Quests;

namespace Server.Engines.Quests.Naturalist
{
	public class Naturalist : BaseQuester
	{
		[Constructable]
		public Naturalist() : base( "the Naturalist" )
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
			AddItem( new Tunic( 0x598 ) );
			AddItem( new LongPants( 0x59B ) );
			AddItem( new Boots() );

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

		public override void OnTalk( PlayerMobile player, bool contextMenu )
		{
			StudyOfSolenQuest qs = player.Quest as StudyOfSolenQuest;

			if ( qs != null && qs.Naturalist == this )
			{
				StudyNestsObjective study = qs.FindObjective( typeof( StudyNestsObjective ) ) as StudyNestsObjective;

				if ( study != null )
				{
					if ( !study.Completed )
					{
						PlaySound( 0x41F );
						qs.AddConversation( new NaturalistDuringStudyConversation() );
					}
					else
					{
						QuestObjective obj = qs.FindObjective( typeof( ReturnToNaturalistObjective ) );

						if ( obj != null && !obj.Completed )
						{
							Seed reward;

							PlantType type;
							switch ( Utility.Random( 17 ) )
							{
								case 0:
									type = PlantType.CampionFlowers;
									break;
								case 1:
									type = PlantType.Poppies;
									break;
								case 2:
									type = PlantType.Snowdrops;
									break;
								case 3:
									type = PlantType.Bulrushes;
									break;
								case 4:
									type = PlantType.Lilies;
									break;
								case 5:
									type = PlantType.PampasGrass;
									break;
								case 6:
									type = PlantType.Rushes;
									break;
								case 7:
									type = PlantType.ElephantEarPlant;
									break;
								case 8:
									type = PlantType.Fern;
									break;
								case 9:
									type = PlantType.PonytailPalm;
									break;
								case 10:
									type = PlantType.SmallPalm;
									break;
								case 11:
									type = PlantType.CenturyPlant;
									break;
								case 12:
									type = PlantType.WaterPlant;
									break;
								case 13:
									type = PlantType.SnakePlant;
									break;
								case 14:
									type = PlantType.PricklyPearCactus;
									break;
								case 15:
									type = PlantType.BarrelCactus;
									break;
								default:
									type = PlantType.TribarrelCactus;
									break;
							}

							if ( study.StudiedSpecialNest )
							{
								reward = new Seed( type, PlantHue.FireRed, false );
							}
							else
							{
								PlantHue hue;
								switch ( Utility.Random( 3 ) )
								{
									case 0:
										hue = PlantHue.Pink;
										break;
									case 1:
										hue = PlantHue.Magenta;
										break;
									default:
										hue = PlantHue.Aqua;
										break;
								}

								reward = new Seed( type, hue, false );
							}

							if ( player.PlaceInBackpack( reward ) )
							{
								obj.Complete();

								PlaySound( 0x449 );
								PlaySound( 0x41B );

								if ( study.StudiedSpecialNest )
								{
									qs.AddConversation( new SpecialEndConversation() );
								}
								else
								{
									qs.AddConversation( new EndConversation() );
								}
							}
							else
							{
								reward.Delete();

								qs.AddConversation( new FullBackpackConversation() );
							}
						}
					}
				}
			}
			else
			{
				QuestSystem newQuest = new StudyOfSolenQuest( player, this );

				if ( player.Quest == null && QuestSystem.CanOfferQuest( player, typeof( StudyOfSolenQuest ) ) )
				{
					PlaySound( 0x42F );
					newQuest.SendOffer();
				}
				else
				{
					PlaySound( 0x448 );
					newQuest.AddConversation( new DontOfferConversation() );
				}
			}
		}

		public Naturalist( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}