using System;
using Server;
using Server.Items;
using Server.Spells.Ninjitsu;

namespace Server.Mobiles
{
	public class Paragon
	{
		public static double Chance = .05; // Chance that a newly spawned creature will be a paragon
		public static double ChestChance = .10; // Chance that a paragon will carry a paragon chest
		public static Map[] Maps = new Map[] // Maps that paragons will spawn on
			{Map.Ilshenar,};

		public static Type[] Artifacts = new Type[] {typeof( GoldBricks ), typeof( PhillipsWoodenSteed ), typeof( AlchemistsBauble ), typeof( ArcticDeathDealer ), typeof( BlazeOfDeath ), typeof( BowOfTheJukaKing ), typeof( BurglarsBandana ), typeof( CavortingClub ), typeof( EnchantedTitanLegBone ), typeof( GwennosHarp ), typeof( IolosLute ), typeof( LunaLance ), typeof( NightsKiss ), typeof( NoxRangersHeavyCrossbow ), typeof( OrcishVisage ), typeof( PolarBearMask ), typeof( ShieldOfInvulnerability ), typeof( StaffOfPower ), typeof( VioletCourage ), typeof( HeartOfTheLion ), typeof( WrathOfTheDryad ), typeof( PixieSwatter ), typeof( GlovesOfThePugilist )};

		public static int Hue = 0x501; // Paragon hue

		// Buffs
		public static double HitsBuff = 5.0;
		public static double StrBuff = 1.05;
		public static double IntBuff = 1.20;
		public static double DexBuff = 1.20;
		public static double SkillsBuff = 1.20;
		public static double SpeedBuff = 2.40;
		public static double FameBuff = 1.40;
		public static double KarmaBuff = 1.40;
		public static int DamageBuff = 5;

		public static void Convert( BaseCreature bc )
		{
			if ( bc.IsParagon )
			{
				return;
			}

			bc.Hue = Hue;

			bc.HitsMaxSeed = (int) (bc.HitsMaxSeed*HitsBuff);
			bc.Hits = bc.HitsMax;

			bc.RawStr = (int) (bc.RawStr*StrBuff);
			bc.RawInt = (int) (bc.RawInt*IntBuff);
			bc.RawDex = (int) (bc.RawDex*DexBuff);

			for ( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = (Skill) bc.Skills[ i ];
				if ( skill.Base == 0.0 )
				{
					continue;
				}
				else
				{
					skill.Base *= SkillsBuff;
				}
			}

			bc.PassiveSpeed /= SpeedBuff;
			bc.ActiveSpeed /= SpeedBuff;

			bc.DamageMin += DamageBuff;
			bc.DamageMax += DamageBuff;

			if ( bc.Fame > 0 )
			{
				bc.Fame = (int) (bc.Fame*FameBuff);
			}
			if ( bc.Karma != 0 )
			{
				bc.Karma = (int) (bc.Karma*KarmaBuff);
			}
		}

		public static void UnConvert( BaseCreature bc )
		{
			if ( !bc.IsParagon )
			{
				return;
			}

			bc.Hue = 0;

			bc.HitsMaxSeed = (int) (bc.HitsMaxSeed/HitsBuff);
			bc.Hits = bc.HitsMax;

			bc.RawStr = (int) (bc.RawStr/StrBuff);
			bc.RawInt = (int) (bc.RawInt/IntBuff);
			bc.RawDex = (int) (bc.RawDex/DexBuff);

			for ( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = (Skill) bc.Skills[ i ];
				if ( skill.Base == 0.0 )
				{
					continue;
				}
				else
				{
					skill.Base /= SkillsBuff;
				}
			}

			bc.PassiveSpeed *= SpeedBuff;
			bc.ActiveSpeed *= SpeedBuff;

			bc.DamageMin -= DamageBuff;
			bc.DamageMax -= DamageBuff;

			if ( bc.Fame > 0 )
			{
				bc.Fame = (int) (bc.Fame/FameBuff);
			}
			if ( bc.Karma != 0 )
			{
				bc.Karma = (int) (bc.Karma/KarmaBuff);
			}
		}

		public static bool CheckConvert( BaseCreature bc )
		{
			return CheckConvert( bc, bc.Location, bc.Map );
		}

		public static bool CheckConvert( BaseCreature bc, Point3D location, Map m )
		{
			if ( !Core.AOS )
			{
				return false;
			}

			if ( Array.IndexOf( Maps, m ) == -1 )
			{
				return false;
			}

			if ( bc is BaseChampion || bc is Harrower || bc is BaseVendor || bc is BaseEscortable || bc.Summoned || bc.Controled || bc is Clone )
			{
				return false;
			}

			return (Chance > Utility.RandomDouble());
		}

		public static bool CheckArtifactChance( Mobile m, BaseCreature bc )
		{
			if ( !Core.AOS )
			{
				return false;
			}

			double fame = (double) bc.Fame;

			if ( fame > 32000 )
			{
				fame = 32000;
			}

			double luck = m.Luck;

			// Luck must be capped to 1200
			if ( luck > 1200 )
			{
				luck = 1200;
			}

			double chance = 1/(Math.Max( 10, 100*(0.83 - Math.Round( Math.Log( Math.Round( fame/6000, 3 ) + 0.001, 10 ), 3 )) )*(100 - Math.Sqrt( luck ))/100.0);

			return chance > Utility.RandomDouble();
		}

		public static void GiveArtifactTo( Mobile m )
		{
			Item item = (Item) Activator.CreateInstance( Artifacts[ Utility.Random( Artifacts.Length ) ] );

			if ( m.AccessLevel > AccessLevel.Player )
			{
				item.Cheater_Name = String.Format( "This item received by GM {0}", m.Name );
			}

			if ( m.AddToBackpack( item ) )
			{
				m.SendMessage( "As a reward for slaying the mighty paragon, an artifact has been placed in your backpack." );
			}
			else
			{
				Container bank = m.BankBox;

				if ( bank != null && bank.TryDropItem( m, item, false ) )
				{
					m.SendMessage( "As a reward for slaying the mighty paragon, an artifact has been placed in your bankbox." );
				}
				else
				{
					m.SendLocalizedMessage( 1072523 ); // You find an artifact, but your backpack and bank are too full to hold it.

					item.MoveToWorld( m.Location, m.Map );
				}
			}
		}
	}
}