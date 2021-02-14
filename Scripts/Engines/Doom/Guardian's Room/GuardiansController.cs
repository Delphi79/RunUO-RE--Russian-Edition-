using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class GuardianController : Item
	{
		public Rectangle2D Rect = new Rectangle2D( 356, 6, 19, 19 );

		private GuardianDoor m_Door;

		public PoisonTimer m_Timer;

		private bool m_CanActive;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool CanActive { get { return m_CanActive; } set { m_CanActive = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public GuardianDoor Door { get { return m_Door; } set { m_Door = value; } }

		private bool Check()
		{
			foreach ( Item item in World.Items.Values )
			{
				if ( item is GuardianController && !item.Deleted && item != this )
				{
					return true;
				}
			}

			return false;
		}

		[Constructable]
		public GuardianController() : base( 0xFEA )
		{
			if ( Check() )
			{
				World.Broadcast( 0x35, true, "Another guardian's room controller exists in the world!" );
				Delete();
				return;
			}

			Hue = 0x835;

			Movable = false;

			Setup();
		}

		public void Setup()
		{
			m_Door = new GuardianDoor( DoorFacing.SouthCW );

			m_Door.MoveToWorld( new Point3D( 355, 15, -1 ), Map.Malas );

			m_CanActive = true;

			for ( int i = 0; i < 3; i++ )
			{
				int x = Utility.Random( Rect.X, Rect.Width );
				int y = Utility.Random( Rect.Y, Rect.Height );

				GuardianTreasureChest chest = new GuardianTreasureChest( 0xE41 );

				int itemID = Utility.RandomList( chest.ItemIDs );

				chest.ItemID = itemID;

				chest.MoveToWorld( new Point3D( x, y, -1 ), Map.Malas );
			}
		}

		public override bool HandlesOnMovement { get { return true; } }

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( m != null && m_Door != null && !m_Door.Locked && !m_Door.Open && m.Player && m.Alive && m_CanActive )
			{
				IPooledEnumerable eable = Map.Malas.GetMobilesInBounds( Rect );

				foreach ( object obj in eable )
				{
					if ( obj is Mobile )
					{
						Mobile mobile = obj as Mobile;

						if ( mobile != null && !(mobile is DarkGuardian) )
						{
							if ( mobile.Player )
							{
								mobile.SendLocalizedMessage( 1050000, null, 0x41 ); // The locks on the door click loudly and you begin to hear a faint hissing near the walls.
							}

							for ( int j = 0; j < 2; j++ )
							{
								DarkGuardian guard = new DarkGuardian();

								guard.Map = Map.Malas;

								guard.Location = new Point3D( 365, 15, -1 );
							}
						}
					}
				}

				eable.Free();

				m_Door.Locked = true;
				m_Door.Link.Locked = true;

				if ( m_Timer != null )
				{
					m_Timer.Stop();
				}

				m_Timer = new PoisonTimer( this );

				m_Timer.Start();

				m_CanActive = false;
			}
		}

		public void ClearRoom()
		{
			IPooledEnumerable eable = Map.Malas.GetMobilesInBounds( Rect );

			ArrayList list = new ArrayList();

			foreach ( object obj in eable )
			{
				if ( obj is Mobile )
				{
					Mobile mobile = obj as Mobile;

					if ( mobile != null )
					{
						if ( mobile.Player )
						{
							mobile.SendLocalizedMessage( 1050055, null, 0x41 ); // You hear the doors unlocking and the hissing stops.
						}

						if ( mobile is DarkGuardian )
						{
							list.Add( mobile );
						}
					}
				}
			}

			eable.Free();

			for ( int i = 0; i < list.Count; i++ )
			{
				DarkGuardian guardian = list[ i ] as DarkGuardian;

				if ( guardian != null )
				{
					guardian.Delete();
				}
			}

			if ( m_Door != null && m_Door.Link != null )
			{
				m_Door.Locked = false;

				m_Door.Link.Locked = false;
			}

			if ( m_Timer != null )
			{
				m_Timer.Stop();
			}

			InternalTimer timer = new InternalTimer( this ); // at OSI we have some delay for looting corpses ;-)

			timer.Start();
		}

		public GuardianController( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Door );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Door = reader.ReadItem() as GuardianDoor;

			ClearRoom();

			m_CanActive = true;
		}

		public class InternalTimer : Timer
		{
			public GuardianController m_Controller;

			public InternalTimer( GuardianController controller ) : base( TimeSpan.FromMinutes( 1.0 ) )
			{
				m_Controller = controller;
			}

			protected override void OnTick()
			{
				m_Controller.CanActive = true;
			}
		}

		public class PoisonTimer : Timer
		{
			public GuardianController m_Controller;
			public int count = 1;

			public PoisonTimer( GuardianController controller ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Controller = controller;
			}

			public void CheckAlive()
			{
				bool AliveCreatures = false;

				bool AliveGuardians = false;

				IPooledEnumerable eable = Map.Malas.GetMobilesInBounds( m_Controller.Rect );

				foreach ( object obj in eable )
				{
					if ( obj is Mobile )
					{
						Mobile mobile = obj as Mobile;

						if ( mobile != null && mobile.Alive )
						{
							if ( mobile is DarkGuardian )
							{
								AliveGuardians = true;
							}
							else
							{
								AliveCreatures = true;
							}
						}
					}
				}

				eable.Free();

				if ( !(AliveCreatures && AliveGuardians) )
				{
					m_Controller.ClearRoom();

					Stop();
				}
			}

			private static Point3D[] m_FaceLocations = new Point3D[] {new Point3D( 356, 7, -1 ), new Point3D( 356, 13, -1 ), new Point3D( 356, 16, -1 ), new Point3D( 356, 22, -1 ), new Point3D( 358, 6, -1 ), new Point3D( 363, 6, -1 ), new Point3D( 368, 6, -1 ), new Point3D( 373, 6, -1 )};

			public static Point3D RandomFaceLocation()
			{
				int index = Utility.Random( m_FaceLocations.Length );

				return m_FaceLocations[ index ];
			}

			public void Gas( int level )
			{
				int[] x = new int[3], y = new int[3];

				for ( int i = 0; i < x.Length; i++ )
				{
					x[ i ] = Utility.Random( m_Controller.Rect.X, m_Controller.Rect.Width );
					y[ i ] = Utility.Random( m_Controller.Rect.Y, m_Controller.Rect.Height );
				}

				int hue = 0xAC;

				Poison poison = null;

				switch ( level )
				{
					case 0:
						hue = 0xA6;
						poison = Poison.Lesser;
						break;
					case 1:
						hue = 0xAA;
						poison = Poison.Regular;
						break;
					case 2:
						hue = 0xAC;
						poison = Poison.Greater;
						break;
					case 3:
						hue = 0xA8;
						poison = Poison.Deadly;
						break;
					case 4:
						hue = 0xA4;
						poison = Poison.Lethal;
						break;
					case 5:
						hue = 0xAC;
						poison = Poison.Lethal;
						break;
				}

				Effects.SendLocationParticles( EffectItem.Create( new Point3D( x[ 0 ], y[ 0 ], -1 ), Map.Malas, EffectItem.DefaultDuration ), 0x36B0, 1, Utility.Random( 160, 200 ), hue, 0, 0x1F78, 0 );
				Effects.SendLocationParticles( EffectItem.Create( new Point3D( x[ 1 ], y[ 1 ], -1 ), Map.Malas, EffectItem.DefaultDuration ), 0x36CB, 1, Utility.Random( 160, 200 ), hue, 0, 0x1F78, 0 );
				Effects.SendLocationParticles( EffectItem.Create( new Point3D( x[ 2 ], y[ 2 ], -1 ), Map.Malas, EffectItem.DefaultDuration ), 0x36BD, 1, Utility.Random( 160, 200 ), hue, 0, 0x1F78, 0 );

				Effects.SendLocationParticles( EffectItem.Create( RandomFaceLocation(), Map.Malas, EffectItem.DefaultDuration ), 0x1145, 1, 100, 0, 0x4, 0x1F7A, 0 );
				Effects.SendLocationParticles( EffectItem.Create( RandomFaceLocation(), Map.Malas, EffectItem.DefaultDuration ), 0x113A, 1, 100, 0, 0x4, 0x1F79, 0 );

				IPooledEnumerable eable = Map.Malas.GetMobilesInBounds( m_Controller.Rect );

				foreach ( object obj in eable )
				{
					if ( obj is Mobile )
					{
						Mobile mobile = obj as Mobile;

						if ( mobile != null && poison != null && mobile.Poison == null && !(mobile is DarkGuardian) )
						{
							double chance = (level + 1)*0.3;

							if ( chance >= Utility.RandomDouble() )
							{
								mobile.ApplyPoison( mobile, poison );
							}
						}
					}
				}

				eable.Free();
			}


			protected override void OnTick()
			{
				CheckAlive();

				count++;

				int level = (int) (count/60);

				if ( count%60 == 0 ) // every minute we need send message to player about level's change
				{
					int number = 0;
					int hue = 0x485;

					switch ( level )
					{
						case 1:
							number = 1050001;
							break; // It is becoming more difficult for you to breathe as the poisons in the room become more concentrated.
						case 2:
							number = 1050003;
							break; // You begin to panic as the poison clouds thicken.
						case 3:
							number = 1050056;
							break; // Terror grips your spirit as you realize you may never leave this room alive.
						case 4:
							number = 1050057;
							break; // The end is near. You feel hopeless and desolate.  The poison is beginning to stiffen your muscles.
						case 5:
							number = 1062091;
							hue = 0x23F3;
							break; // The poison is becoming too much for you to bear.  You fear that you may die at any moment.
					}

					IPooledEnumerable eable = Map.Malas.GetMobilesInBounds( m_Controller.Rect );

					foreach ( object obj in eable )
					{
						if ( obj is Mobile )
						{
							Mobile mobile = obj as Mobile;

							if ( mobile != null && mobile.Player )
							{
								if ( number != 0 )
								{
									mobile.SendLocalizedMessage( number, null, hue );
								}
							}
						}
					}

					eable.Free();

					if ( level == 5 )
					{
						PainTimer timer = new PainTimer( m_Controller );

						timer.Start();
					}
				}

				if ( count%5 == 0 ) // every 5 seconds we fill room with a gas
				{
					Gas( level );
				}
			}
		}

		public class PainTimer : Timer
		{
			public GuardianController m_Controller;
			public int count = 1;

			public PainTimer( GuardianController controller ) : base( TimeSpan.FromSeconds( 10.0 ), TimeSpan.FromSeconds( 10.0 ) )
			{
				m_Controller = controller;
			}

			protected override void OnTick()
			{
				count++;

				IPooledEnumerable eable = Map.Malas.GetMobilesInBounds( m_Controller.Rect );

				ArrayList targets = new ArrayList();

				foreach ( Mobile mobile in eable )
				{
					targets.Add( mobile );
				}

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile mobile = targets[ i ] as Mobile;

					if ( mobile != null && !(mobile is DarkGuardian) )
					{
						if ( mobile.Player )
						{
							mobile.Say( 1062092 ); // Your body reacts violently from the pain.

							mobile.Animate( 32, 5, 1, true, false, 0 );
						}

						mobile.Damage( Utility.Random( 15, 20 ) );

						if ( count == 10 ) // at OSI at this second all mobiles is killed and room is cleared
						{
							mobile.Kill();
						}
					}
				}

				eable.Free();

				if ( count == 10 )
				{
					if ( m_Controller != null )
					{
						m_Controller.ClearRoom(); // clear room

						if ( m_Controller.m_Timer != null )
						{
							m_Controller.m_Timer.Stop(); // stop gas effects
						}

						Stop(); // stop convulsions
					}
				}
			}
		}
	}
}