using System;
using Server;
using Server.Mobiles;
using Server.Engines.Quests;

namespace Server.Engines.Quests.Haven
{
	public class QuestFertileDirt : QuestItem
	{
		[Constructable]
		public QuestFertileDirt() : base( 0xF81 )
		{
			Weight = 1.0;
		}

		public QuestFertileDirt( Serial serial ) : base( serial )
		{
		}

		public override bool CanDrop( PlayerMobile player )
		{
			UzeraanTurmoilQuest qs = player.Quest as UzeraanTurmoilQuest;

			if ( qs == null )
			{
				return true;
			}

			return !qs.IsObjectiveInProgress( typeof( ReturnFertileDirtObjective ) );
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