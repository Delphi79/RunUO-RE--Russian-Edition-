using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class TokunoTreasures
	{
		public static bool Enabled = false; // at OSI TT Event isn't static, so we can disable if it's need

		public static Type[] MinorArtifacts = new Type[] {typeof( AncientFarmersKasa ), typeof( AncientSamuraiDo ), typeof( AncientUrn ), typeof( ArmsOfTacticalExcellence ), typeof( BlackLotusHood ), typeof( ChestOfHeirlooms ), typeof( DaimyosHelm ), typeof( DemonForks ), typeof( DragonNunchaku ), typeof( Exiler ), typeof( FluteOfRenewal ), typeof( GlovesOfTheSun ), typeof( HanzosBow ), typeof( HonorableSwords ), typeof( LegsOfStability ), typeof( PeasantsBokuto ), typeof( PigmentsOfTokuno ), typeof( PilferedDancerFans ), typeof( TheDestroyer ), typeof( TomeOfEnlightenment )};

		public static Type[] MajorArtifacts = new Type[] {typeof( DarkenedSky ), typeof( KasaOfRajin ), typeof( PigmentsOfTokunoMajor ), typeof( RuneBeetleCarapace ), typeof( Stormgrip ), typeof( SwordOfStampede ), typeof( SwordsOfProsperity ), typeof( TheHorselord ), typeof( TomeOfLostKnowledge ), typeof( WindsEdge )};

		public static bool CheckArtifactChance( Mobile from, BaseCreature bc )
		{
			if ( !Core.AOS )
			{
				return false;
			}

			if ( bc.Map != Map.Malas && bc.Map != Map.Tokuno )
			{
				return false;
			}

			if ( bc is Crane )
			{
				return false;
			}

			if ( bc.Map == Map.Malas )
			{
				if ( bc.Region.Name != "Fan Dancer Dojo" && bc.Region.Name != "Yomotsu Mines" )
				{
					return false;
				}
			}

			double fame = (double) bc.Fame;

			PlayerMobile pm = from as PlayerMobile;

			int luck = pm.Luck;

			pm.ChanceGetArtifact += fame/10000000; // TODO: verify 

			double chance = pm.ChanceGetArtifact;

			if ( luck > 0 )
			{
				// Luck must be capped to 1200
				if ( luck > 1200 )
				{
					luck = 1200;
				}

				double luckmodifier = ((double) luck/36); // TODO: verify			

				chance *= luckmodifier;
			}

			return chance > Utility.RandomDouble();
		}

		public static void GiveArtifactTo( Mobile m )
		{
			Item item = (Item) Activator.CreateInstance( MinorArtifacts[ Utility.Random( MinorArtifacts.Length ) ] );

			if ( m.AccessLevel > AccessLevel.Player )
			{
				item.Cheater_Name = String.Format( "This item received by GM {0}", m.Name );
			}

			bool message = true;

			if ( !m.AddToBackpack( item ) )
			{
				Container bank = m.BankBox;

				if ( !(bank != null && bank.TryDropItem( m, item, false )) )
				{
					m.SendLocalizedMessage( 1072523 ); // You find an artifact, but your backpack and bank are too full to hold it.

					message = false;

					item.MoveToWorld( m.Location, m.Map );
				}
			}

			if ( message )
			{
				// For your valor in combating the fallen beast, a special artifact has been bestowed on you.
				m.SendLocalizedMessage( 1062317 );
			} 

			PlayerMobile pm = m as PlayerMobile;

			pm.ChanceGetArtifact = 0;
		}
	}
}