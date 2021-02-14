using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
	public enum PromotionalType
	{
		SoulStone,
		SoulStoneFragment,
		CharacterTransfer,
		SeventhAnniversary,
		AdvancedCharacter,
		None
	}

	public class PromotionalToken : Item
	{
		public override int LabelNumber { get { return 1070997; } } // A promotional token

		private PromotionalType m_Type;

		[CommandProperty( AccessLevel.GameMaster )]
		public PromotionalType Type { get { return m_Type; } set { m_Type = value; } }

		[Constructable]
		public PromotionalToken( PromotionalType type ) : base( 0x2AAA )
		{
			m_Type = type;

			Light = LightType.Circle300;

			LootType = LootType.Blessed;

			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			string args = null;

			switch ( m_Type )
			{
				case PromotionalType.SoulStone:
					args = "Soulstone";
					break;
				case PromotionalType.SoulStoneFragment:
					args = "Soulstone Fragment";
					break;
				case PromotionalType.CharacterTransfer:
					args = "Character Transfer";
					break;
				case PromotionalType.SeventhAnniversary:
					args = "UO Seventh Anniversary";
					break;
				case PromotionalType.AdvancedCharacter:
					args = "Advanced Character";
					break;
			}

			list.Add( 1070998, args ); // Use this to redeem your ~1_PROMO~
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Type == PromotionalType.AdvancedCharacter )
			{
				PlayerMobile pm = from as PlayerMobile;

				if ( pm.ACState == AdvancedCharacterState.InUse )
				{
					pm.SendLocalizedMessage( 1073815 ); // You are already choosing an advanced character template.
					return;
				}

				if ( from.SkillsTotal > 2000 )
				{
					from.SendGump( new AdvancedCharacterWarningGump( this ) );
					return;
				}
			}

			from.SendGump( new PromotionalTokenGump( this, from ) );
		}

		public PromotionalToken( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( (int) m_Type );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Type = (PromotionalType) reader.ReadInt();
		}
	}

	public class PromotionalTokenGump : Gump
	{
		private Mobile from;
		private PromotionalToken token;

		public PromotionalTokenGump( PromotionalToken t, Mobile m ) : base( 10, 10 )
		{
			token = t;

			from = m;

			AddPage( 0 );

			AddBackground( 0, 0, 240, 135, 0x2422 );

			AddHtmlLocalized( 15, 15, 210, 75, 1070972, 0x0, true, false ); // Click "OKAY" to redeem the following promotional item:

			switch ( token.Type )
			{
				case PromotionalType.SoulStone:
					{
						AddHtmlLocalized( 15, 60, 210, 75, 1030903, 0x0, false, false ); // <center>Soulstone</center>

						break;
					}
				case PromotionalType.SoulStoneFragment:
					{
						AddHtmlLocalized( 15, 60, 210, 75, 1070999, 0x0, false, false ); // <center>Soulstone Fragment</center> 

						break;
					}
				case PromotionalType.CharacterTransfer:
					{
						AddHtmlLocalized( 15, 60, 210, 75, 1062785, 0x0, false, false ); // <BODY><CENTER>Character Transfer<CENTER></BODY>

						break;
					}
				case PromotionalType.SeventhAnniversary:
					{
						AddHtmlLocalized( 15, 60, 210, 75, 1062928, 0x0, false, false ); // <CENTER>UO Seventh Anniversary</CENTER>

						break;
					}
				case PromotionalType.AdvancedCharacter:
					{
						AddHtmlLocalized( 15, 60, 210, 75, 1072839, 0x0, false, false ); // <center>Advanced Character</center>

						break;
					}
			}

			AddButton( 160, 95, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0 );

			AddButton( 90, 95, 0xF2, 0xF1, 0, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( info.ButtonID == 1 )
			{
				BankBox bank = from.BankBox;

				if ( bank == null )
				{
					return;
				}

				Accounting.Account acct = from.Account as Accounting.Account;

				switch ( token.Type )
				{
					case PromotionalType.SoulStone:
						{
							SoulStone ss = new SoulStone( acct.Username );

							bank.DropItem( ss );

							from.SendLocalizedMessage( 1070743 ); // A Soulstone has been created in your bank box!

							break;
						}
					case PromotionalType.SoulStoneFragment:
						{
							int offset = Utility.Random( 0, 8 );

							SoulStoneFragment ssf = new SoulStoneFragment( 0x2AA1 + offset, acct.Username );

							bank.DropItem( ssf );

							from.SendLocalizedMessage( 1070976 ); // A soulstone fragment has been created in your bank box.

							break;
						}
					case PromotionalType.AdvancedCharacter:
						{
							pm.SendGump( new AdvancedCharacterChoiceGump() );

							pm.ACState = AdvancedCharacterState.InUse;

							break;
						}
						// TODO: character transfer, seventh anniversary
				}

				token.Delete();
			}
		}
	}
}