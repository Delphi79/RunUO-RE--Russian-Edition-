using System;
using System.Collections;
using Server;

namespace Server.Mobiles
{
	public class TailorGuildmaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild { get { return NpcGuild.TailorsGuild; } }

		[Constructable]
		public TailorGuildmaster() : base( "tailor" )
		{
			SetSkill( SkillName.Tailoring, 90.0, 100.0 );
		}

		public TailorGuildmaster( Serial serial ) : base( serial )
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