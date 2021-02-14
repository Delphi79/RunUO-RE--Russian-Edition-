using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class GlacialStaff : BlackStaff
	{
		public override int LabelNumber{ get{ return 1017413; } } // Glacial Staff

		private int m_phys_resist;

		[Constructable]
		public GlacialStaff()
		{
			Hue = 0x480;
			WeaponAttributes.HitHarm = Utility.RandomList( 5, 10, 15, 20 ); 
			WeaponAttributes.MageWeapon = Utility.RandomMinMax( 5, 10 );

			m_phys_resist = Utility.RandomMinMax( 50, 100 ); // TODO: Verify
		}

		public GlacialStaff( Serial serial ) : base( serial )
		{
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy )
		{
			fire = pois = nrgy = 0;

			phys = m_phys_resist;

			cold = 100 - phys;
		}

		// TODO: Pre-AOS work of glacial staff

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( m_phys_resist );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1)
				m_phys_resist = Utility.RandomMinMax( 50, 100 ); // TODO: Verify

			switch( version )
			{
				case 1:
				{
					m_phys_resist = reader.ReadInt();

					goto case 0;
				}
				case 0:
				{
					break;
				}	
			}                     

			if ( WeaponAttributes.MageWeapon < 5 || WeaponAttributes.MageWeapon > 10 )
				WeaponAttributes.MageWeapon = Utility.RandomMinMax( 5, 10 ); // TODO: verify
		}
	}
}