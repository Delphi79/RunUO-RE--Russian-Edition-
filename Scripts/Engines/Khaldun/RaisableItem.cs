using System;
using Server;

namespace Server.Items
{
	public class RaisableItem : Item
	{
		private int m_MaxElevation;
		private int m_MoveSound;
		private int m_StopSound;
		private TimeSpan m_CloseDelay;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxElevation
		{
			get { return m_MaxElevation; }
			set
			{
				if ( value <= 0 )
				{
					m_MaxElevation = 0;
				}
				else if ( value >= 60 )
				{
					m_MaxElevation = 60;
				}
				else
				{
					m_MaxElevation = value;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MoveSound { get { return m_MoveSound; } set { m_MoveSound = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int StopSound { get { return m_StopSound; } set { m_StopSound = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan CloseDelay { get { return m_CloseDelay; } set { m_CloseDelay = value; } }

		[Constructable]
		public RaisableItem( int itemID ) : this( itemID, 20, -1, -1, TimeSpan.FromMinutes( 1.0 ) )
		{
		}

		[Constructable]
		public RaisableItem( int itemID, int maxElevation, TimeSpan closeDelay ) : this( itemID, maxElevation, -1, -1, closeDelay )
		{
		}

		[Constructable]
		public RaisableItem( int itemID, int maxElevation, int moveSound, int stopSound, TimeSpan closeDelay ) : base( itemID )
		{
			Movable = false;

			m_MaxElevation = maxElevation;
			m_MoveSound = moveSound;
			m_StopSound = stopSound;
			m_CloseDelay = closeDelay;
		}

		private int m_Elevation;
		private RaiseTimer m_RaiseTimer;

		public bool IsRaisable { get { return m_RaiseTimer == null; } }

		public void Raise()
		{
			if ( !IsRaisable )
			{
				return;
			}

			m_RaiseTimer = new RaiseTimer( this );
			m_RaiseTimer.Start();
		}

		private class RaiseTimer : Timer
		{
			private RaisableItem m_Item;
			private DateTime m_CloseTime;
			private bool m_Up;
			private int m_Step;

			public RaiseTimer( RaisableItem item ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.5 ) )
			{
				m_Item = item;
				m_CloseTime = DateTime.Now + item.CloseDelay;
				m_Up = true;

				Priority = TimerPriority.TenMS;
			}

			protected override void OnTick()
			{
				if ( m_Item.Deleted )
				{
					Stop();
					return;
				}

				if ( m_Step++%3 == 0 )
				{
					if ( m_Up )
					{
						m_Item.Z++;

						if ( ++m_Item.m_Elevation >= m_Item.MaxElevation )
						{
							Stop();

							if ( m_Item.StopSound >= 0 )
							{
								Effects.PlaySound( m_Item.Location, m_Item.Map, m_Item.StopSound );
							}

							m_Up = false;
							m_Step = 0;

							TimeSpan delay = m_CloseTime - DateTime.Now;
							Timer.DelayCall( delay > TimeSpan.Zero ? delay : TimeSpan.Zero, new TimerCallback( Start ) );

							return;
						}
					}
					else
					{
						m_Item.Z--;

						if ( --m_Item.m_Elevation <= 0 )
						{
							Stop();

							if ( m_Item.StopSound >= 0 )
							{
								Effects.PlaySound( m_Item.Location, m_Item.Map, m_Item.StopSound );
							}

							m_Item.m_RaiseTimer = null;

							return;
						}
					}
				}

				if ( m_Item.MoveSound >= 0 )
				{
					Effects.PlaySound( m_Item.Location, m_Item.Map, m_Item.MoveSound );
				}
			}
		}

		public RaisableItem( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.WriteEncodedInt( (int) m_MaxElevation );
			writer.WriteEncodedInt( (int) m_MoveSound );
			writer.WriteEncodedInt( (int) m_StopSound );
			writer.Write( (TimeSpan) m_CloseDelay );

			writer.WriteEncodedInt( (int) m_Elevation );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_MaxElevation = reader.ReadEncodedInt();
			m_MoveSound = reader.ReadEncodedInt();
			m_StopSound = reader.ReadEncodedInt();
			m_CloseDelay = reader.ReadTimeSpan();

			int elevation = reader.ReadEncodedInt();
			this.Z -= elevation;
		}
	}
}