using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Engines.CannedEvil;

namespace Server
{
	public class ValorVirtue
	{
		private static TimeSpan LossDelay = TimeSpan.FromDays( 7.0 );

		public static void Initialize()
		{
			VirtueGump.Register( 112, new OnVirtueUsed( OnVirtueUsed ) );
		}

		public static void OnVirtueUsed( Mobile from )
		{
			if ( from.Alive )
			{
				from.SendLocalizedMessage( 1054034 ); // Target the Champion Idol of the Champion you wish to challenge!

				from.Target = new InternalTarget();
			}
		}

		public static void CheckAtrophy( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null )
			{
				return;
			}

			try
			{
				if ( (pm.LastValorLoss + LossDelay) < DateTime.Now )
				{
					if ( VirtueHelper.Atrophy( from, VirtueName.Valor ) )
					{
						from.SendLocalizedMessage( 1054040 ); // You have lost some Valor.
					}

					VirtueLevel level = VirtueHelper.GetLevel( from, VirtueName.Valor );

					pm.LastValorLoss = DateTime.Now;
				}
			} 
			catch
			{
			}
		}

		public static bool CheckValor( ChampionSpawn spawn, Mobile from )
		{
			VirtueLevel level = VirtueHelper.GetLevel( from, VirtueName.Valor );

			if ( spawn != null && VirtueHelper.HasAny( from, VirtueName.Valor ) )
			{
				if ( level <= VirtueLevel.Seeker && spawn.Level < 5 )
				{
					return true;
				}

				if ( level <= VirtueLevel.Follower && spawn.Level < 10 )
				{
					return true;
				}

				if ( level <= VirtueLevel.Knight && spawn.Level < 15 )
				{
					return true;
				}
			}

			return false;
		}

		public static void Valor( Mobile from, object targeted )
		{
			if ( !from.CheckAlive() )
			{
				return;
			}

			ChampionIdol targ = targeted as ChampionIdol;

			if ( targ == null )
			{
				return;
			}

			VirtueLevel level = VirtueHelper.GetLevel( from, VirtueName.Valor );

			int current = from.Virtues.GetValue( (int) VirtueName.Valor );

			if ( targ.Spawn.IsValorUsed )
			{
				from.SendLocalizedMessage( 1054038 ); // The Champion of this region has already been challenged!	
				return;
			}

			if ( !targ.Spawn.Active )
			{
				if ( level >= VirtueLevel.Knight )
				{
					targ.Spawn.Active = true;

					if ( targ.Spawn.RandomizeType )
					{
						switch ( Utility.Random( 5 ) )
						{
							case 0:
								targ.Spawn.Type = ChampionSpawnType.VerminHorde;
								break;
							case 1:
								targ.Spawn.Type = ChampionSpawnType.UnholyTerror;
								break;
							case 2:
								targ.Spawn.Type = ChampionSpawnType.ColdBlood;
								break;
							case 3:
								targ.Spawn.Type = ChampionSpawnType.Abyss;
								break;
							case 4:
								targ.Spawn.Type = ChampionSpawnType.Arachnid;
								break;
						}
					}

					// It's strange, but at OSI Valor not only activate spawn but also increase 2-4 levels on it.
					// If counts of increased levels less than 4, champion spawn can be used with Valor again for advance level.
					int candles = Utility.RandomMinMax( 2, 4 );
					targ.Spawn.Level = candles;
					if ( candles > 3 )
					{
						targ.Spawn.IsValorUsed = true;
					}

					// on Stratics written:
					// The player will sacrifice a large portion of his Valor in order to activate a champion spawn shrine.
					// Really, player become Follower of Valor instead Knight
					from.Virtues.SetValue( (int) VirtueName.Valor, 29 );

					// Your challenge is heard by the Champion of this region! Beware its wrath!
					targ.PublicOverheadMessage( Network.MessageType.Regular, 0x3B2, 1054037, "" );

					from.SendLocalizedMessage( 1054040 ); // You have lost some Valor.
				}
				else
				{
					from.SendLocalizedMessage( 1054036 ); // You must be a Knight of Valor to summon the champion's spawn in this manner!
				}
			}
			else
			{
				if ( targ.Spawn.Champion == null )
				{
					if ( CheckValor( targ.Spawn, from ) )
					{
						int sacrifice_advance_level = 0;

						if ( targ.Spawn.Level < 5 )
						{
							sacrifice_advance_level = 5;
						}

						if ( targ.Spawn.Level >= 5 && targ.Spawn.Level < 10 )
						{
							sacrifice_advance_level = 10;
						}

						if ( targ.Spawn.Level >= 10 && targ.Spawn.Level < 15 )
						{
							sacrifice_advance_level = 15;
						}

						if ( sacrifice_advance_level > current )
						{
							sacrifice_advance_level = current;
						}

						targ.Spawn.Level += 1;

						from.Virtues.SetValue( (int) VirtueName.Valor, current - sacrifice_advance_level );

						// Your challenge is heard by the Champion of this region! Beware its wrath!
						targ.PublicOverheadMessage( Network.MessageType.Regular, 0x3B2, 1054037, "" );

						from.SendLocalizedMessage( 1054040 ); // You have lost some Valor.

						targ.Spawn.IsValorUsed = true;
					}
					else
					{
						from.SendLocalizedMessage( 1054039 ); // The Champion of this region ignores your challenge. You must further prove your valor. 			
					}

					if ( targ.Spawn.Level >= 15 && level >= VirtueLevel.Knight )
					{
						if ( 0.03 >= Utility.RandomDouble() ) // TODO: verify
						{
							// Your challenge is heard by the Champion of this region! Beware its wrath!
							targ.PublicOverheadMessage( Network.MessageType.Regular, 0x3B2, 1054037, "" );

							targ.Spawn.SpawnChampion();

							from.Virtues.SetValue( (int) VirtueName.Valor, 0 );

							from.SendLocalizedMessage( 1054040 ); // You have lost some Valor.

							targ.Spawn.IsValorUsed = true;
						}
						else
						{
							from.SendLocalizedMessage( 1054039 ); // The Champion of this region ignores your challenge. You must further prove your valor. 			
						}
					}
				}
				else
				{
					from.SendLocalizedMessage( 1054038 ); // The Champion of this region has already been challenged!	
				}
			}
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 8, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is ChampionIdol )
				{
					Valor( from, targeted );
				}
				else
				{
					from.SendLocalizedMessage( 1054035 ); // You must target a Champion Idol to challenge the Champion's spawn!
				}
			}

			protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
			{
				from.SendLocalizedMessage( 1054035 ); // You must target a Champion Idol to challenge the Champion's spawn!			
			}
		}
	}
}