using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Targeting;

namespace Server.Items
{
	public abstract class BaseConflagrationPotion : BasePotion
	{
		public abstract int MinDamage { get; }
		
		public abstract int MaxDamage { get; }

		public BaseConflagrationPotion( PotionEffect effect ) : base( 0xF06, effect )
		{
			Hue = 0x489;
		}

		public BaseConflagrationPotion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override TimeSpan GetNextDrinkTime( Mobile from )
		{
			if ( from is PlayerMobile )
				return ((PlayerMobile)from).NextDrinkConflagrationPotion;

			return TimeSpan.Zero;
		}

		public override void SetNextDrinkTime( Mobile from )
		{
			if ( from is PlayerMobile )
				((PlayerMobile)from).NextDrinkConflagrationPotion = TimeSpan.FromSeconds( 30.0 );
		}

		public override void Drink( Mobile from )
		{
			ThrowTarget targ = from.Target as ThrowTarget;

			if ( targ != null && targ.Potion == this )
				return;

			from.RevealingAction();

			from.Target = new ThrowTarget( this );
		}

		private class ThrowTarget : Target
		{
			private BaseConflagrationPotion m_Potion;

			public BaseConflagrationPotion Potion
			{
				get{ return m_Potion; }
			}

			public ThrowTarget( BaseConflagrationPotion potion ) : base( 12, true, TargetFlags.None )
			{
				m_Potion = potion;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Potion.Deleted || m_Potion.Map == Map.Internal )
					return;

				IPoint3D p = targeted as IPoint3D;

				if ( p == null )
					return;

				Map map = from.Map;

				if ( map == null )
					return;

				SpellHelper.GetSurfaceTop( ref p );

				from.RevealingAction();

				IEntity to;

				if ( p is Mobile )
					to = (Mobile)p;
				else
					to = new Entity( Serial.Zero, new Point3D( p ), map );

				Effects.SendMovingEffect( from, to, m_Potion.ItemID & 0x3FFF, 7, 0, false, false, m_Potion.Hue, 0 );
			
				int alchemy_bonus = (int)( ( ( from.Skills[SkillName.Alchemy].Base - 40.0 ) / 20.0 ) * m_Potion.MinDamage );

				if ( alchemy_bonus < 0 )
					alchemy_bonus = 0;

				int damage = Utility.RandomMinMax( m_Potion.MinDamage, m_Potion.MaxDamage ) + alchemy_bonus;			

				m_Potion.Delete();                       

				int val_glow = -2;
				int val_field = -2;
	
	                        for ( int x = p.X - 2; x <= p.X + 2; ++x )
				{
					for ( int y = p.Y - 2; y <= p.Y + 2; ++y )
					{
						Effects.SendPacket( from, from.Map, new HuedEffect( EffectType.FixedXYZ, Serial.Zero, Serial.Zero, 0x376A, new Point3D( x, y, p.Z ), new Point3D( x, y, p.Z ), 9, 10, true, false, 0, 0 ) );

						Effects.PlaySound( new Point3D( x, y, p.Z ), from.Map, 0x20C );

						new FireFieldSpell.InternalItem( 0x37C3, new Point3D( x, y, p.Z ), from, from.Map, TimeSpan.FromSeconds( 10.0 ), val_glow, 0 );		

						new FireFieldSpell.InternalItem( 0x3996, new Point3D( x, y, p.Z ), from, from.Map, TimeSpan.FromSeconds( 10.0 ), val_field, damage );		

						val_glow++;
	
						val_field++;
					}
				}
			}
		}
	}
}