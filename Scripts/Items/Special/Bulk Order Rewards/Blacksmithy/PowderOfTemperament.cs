using System;
using Server;
using Server.Targeting;
using Server.Engines.Craft;
using Server.Network;

namespace Server.Items
{
	public class PowderOfTemperament : Item, IUsesRemaining
	{
		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set
			{
				m_UsesRemaining = value;
				InvalidateProperties();
			}
		}

		public bool ShowUsesRemaining
		{
			get { return true; }
			set
			{
			}
		}

		public override int LabelNumber { get { return 1049082; } } // powder of temperament

		[Constructable]
		public PowderOfTemperament() : this( 10 )
		{
		}

		[Constructable]
		public PowderOfTemperament( int charges ) : base( 4102 )
		{
			Weight = 1.0;
			Hue = 2419;
			UsesRemaining = charges;
		}

		public PowderOfTemperament( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
					{
						m_UsesRemaining = reader.ReadInt();
						break;
					}
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public virtual void DisplayDurabilityTo( Mobile m )
		{
			LabelToAffix( m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString() ); // Durability
		}

		public override void OnSingleClick( Mobile from )
		{
			DisplayDurabilityTo( from );

			base.OnSingleClick( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.Target = new InternalTarget( this );
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

		private class InternalTarget : Target
		{
			private PowderOfTemperament m_Powder;

			public InternalTarget( PowderOfTemperament powder ) : base( 2, false, TargetFlags.None )
			{
				m_Powder = powder;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Powder.Deleted || m_Powder.UsesRemaining <= 0 )
				{
					from.SendLocalizedMessage( 1049086 ); // You have used up your powder of temperament.
					return;
				}

				if ( targeted is BaseArmor /*&& (DefBlacksmithy.CraftSystem.CraftItems.SearchForSubclass( targeted.GetType() ) != null)*/ )
				{
					BaseArmor ar = (BaseArmor) targeted;

					if ( ar.IsChildOf( from.Backpack ) && m_Powder.IsChildOf( from.Backpack ) )
					{
						int origMaxHP = ar.MaxHitPoints;
						int origCurHP = ar.HitPoints;

						int initMaxHP = Core.AOS ? 255 : ar.InitMaxHits;

						ar.UnscaleDurability();

						if ( ar.MaxHitPoints < initMaxHP )
						{
							int bonus = initMaxHP - ar.MaxHitPoints;

							if ( bonus > 10 )
							{
								bonus = 10;
							}

							ar.MaxHitPoints += bonus;
							ar.HitPoints += bonus;

							ar.ScaleDurability();

							if ( ar.MaxHitPoints > 255 )
							{
								ar.MaxHitPoints = 255;
							}
							if ( ar.HitPoints > 255 )
							{
								ar.HitPoints = 255;
							}

							if ( ar.MaxHitPoints > origMaxHP )
							{
								from.SendLocalizedMessage( 1049084 ); // You successfully use the powder on the item.

								--m_Powder.UsesRemaining;

								if ( m_Powder.UsesRemaining <= 0 )
								{
									from.SendLocalizedMessage( 1049086 ); // You have used up your powder of temperament.
									m_Powder.Delete();
								}
							}
							else
							{
								ar.MaxHitPoints = origMaxHP;
								ar.HitPoints = origCurHP;
								from.SendLocalizedMessage( 1049085 ); // The item cannot be improved any further.
							}
						}
						else
						{
							from.SendLocalizedMessage( 1049085 ); // The item cannot be improved any further.
							ar.ScaleDurability();
						}
					}
					else
					{
						from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
					}
				}
				else if ( targeted is BaseWeapon /*&& (DefBlacksmithy.CraftSystem.CraftItems.SearchForSubclass( targeted.GetType() ) != null)*/ )
				{
					BaseWeapon wep = (BaseWeapon) targeted;

					if ( wep.IsChildOf( from.Backpack ) && m_Powder.IsChildOf( from.Backpack ) )
					{
						int origMaxHP = wep.MaxHits;
						int origCurHP = wep.Hits;

						int initMaxHP = Core.AOS ? 255 : wep.InitMaxHits;

						wep.UnscaleDurability();

						if ( wep.MaxHits < initMaxHP )
						{
							int bonus = initMaxHP - wep.MaxHits;

							if ( bonus > 10 )
							{
								bonus = 10;
							}

							wep.MaxHits += bonus;
							wep.Hits += bonus;

							wep.ScaleDurability();

							if ( wep.MaxHits > 255 )
							{
								wep.MaxHits = 255;
							}
							if ( wep.Hits > 255 )
							{
								wep.Hits = 255;
							}

							if ( wep.MaxHits > origMaxHP )
							{
								from.SendLocalizedMessage( 1049084 ); // You successfully use the powder on the item.

								--m_Powder.UsesRemaining;

								if ( m_Powder.UsesRemaining <= 0 )
								{
									from.SendLocalizedMessage( 1049086 ); // You have used up your powder of temperament.
									m_Powder.Delete();
								}
							}
							else
							{
								wep.MaxHits = origMaxHP;
								wep.Hits = origCurHP;
								from.SendLocalizedMessage( 1049085 ); // The item cannot be improved any further.
							}
						}
						else
						{
							from.SendLocalizedMessage( 1049085 ); // The item cannot be improved any further.
							wep.ScaleDurability();
						}
					}
					else
					{
						from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
					}
				}
				else
				{
					from.SendLocalizedMessage( 1049083 ); // You cannot use the powder on that item.
				}
			}
		}
	}
}