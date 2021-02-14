using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.Quests.SE
{
	public class YoungRonin : BaseCreature
	{
		[Constructable]
		public YoungRonin() : base( AIType.AI_Melee, FightMode.Agressor, 10, 1, 0.2, 0.4 )
		{
			InitStats( 35, 30, 5 );

			Hue = 0x83FF;

			Body = 0x190;

			Name = "a young ronin";

			Item hair = new Item( Utility.RandomList( 0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2049, 0x204A ) );
			hair.Hue = Utility.RandomHairHue();
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem( hair );

			Item beard = new Item( Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D ) );
			beard.Hue = hair.Hue;
			beard.Layer = Layer.FacialHair;
			beard.Movable = false;
			AddItem( beard );

			AddItem( Loot.RandomSEWeapon() );
			AddItem( new SamuraiTabi() );
			AddItem( new LeatherSuneate() );
			AddItem( new LeatherDo() );
			AddItem( new LeatherHiroSode() );
			AddItem( new Bandana() );

			SetSkill( SkillName.Swords, 50.0 );
			SetSkill( SkillName.Tactics, 50.0 );
		}

		public override bool ClickTitle { get { return false; } }

		public override bool PlayerRangeSensitive { get { return false; } }
		public override bool AlwaysMurderer { get { return true; } }

		public YoungRonin( Serial serial ) : base( serial )
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