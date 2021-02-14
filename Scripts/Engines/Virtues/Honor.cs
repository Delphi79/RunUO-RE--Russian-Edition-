using System;
using System.Collections;
using Server;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server
{
	public class HonorVirtue
	{
		private static TimeSpan LossDelay = TimeSpan.FromDays( 7.0 );

		public static Hashtable m_Table = new Hashtable();

		public static Hashtable m_Table2 = new Hashtable();

		public static bool UnderHonorEmbrace( Mobile m )
		{
			return m_Table.Contains( m );
		}

		public static bool CantEmbrace( Mobile m )
		{
			return m_Table2.Contains( m );
		}

		private static void Expire_Callback( object state )
		{
			Mobile m = (Mobile) state;

			m.SendLocalizedMessage( 1063236 ); // You no longer embrace your honor

			m_Table.Remove( m );
		}

		private static void Expire_Callback2( object state )
		{
			Mobile m = (Mobile) state;

			m_Table2.Remove( m );
		}

		public static void Initialize()
		{
			VirtueGump.Register( 107, new OnVirtueUsed( OnVirtueUsed ) );
		}

		public static void OnVirtueUsed( Mobile from )
		{
			if ( from.Alive )
			{
				from.SendLocalizedMessage( 1063160 ); // Target what you wish to honor.

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
				if ( (pm.LastHonorLoss + LossDelay) < DateTime.Now )
				{
					if ( VirtueHelper.Atrophy( from, VirtueName.Honor ) )
					{
						from.SendLocalizedMessage( 1063227 ); // You have lost some Honor.
					}

					pm.LastHonorLoss = DateTime.Now;
				}
			} 
			catch
			{
			}
		}

		public static void Honor( Mobile from, object targeted )
		{
			if ( !from.CheckAlive() )
			{
				return;
			}

			Mobile targ = targeted as Mobile;

			if ( targ == null )
			{
				return;
			}

			VirtueLevel level = VirtueHelper.GetLevel( from, VirtueName.Honor );

			if ( targ == from )
			{
				if ( CantEmbrace( from ) )
				{
					from.SendLocalizedMessage( 1063230 ); // You must wait awhile before you can embrace honor again.

					return;
				}

				if ( level < VirtueLevel.Seeker )
				{
					from.SendLocalizedMessage( 1063234 ); // You do not have enough honor to do that

					return;
				}

				Timer t = (Timer) m_Table[ from ];

				if ( t != null )
				{
					t.Stop();
				}

				Timer t2 = (Timer) m_Table2[ from ];

				if ( t2 != null )
				{
					t.Stop();
				}

				double delay = 0;

				switch ( level )
				{
					case VirtueLevel.Seeker:
						delay = 60.0;
						break;
					case VirtueLevel.Follower:
						delay = 90.0;
						break;
					case VirtueLevel.Knight:
						delay = 120.0;
						break;
				}

				m_Table[ from ] = t = Timer.DelayCall( TimeSpan.FromSeconds( delay ), new TimerStateCallback( Expire_Callback ), from );

				m_Table2[ from ] = t2 = Timer.DelayCall( TimeSpan.FromMinutes( 5.0 ), new TimerStateCallback( Expire_Callback2 ), from );

				from.SendLocalizedMessage( 1063235 ); // You embrace your honor

				if ( VirtueHelper.Atrophy( from, VirtueName.Honor ) )
				{
					from.SendLocalizedMessage( 1063227 ); // You have lost some Honor.			
				}
			}
			else
			{
				if ( targ is BaseCreature )
				{
					BaseCreature cr = targ as BaseCreature;

					if ( cr.HonorOpponent != null && cr.HonorOpponent != from )
					{
						from.SendLocalizedMessage( 1063233 ); // Somebody else is honoring this opponent

						return;
					}
				}

				if ( targ is PlayerMobile )
				{
					PlayerMobile pm = targ as PlayerMobile;

					if ( pm.HonorOpponent != null && pm.HonorOpponent != from )
					{
						from.SendLocalizedMessage( 1063233 ); // Somebody else is honoring this opponent

						return;
					}
				}

				if ( VirtueHelper.IsHighestPath( from, VirtueName.Honor ) )
				{
					from.SendLocalizedMessage( 1063228 ); // You cannot gain more Honor.

					return;
				}

				if ( !from.InRange( targ.Location, 5 ) )
				{
					from.SendLocalizedMessage( 1063232 ); // You are too far away to honor your opponent

					return;
				}

				if ( ((targ.Hits*100)/Math.Max( targ.HitsMax, 1 )) < 85 )
				{
					from.SendLocalizedMessage( 1063166 ); // You cannot honor this monster because it is too damaged.

					return;
				}

				if ( !NotorietyHandlers.Mobile_AllowHarmful( from, targ ) )
				{
					return;
				}

				if ( !from.CanSee( targ ) || !from.InLOS( targ ) )
				{
					return;
				}

				from.Direction = from.GetDirectionTo( targ.Location );

				from.Animate( 32, 5, 1, true, false, 0 );

				from.Say( 1063231 ); // I honor you

				PlayerMobile player = from as PlayerMobile;

				player.HonorOpponent = targ;

				if ( targ is BaseCreature )
				{
					((BaseCreature) targ).HonorOpponent = player;
				}

				if ( targ is PlayerMobile )
				{
					((PlayerMobile) targ).HonorOpponent = player;
				}

				player.SpotHonor = player.Location;

				player.Perfection = 0;
			}
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 8, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Mobile )
				{
					Honor( from, targeted );
				}
			}

			protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
			{
				from.SendLocalizedMessage( 1004054 ); // You cannot perform negative acts on your target.
			}
		}
	}
}