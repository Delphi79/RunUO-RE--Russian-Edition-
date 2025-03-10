using System;

namespace Server.Items
{
	public enum TrapType
	{
		None,
		MagicTrap,
		ExplosionTrap,
		DartTrap,
		PoisonTrap
	}

	public abstract class TrapableContainer : BaseContainer, ITelekinesisable
	{
		private TrapType m_TrapType;
		private int m_TrapPower;
		private bool m_Enabled;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Enabled { get { return m_Enabled; } set { m_Enabled = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public TrapType TrapType { get { return m_TrapType; } set { m_TrapType = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int TrapPower { get { return m_TrapPower; } set { m_TrapPower = value; } }

		public TrapableContainer( int itemID ) : base( itemID )
		{
		}

		public TrapableContainer( Serial serial ) : base( serial )
		{
		}

		private void SendMessageTo( Mobile to, int number, int hue )
		{
			if ( Deleted || !to.CanSee( this ) )
			{
				return;
			}

			to.Send( new Network.MessageLocalized( Serial, ItemID, Network.MessageType.Regular, hue, 3, number, "", "" ) );
		}

		private void SendMessageTo( Mobile to, string text, int hue )
		{
			if ( Deleted || !to.CanSee( this ) )
			{
				return;
			}

			to.Send( new Network.UnicodeMessage( Serial, ItemID, Network.MessageType.Regular, hue, 3, "ENU", "", text ) );
		}

		public virtual bool ExecuteTrap( Mobile from )
		{
			if ( !m_Enabled )
			{
				return false;
			}

			if ( m_TrapType != TrapType.None )
			{
				Point3D loc = this.GetWorldLocation();
				Map facet = this.Map;

				if ( from.AccessLevel >= AccessLevel.GameMaster )
				{
					SendMessageTo( from, "That is trapped, but you open it with your godly powers.", 0x3B2 );
					return false;
				}

				if ( this is LockableContainer )
				{
					LockableContainer lc = this as LockableContainer;

					if ( from == lc.Locker )
					{
						SendLocalizedMessageTo( from, 1005380 ); // The chest is locked, but since it's yours, you can open it.

						return false;
					}
				}

				switch ( m_TrapType )
				{
					case TrapType.ExplosionTrap:
						{
							SendMessageTo( from, 502999, 0x3B2 ); // You set off a trap!

							if ( from.InRange( loc, 3 ) )
							{
								AOS.Damage( from, m_TrapPower, 0, 100, 0, 0, 0 );

								// A purple potion explodes in your face!
								from.LocalOverheadMessage( Network.MessageType.Regular, 0x2A, 502378 );
							}

							Effects.SendLocationEffect( loc, facet, 0x36BD, 15, 10 );
							Effects.PlaySound( loc, facet, 0x307 );

							break;
						}
					case TrapType.MagicTrap:
						{
							if ( from.InRange( loc, 1 ) )
							{
								from.Damage( m_TrapPower );
							}
							//AOS.Damage( from, m_TrapPower, 0, 100, 0, 0, 0 );

							Effects.PlaySound( loc, Map, 0x307 );

							Effects.SendLocationEffect( new Point3D( loc.X - 1, loc.Y, loc.Z ), Map, 0x36BD, 15 );
							Effects.SendLocationEffect( new Point3D( loc.X + 1, loc.Y, loc.Z ), Map, 0x36BD, 15 );

							Effects.SendLocationEffect( new Point3D( loc.X, loc.Y - 1, loc.Z ), Map, 0x36BD, 15 );
							Effects.SendLocationEffect( new Point3D( loc.X, loc.Y + 1, loc.Z ), Map, 0x36BD, 15 );

							Effects.SendLocationEffect( new Point3D( loc.X + 1, loc.Y + 1, loc.Z + 11 ), Map, 0x36BD, 15 );

							break;
						}
					case TrapType.DartTrap:
						{
							SendMessageTo( from, 502999, 0x3B2 ); // You set off a trap!

							if ( from.InRange( loc, 3 ) )
							{
								AOS.Damage( from, m_TrapPower, 100, 0, 0, 0, 0 );

								// A dart imbeds itself in your flesh!
								from.LocalOverheadMessage( Network.MessageType.Regular, 0x62, 502380 );
							}

							Effects.PlaySound( loc, facet, 0x223 );

							break;
						}
					case TrapType.PoisonTrap:
						{
							SendMessageTo( from, 502999, 0x3B2 ); // You set off a trap!

							if ( from.InRange( loc, 3 ) )
							{
								AOS.Damage( from, m_TrapPower, 0, 0, 0, 100, 0 );
								from.Poison = Poison.Greater;

								// A poison potion explodes and envelopes you in noxious gas!
								from.LocalOverheadMessage( Network.MessageType.Regular, 0x44, 502379 );
							}

							Effects.SendLocationEffect( loc, facet, 0x113A, 10, 20 );
							Effects.PlaySound( loc, facet, 0x231 );

							break;
						}
				}

				m_TrapType = TrapType.None;
				return true;
			}

			return false;
		}

		public virtual void OnTelekinesis( Mobile from )
		{
			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5022 );
			Effects.PlaySound( Location, Map, 0x1F5 );

			if ( !ExecuteTrap( from ) )
			{
				base.DisplayTo( from );
			}
		}

		public override void Open( Mobile from )
		{
			if ( !ExecuteTrap( from ) )
			{
				base.Open( from );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			writer.Write( (bool) m_Enabled );

			writer.Write( (int) m_TrapPower );
			writer.Write( (int) m_TrapType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
					{
						m_Enabled = reader.ReadBool();

						goto case 1;
					}
				case 1:
					{
						m_TrapPower = reader.ReadInt();

						goto case 0;
					}

				case 0:
					{
						m_TrapType = (TrapType) reader.ReadInt();

						break;
					}
			}
		}
	}
}