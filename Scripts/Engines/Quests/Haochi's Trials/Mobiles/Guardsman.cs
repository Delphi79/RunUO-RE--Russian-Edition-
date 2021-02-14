using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Engines.Quests.SE
{
	public class Guardsman : BaseQuester
	{
		public override bool ClickTitle { get { return false; } }

		[Constructable]
		public Guardsman() : base( "the Guardsman of Daimyo Haochi" )
		{
		}

		public Guardsman( Serial serial ) : base( serial )
		{
		}

		public override void InitBody()
		{
			InitStats( 100, 100, 25 );

			Hue = 0x8407;

			Body = 0x190;

			Name = NameList.RandomName( "guardsman" );
		}

		public override void InitOutfit()
		{
			AddItem( Loot.RandomSEWeapon() );
			AddItem( new SamuraiTabi( 0x4C2 ) );
			AddItem( new LeatherSuneate() );
			AddItem( new LightPlateJingasa() );
			AddItem( new LeatherDo() );
			AddItem( new LeatherHiroSode() );

			AddItem( new Mohawk( 0x44E ) );
		}

		public override bool NoContextMenu( PlayerMobile pm )
		{
			return true;
		}

		public override void OnTalk( PlayerMobile player, bool contextMenu )
		{
			return;
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