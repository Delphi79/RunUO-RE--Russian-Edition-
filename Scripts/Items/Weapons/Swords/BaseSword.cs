using System;
using Server;
using Server.Items;
using Server.Targets;

namespace Server.Items
{
	public abstract class BaseSword : BaseMeleeWeapon
	{
		public override SkillName DefSkill { get { return SkillName.Swords; } }
		public override WeaponType DefType { get { return WeaponType.Slashing; } }
		public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Slash1H; } }

		public BaseSword( int itemID ) : base( itemID )
		{
		}

		public BaseSword( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			from.SendLocalizedMessage( 1010018 ); // What do you want to use this item on?

			from.Target = new BladedItemTarget( this );
		}

		public override void OnHit( Mobile attacker, Mobile defender )
		{
			base.OnHit( attacker, defender );

			if ( !Core.AOS && Poison != null && PoisonCharges > 0 )
			{
				--PoisonCharges;

				if ( Utility.RandomDouble() >= 0.5 ) // 50% chance to poison
				{
					defender.ApplyPoison( attacker, Poison );
				}
			}
		}
	}
}