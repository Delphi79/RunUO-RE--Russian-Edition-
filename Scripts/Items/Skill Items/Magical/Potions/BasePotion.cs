using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public enum PotionEffect
	{
		Nightsight,
		CureLesser,
		Cure,
		CureGreater,
		Agility,
		AgilityGreater,
		Strength,
		StrengthGreater,
		PoisonLesser,
		Poison,
		PoisonGreater,
		PoisonDeadly,
		Refresh,
		RefreshTotal,
		HealLesser,
		Heal,
		HealGreater,
		ExplosionLesser,
		Explosion,
		ExplosionGreater,
		Conflagration,
		ConflagrationGreater,
		MaskOfDeath,
		MaskOfDeathGreater,
		ConfusionBlast,
		ConfusionBlastGreater
	}

	public abstract class BasePotion : Item, ICraftable
	{
		private PotionEffect m_PotionEffect;

		public PotionEffect PotionEffect
		{
			get
			{
				return m_PotionEffect;
			}
			set
			{
				m_PotionEffect = value;
				InvalidateProperties();
			}
		}

		public override int LabelNumber{ get{ return 1041314 + (int)m_PotionEffect; } }

		public BasePotion( int itemID, PotionEffect effect ) : base( itemID )
		{
			m_PotionEffect = effect;

			Stackable = false;
			Weight = 1.0;
		}

		public BasePotion( Serial serial ) : base( serial )
		{
		}

		public virtual bool RequireFreeHand{ get{ return true; } }

		public virtual TimeSpan GetNextDrinkTime( Mobile from )
		{
			return TimeSpan.Zero;
		}

		public virtual void SetNextDrinkTime( Mobile from )
		{
		}

		public static bool HasFreeHand( Mobile m )
		{
			Item handOne = m.FindItemOnLayer( Layer.OneHanded );
			Item handTwo = m.FindItemOnLayer( Layer.TwoHanded );

			if ( handTwo is BaseWeapon )
				handOne = handTwo;

			return ( handOne == null || handTwo == null );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				if ( !RequireFreeHand || HasFreeHand( from ) )
				{       				
					if ( this is BaseMaskOfDeathPotion )
					{
						if ( BaseMaskOfDeathPotion.UnderEffect( from ) )
						{
							from.SendLocalizedMessage( 502173 ); // You are already under a similar effect.

							return;
						}
					}

					TimeSpan ts = GetNextDrinkTime( from );
				
					int totalSeconds = (int)ts.TotalSeconds;
					int totalMinutes = 0;
					int totalHours = 0;

					if ( totalSeconds >= 60 )
						totalMinutes = (totalSeconds + 59) / 60;

					if ( totalMinutes >= 60 )
						totalHours = (totalSeconds + 3599) / 3600;

					if ( totalHours > 0 )
					{
						from.SendLocalizedMessage( 1072529, String.Format( "{0}	#1072532", totalHours ) );

						return;	
					}
					else if ( totalMinutes > 0 )
					{
						from.SendLocalizedMessage( 1072529, String.Format( "{0}	#1072531", totalMinutes ) );

						return;	
					}
					else if ( totalSeconds > 0 )
					{
						from.SendLocalizedMessage( 1072529, String.Format( "{0}	#1072530", totalSeconds ) );

						return;	
					}	
					else
					{
						Drink( from );

						SetNextDrinkTime( from );
					}
				}
				else
					from.SendLocalizedMessage( 502172 ); // You must have a free hand to drink a potion.
			}
			else
			{
				from.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_PotionEffect );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_PotionEffect = (PotionEffect)reader.ReadInt();
					break;
				}
			}
		}

		public abstract void Drink( Mobile from );

		public static void PlayDrinkEffect( Mobile m )
		{
			m.RevealingAction();

			m.PlaySound( 0x2D6 );
			m.AddToBackpack( new Bottle() );

			if ( m.Body.IsHuman /*&& !m.Mounted*/ )
				m.Animate( 34, 5, 1, true, false, 0 );
		}

		public static TimeSpan Scale( Mobile m, TimeSpan v )
		{
			if ( !Core.AOS )
				return v;

			double scalar = 1.0 + (0.01 * AosAttributes.GetValue( m, AosAttribute.EnhancePotions ));

			return TimeSpan.FromSeconds( v.TotalSeconds * scalar );
		}

		public static double Scale( Mobile m, double v )
		{
			if ( !Core.AOS )
				return v;

			double scalar = 1.0 + (0.01 * AosAttributes.GetValue( m, AosAttribute.EnhancePotions ));

			return v * scalar;
		}

		public static int Scale( Mobile m, int v )
		{
			if ( !Core.AOS )
				return v;

			return AOS.Scale( v, 100 + AosAttributes.GetValue( m, AosAttribute.EnhancePotions ) );
		}

		#region ICraftable Members
		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			if ( craftSystem is DefAlchemy )
			{
				Container pack = from.Backpack;

				if ( pack != null )
				{
					Item[] kegs = pack.FindItemsByType( typeof( PotionKeg ), true );

					for ( int i = 0; i < kegs.Length; ++i )
					{
						PotionKeg keg = kegs[i] as PotionKeg;

						if ( keg == null )
							continue;

						if ( keg.Held <= 0 || keg.Held >= 100 )
							continue;

						if ( keg.Type != PotionEffect )
							continue;

						++keg.Held;

						Delete();
						from.AddToBackpack( new Bottle() );

						return -1; // signal placed in keg
					}
				}
			}

			return 1;
		}
		#endregion
	}
}