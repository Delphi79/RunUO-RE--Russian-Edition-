using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Spells.Ninjitsu;

namespace Server.Items
{
	public class Bola : Item
	{
		[Constructable]
		public Bola() : this( 1 )
		{
		}

		[Constructable]
		public Bola( int amount ) : base( 0x26AC )
		{
			Weight = 4.0;
			Stackable = true;
			Amount = amount;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1040019 ); // The bola must be in your pack to use it.
			}
			else if ( !from.CanBeginAction( typeof( Bola ) ) )
			{
				from.SendLocalizedMessage( 1049624 ); // You have to wait a few moments before you can use another bola!
			}
			else if ( from.Target is BolaTarget )
			{
				from.SendLocalizedMessage( 1049631 ); // This bola is already being used.
			}
			else if ( !Core.AOS && (from.FindItemOnLayer( Layer.OneHanded ) != null || from.FindItemOnLayer( Layer.TwoHanded ) != null) )
			{
				from.SendLocalizedMessage( 1040015 ); // Your hands must be free to use this
			}
			else if ( from.Mounted )
			{
				from.SendLocalizedMessage( 1040016 ); // You cannot use this while riding a mount
			}
			else if ( AnimalForm.UnderEffect( from ) )
			{
				from.SendLocalizedMessage( 1070902 ); // You can't use this while in an animal form!
			}
			else
			{
				EtherealMount.StopMounting( from );

				Item one = from.FindItemOnLayer( Layer.OneHanded );
				Item two = from.FindItemOnLayer( Layer.TwoHanded );

				if ( one != null )
				{
					from.Backpack.DropItem( one );
				}

				if ( two != null )
				{
					from.Backpack.DropItem( two );
				}

				from.Target = new BolaTarget( this );
				from.LocalOverheadMessage( MessageType.Emote, 0x3B2, 1049632 ); // * You begin to swing the bola...*
				from.NonlocalOverheadMessage( MessageType.Emote, 0x3B2, 1049633, from.Name ); // ~1_NAME~ begins to menacingly swing a bola...
			}
		}

		private static void ReleaseBolaLock( object state )
		{
			((Mobile) state).EndAction( typeof( Bola ) );
		}

		private static void FinishThrow( object state )
		{
			object[] states = (object[]) state;

			Mobile from = (Mobile) states[ 0 ];
			Mobile to = (Mobile) states[ 1 ];

			if ( Core.AOS )
			{
				new Bola().MoveToWorld( to.Location, to.Map );
			}

			to.Damage( 1, from );

			IMount mt = to.Mount;

			if ( mt != null )
			{
				mt.Rider = null;
			}

			to.SendLocalizedMessage( 1040023 ); // You have been knocked off of your mount!

			BaseMount.SetMountPrevention( to, BlockMountType.Dazed, TimeSpan.FromSeconds( 3.0 ) );

			Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( ReleaseBolaLock ), from );
		}

		private class BolaTarget : Target
		{
			private Bola m_Bola;

			public BolaTarget( Bola bola ) : base( 8, false, TargetFlags.Harmful )
			{
				m_Bola = bola;
			}

			protected override void OnTarget( Mobile from, object obj )
			{
				if ( m_Bola.Deleted )
				{
					return;
				}

				if ( obj is Mobile )
				{
					Mobile to = (Mobile) obj;

					if ( !m_Bola.IsChildOf( from.Backpack ) )
					{
						from.SendLocalizedMessage( 1040019 ); // The bola must be in your pack to use it.
					}
					else if ( !Core.AOS && (from.FindItemOnLayer( Layer.OneHanded ) != null || from.FindItemOnLayer( Layer.TwoHanded ) != null) )
					{
						from.SendLocalizedMessage( 1040015 ); // Your hands must be free to use this
					}
					else if ( from.Mounted )
					{
						from.SendLocalizedMessage( 1040016 ); // You cannot use this while riding a mount
					}
					else if ( !to.Mounted )
					{
						from.SendLocalizedMessage( 1049628 ); // You have no reason to throw a bola at that.
					}
					else if ( to is Neira || to is ChaosDragoon || to is ChaosDragoonElite )
					{
						from.SendLocalizedMessage( 1042047 ); // You fail to knock the rider from its mount.
					}
					else if ( !from.CanBeHarmful( to ) )
					{
					}
					else if ( from.BeginAction( typeof( Bola ) ) )
					{
						from.RevealingAction();

						EtherealMount.StopMounting( from );

						Item one = from.FindItemOnLayer( Layer.OneHanded );
						Item two = from.FindItemOnLayer( Layer.TwoHanded );

						if ( one != null )
						{
							from.AddToBackpack( one );
						}

						if ( two != null )
						{
							from.AddToBackpack( two );
						}

						from.DoHarmful( to );

						if ( Core.AOS )
						{
							BaseMount.SetMountPrevention( from, BlockMountType.BolaRecovery, TimeSpan.FromSeconds( 3.0 ) );
						}

						m_Bola.Consume();

						from.Direction = from.GetDirectionTo( to );
						from.Animate( 11, 5, 1, true, false, 0 );
						from.MovingEffect( to, 0x26AC, 10, 0, false, false );

						Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), new TimerStateCallback( FinishThrow ), new object[] {from, to} );
					}
					else
					{
						from.SendLocalizedMessage( 1049624 ); // You have to wait a few moments before you can use another bola!
					}
				}
				else
				{
					from.SendLocalizedMessage( 1049629 ); // You cannot throw a bola at that.
				}
			}
		}

		public Bola( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Bola( amount ), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}