using System;
using System.Collections;
using Server;

namespace Server.Mobiles
{
	public class Cobbler : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos { get { return m_SBInfos; } }

		[Constructable]
		public Cobbler() : base( "the cobbler" )
		{
			SetSkill( SkillName.Tailoring, 60.0, 83.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBCobbler() );
		}

		public override VendorShoeType ShoeType { get { return Utility.RandomBool() ? VendorShoeType.Sandals : VendorShoeType.Shoes; } }

		public Cobbler( Serial serial ) : base( serial )
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