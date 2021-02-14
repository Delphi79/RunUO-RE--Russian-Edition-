using System;
using Server;
using Server.Network;
using Server.Engines.Craft;

namespace Server.Items
{
	public enum ShurikenQuality
	{
		Low,
		Regular,
		Exceptional
	}

	[FlipableAttribute( 0x27F7, 0x27AC )]
	public class Shuriken : Item, IUsesRemaining, ICraftable
	{
		private int m_UsesRemaining;
		private ShurikenQuality m_Quality;
		private Mobile m_Crafter;
		private Poison m_Poison;
		private int m_PoisonCharges;

		private string m_Crafter_Name;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public ShurikenQuality Quality
		{
			get{ return m_Quality; }
			set{ m_Quality = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; CheckName(); InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonCharges
		{
			get{ return m_PoisonCharges; }
			set{ m_PoisonCharges = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Poison Poison
		{
			get{ return m_Poison; }
			set{ m_Poison = value; InvalidateProperties(); }
		}

		public bool ShowUsesRemaining{ get{ return true; } set{} }

		[Constructable]
		public Shuriken() : this( 1 )
		{
		}

		public Shuriken( int uses ) : base( 0x27F7 )
		{
			m_UsesRemaining = uses;
		}

		public Shuriken( Serial serial ) : base( serial )
		{
		}

		public void CheckName()
		{
			string name = m_Crafter != null ? m_Crafter.Name : "";

			if ( m_Crafter != null && m_Crafter.Fame >= 10000 )
			{
				string title = m_Crafter.Female ? "Lady" : "Lord";
		
		                name = title + " " + name;			
			}                            

			if ( name != "" )
				m_Crafter_Name = name;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Crafter != null && m_Crafter_Name != null && m_Crafter_Name != "" ) 
				list.Add( 1050043, m_Crafter_Name ); // crafted by ~1_NAME~

			if ( m_Quality == ShurikenQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			if ( m_Poison != null && m_PoisonCharges > 0 )
				list.Add( 1062412 + m_Poison.Level, m_PoisonCharges.ToString() );

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (string)m_Crafter_Name );

			Poison.Serialize( m_Poison, writer );

			writer.Write( m_PoisonCharges );

			writer.Write( (int) m_UsesRemaining );

			writer.Write( m_Crafter );

			writer.WriteEncodedInt( (int) m_Quality );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Crafter_Name = reader.ReadString();

					goto case 0;
				}
				case 0:
				{
					m_Poison = Poison.Deserialize( reader );

					m_PoisonCharges = reader.ReadInt();

					m_UsesRemaining = reader.ReadInt();

					m_Crafter = reader.ReadMobile();

					m_Quality = (ShurikenQuality)reader.ReadEncodedInt();

					break;
				}
			}
		}

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ShurikenQuality)quality;

			if ( makersMark )
				Crafter = from;

			return quality;
		}
	}
}