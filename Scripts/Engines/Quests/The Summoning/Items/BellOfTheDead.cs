using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.Doom
{
	public class BellOfTheDead : Item
	{
		public override int LabelNumber { get { return 1050018; } } // bell of the dead

		[Constructable]
		public BellOfTheDead() : base( 0x91A )
		{
			Hue = 0x835;
			Movable = false;
		}

		private Chyloth m_Chyloth;
		private SkeletalDragon m_Dragon;
		private bool m_Summoning;

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public Chyloth Chyloth { get { return m_Chyloth; } set { m_Chyloth = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public SkeletalDragon Dragon { get { return m_Dragon; } set { m_Dragon = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public bool Summoning { get { return m_Summoning; } set { m_Summoning = value; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( GetWorldLocation(), 2 ) )
			{
				BeginSummon( from );
			}
			else
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			}
		}

		public virtual void BeginSummon( Mobile from )
		{
			if ( m_Chyloth != null && !m_Chyloth.Deleted )
			{
				from.SendLocalizedMessage( 1050010 ); // The ferry man has already been summoned.  There is no need to ring for him again.
			}
			else if ( m_Dragon != null && !m_Dragon.Deleted )
			{
				from.SendLocalizedMessage( 1050017 ); // The ferryman has recently been summoned already.  You decide against ringing the bell again so soon.
			}
			else if ( !m_Summoning )
			{
				m_Summoning = true;

				Effects.PlaySound( GetWorldLocation(), Map, 0x100 );

				Timer.DelayCall( TimeSpan.FromSeconds( 8.0 ), new TimerStateCallback( EndSummon ), from );
			}
		}

		public virtual void EndSummon( object state )
		{
			Mobile from = (Mobile) state;

			if ( m_Chyloth != null && !m_Chyloth.Deleted )
			{
				from.SendLocalizedMessage( 1050010 ); // The ferry man has already been summoned.  There is no need to ring for him again.
			}
			else if ( m_Dragon != null && !m_Dragon.Deleted )
			{
				from.SendLocalizedMessage( 1050017 ); // The ferryman has recently been summoned already.  You decide against ringing the bell again so soon.
			}
			else if ( m_Summoning )
			{
				m_Summoning = false;

				Point3D loc = GetWorldLocation();

				loc.Z -= 16;

				Effects.SendLocationParticles( EffectItem.Create( loc, Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 0, 0, 2023, 0 );
				Effects.PlaySound( loc, Map, 0x1FE );

				m_Chyloth = new Chyloth();

				m_Chyloth.Direction = (Direction) (7 & (4 + (int) from.GetDirectionTo( loc )));
				m_Chyloth.MoveToWorld( loc, Map );

				m_Chyloth.Bell = this;
				m_Chyloth.AngryAt = from;
				m_Chyloth.BeginGiveWarning();
				m_Chyloth.BeginRemove( TimeSpan.FromSeconds( 40.0 ) );
			}
		}

		public BellOfTheDead( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (Mobile) m_Chyloth );
			writer.Write( (Mobile) m_Dragon );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Chyloth = reader.ReadMobile() as Chyloth;
			m_Dragon = reader.ReadMobile() as SkeletalDragon;

			if ( m_Chyloth != null )
			{
				m_Chyloth.Delete();
			}
		}
	}
}