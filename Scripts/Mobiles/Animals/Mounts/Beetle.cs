using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a giant beetle corpse" )]
	public class Beetle : BaseMount
	{
		[Constructable]
		public Beetle() : this( "a giant beetle" )
		{
		}

		public override bool SubdueBeforeTame { get { return true; } } // Must be beaten into submission

		[Constructable]
		public Beetle( string name ) : base( name, 0x317, 0x3EBC, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SetStr( 300 );
			SetDex( 100 );
			SetInt( 500 );

			SetHits( 200 );

			SetDamage( 7, 20 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30, 40 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 80.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			Fame = 4000;
			Karma = -4000;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 29.1;

			Container pack = Backpack;

			if ( pack != null )
			{
				pack.Delete();
			}

			pack = new StrongBackpack();
			pack.Movable = false;

			AddItem( pack );
		}

		public override int GetAngerSound()
		{
			return 0x21D;
		}

		public override int GetIdleSound()
		{
			return 0x21D;
		}

		public override int GetAttackSound()
		{
			return 0x162;
		}

		public override int GetHurtSound()
		{
			return 0x163;
		}

		public override int GetDeathSound()
		{
			return 0x21D;
		}

		public override FoodType FavoriteFood { get { return FoodType.Meat; } }

		public Beetle( Serial serial ) : base( serial )
		{
		}

		#region Pack Animal Methods
		public override bool OnBeforeDeath()
		{
			if ( !base.OnBeforeDeath() )
			{
				return false;
			}

			PackAnimal.CombineBackpacks( this );

			return true;
		}

		public override DeathMoveResult GetInventoryMoveResultFor( Item item )
		{
			return DeathMoveResult.MoveToCorpse;
		}

		public override bool IsSnoop( Mobile from )
		{
			if ( PackAnimal.CheckAccess( this, from ) )
			{
				return false;
			}

			return base.IsSnoop( from );
		}

		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( CheckFeed( from, item ) )
			{
				return true;
			}

			if ( PackAnimal.CheckAccess( this, from ) )
			{
				AddToBackpack( item );
				return true;
			}

			return base.OnDragDrop( from, item );
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override void GetContextMenuEntries( Mobile from, System.Collections.ArrayList list )
		{
			base.GetContextMenuEntries( from, list );

			PackAnimal.GetContextMenuEntries( this, from, list );
		}
		#endregion

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
	}
}