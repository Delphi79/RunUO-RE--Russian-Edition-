using System;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items
{
	public abstract class BaseRanged : BaseMeleeWeapon
	{
		public abstract int EffectID { get; }
		public abstract Type AmmoType { get; }
		public abstract Item Ammo { get; }

		public override int DefHitSound { get { return 0x234; } }
		public override int DefMissSound { get { return 0x238; } }

		public override SkillName DefSkill { get { return SkillName.Archery; } }
		public override WeaponType DefType { get { return WeaponType.Ranged; } }
		public override WeaponAnimation DefAnimation { get { return WeaponAnimation.ShootXBow; } }

		public override SkillName AccuracySkill { get { return SkillName.Archery; } }

		public BaseRanged( int itemID ) : base( itemID )
		{
		}

		public BaseRanged( Serial serial ) : base( serial )
		{
		}

		public override TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
			// Make sure we've been standing still for one second
			if ( DateTime.Now > (attacker.LastMoveTime + TimeSpan.FromSeconds( Core.AOS ? 0.25 : 1.0 )) || (Core.AOS && WeaponAbility.GetCurrentAbility( attacker ) is MovingShot) )
			{
				bool canSwing = true;

				if ( Core.AOS )
				{
					canSwing = (!attacker.Paralyzed && !attacker.Frozen);

					if ( canSwing )
					{
						Spell sp = attacker.Spell as Spell;

						canSwing = (sp == null || !sp.IsCasting || !sp.BlocksMovement);
					}
				}

				if ( canSwing && attacker.HarmfulCheck( defender ) )
				{
					attacker.DisruptiveAction();
					attacker.Send( new Swing( 0, attacker, defender ) );

					bool doubleS = false;
					WeaponAbility ability = WeaponAbility.GetCurrentAbility( attacker );
					if ( ability is DoubleShot && ((int) (Math.Max( attacker.Skills[ SkillName.Bushido ].Value, attacker.Skills[ SkillName.Ninjitsu ].Value ))) >= Utility.Random( 130 ) )
					{
						doubleS = true;
					}

					if ( OnFired( attacker, defender ) )
					{
						if ( CheckHit( attacker, defender ) )
						{
							OnHit( attacker, defender );
						}
						else
						{
							OnMiss( attacker, defender );
						}

						if ( doubleS )
						{
							OnSwing( attacker, defender );
						}
					}
				}

				return GetDelay( attacker );
			}
			else
			{
				return TimeSpan.FromSeconds( 0.25 );
			}
		}

		public override void OnHit( Mobile attacker, Mobile defender )
		{
			if ( attacker.Player && !defender.Player && (defender.Body.IsAnimal || defender.Body.IsMonster) && 0.4 >= Utility.RandomDouble() )
			{
				defender.AddToBackpack( Ammo );
			}

			base.OnHit( attacker, defender );
		}

		public class RecoveryTimer : Timer
		{
			private Mobile a;
			private Item Ammo;

			public void RecoveryAmmo( Mobile attacker, Item ammo )
			{
				int number = 0;
			
				if ( ammo is Arrow )
					number = 1023903;
				else if ( ammo is Bolt )
					number = 1027163;

				string arguments = String.Format( "{0}	#{1}", ammo.Amount, number );

				if ( attacker.AddToBackpack( ammo ) )
				{
					attacker.SendLocalizedMessage( 1073504, arguments ); // You recover ~1_NUM~ ~2_AMMO~.
				}
				else
				{
					attacker.SendLocalizedMessage( 1073559, arguments  ); // You attempt to recover ~1_NUM~ ~2_AMMO~, but there is no room in your backpack, and they are lost.
				}
			}

			public RecoveryTimer( Mobile attacker, Item ammo ) : base( TimeSpan.FromSeconds( 5.0 ) )
			{
				a = attacker;

				Ammo = ammo;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				RecoveryAmmo( a, Ammo );

				Stop();
			}
		}

		public override void OnMiss( Mobile attacker, Mobile defender )
		{
			if ( attacker.Player/* && 0.4 >= Utility.RandomDouble()*/ )
			{
				RecoveryTimer timer = new RecoveryTimer( attacker, Ammo );

				timer.Start();
			}

			base.OnMiss( attacker, defender );
		}

		public virtual bool OnFired( Mobile attacker, Mobile defender )
		{
			Container pack = attacker.Backpack;

			if ( attacker.Player && (pack == null || !pack.ConsumeTotal( AmmoType, 1 )) )
			{
				return false;
			}

			attacker.MovingEffect( defender, EffectID, 18, 1, false, false );

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				case 1:
					{
						break;
					}
				case 0:
					{
						/*m_EffectID =*/
						reader.ReadInt();
						break;
					}
			}

			if ( version < 2 )
			{
				WeaponAttributes.MageWeapon = 0;
				WeaponAttributes.UseBestSkill = 0;
			}
		}
	}
}