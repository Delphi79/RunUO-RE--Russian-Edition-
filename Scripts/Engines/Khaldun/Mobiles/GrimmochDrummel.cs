using System;
using Server.Items;

namespace Server.Mobiles
{
	public class GrimmochDrummel : BaseCreature
	{
		public override bool ClickTitle { get { return false; } }
		public override bool ShowFameTitle { get { return false; } }
		public override bool DeleteCorpseOnDeath { get { return true; } }

		[Constructable]
		public GrimmochDrummel() : base( AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Title = "the Cursed";

			Hue = 0x8596;
			Body = 0x190;
			Name = "Grimmoch Drummel";

			Item hair = new KrisnaHair();
			hair.Movable = false;
			AddItem( hair );

			Bow bow = new Bow();
			bow.Movable = false;
			AddItem( bow );

			AddItem( new Boots( 0x8A4 ) );
			AddItem( new BodySash( 0x8A4 ) );

			Backpack backpack = new Backpack();
			backpack.Movable = false;
			AddItem( backpack );

			LeatherGloves gloves = new LeatherGloves();
			LeatherChest chest = new LeatherChest();
			gloves.Hue = 0x96F;
			chest.Hue = 0x96F;

			AddItem( gloves );
			AddItem( chest );

			SetStr( 111, 120 );
			SetDex( 151, 160 );
			SetInt( 41, 50 );

			SetHits( 180, 207 );
			SetMana( 0 );

			SetDamage( 13, 16 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 45, 55 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 20, 25 );

			SetSkill( SkillName.Archery, 90.1, 110.0 );
			SetSkill( SkillName.Swords, 60.1, 70.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 60.1, 70.0 );
			SetSkill( SkillName.Anatomy, 90.1, 100.0 );

			Fame = 5000;
			Karma = -1000;

			PackItem( new Arrow( 40 ) );

			if ( 3 > Utility.Random( 100 ) )
			{
				PackItem( new FireHorn() );
			}

			if ( 1 > Utility.Random( 3 ) )
			{
				PackItem( Loot.RandomGrimmochJournal() );
			}
		}

		public override int GetIdleSound()
		{
			return 0x178;
		}

		public override int GetAngerSound()
		{
			return 0x1AC;
		}

		public override int GetDeathSound()
		{
			return 0x27E;
		}

		public override int GetHurtSound()
		{
			return 0x177;
		}

		public override bool OnBeforeDeath()
		{
			Gold gold = new Gold( Utility.RandomMinMax( 190, 230 ) );
			gold.MoveToWorld( Location, Map );

			Container pack = this.Backpack;
			if ( pack != null )
			{
				pack.Movable = true;
				pack.MoveToWorld( Location, Map );
			}

			Effects.SendLocationEffect( Location, Map, 0x376A, 10, 1 );
			return true;
		}

		public override bool AlwaysMurderer { get { return true; } }

		public GrimmochDrummel( Serial serial ) : base( serial )
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
	}
}