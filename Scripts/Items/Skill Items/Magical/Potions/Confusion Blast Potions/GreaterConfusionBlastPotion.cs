using System;
using Server;

namespace Server.Items
{
	public class GreaterConfusionBlastPotion : BaseConfusionBlastPotion
	{
		public override int AreaSize{ get{ return 7; } }

		public override int LabelNumber{ get{ return 1072108; } } // a Greater Confusion Blast potion

		[Constructable]
		public GreaterConfusionBlastPotion() : base( PotionEffect.ConfusionBlastGreater )
		{
		}

		public GreaterConfusionBlastPotion( Serial serial ) : base( serial )
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