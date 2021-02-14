using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Gumps;

namespace Server.Mobiles
{
	public enum AdvancedCharacterState
	{
		None,
		InUse
	}

	public class AdvancedCharacter
	{
		public static bool HasRequest( Mobile m )
		{
			PlayerMobile from = m as PlayerMobile;

			if ( from != null && from.ACState == AdvancedCharacterState.InUse )
			{
				return true;
			}

			return false;
		}

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler( OnLogin );
		}

		private static void OnLogin( LoginEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( HasRequest( from ) )
			{
				from.SendGump( new AdvancedCharacterChoiceGump() );
			}
		}
	}

	public class AdvancedCharacterChoiceGump : Gump
	{
		public AdvancedCharacterChoiceGump() : base( 200, 27 )
		{
			Closable = true;

			AddPage( 0 );

			AddBackground( 0, 0, 267, 44, 0x13BE );

			AddImageTiled( 8, 10, 250, 24, 0xA40 );

			AddAlphaRegion( 8, 10, 250, 24 );

			AddButton( 10, 14, 0x845, 0x846, 1, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 34, 12, 233, 24, 1073365, 0xFFFFFF, false, false ); // Choose Advanced Character Template
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{		
			Mobile from = state.Mobile;

			switch( info.ButtonID )
			{
				case 1:
				{
					if ( from.SkillsTotal > 2000 )
					{
						from.SendGump( new AdvanceCharacterLimitGump() );
					}
					else
					{
						from.SendGump( new AdvancedCharacterMenuGump() );
					}

					break;
				}
				case 0:
				{
					from.SendGump( new AdvancedCharacterChoiceGump() );

					break;
				}
			}
		}
	}

	public class AdvancedCharacterMenuGump : Gump
	{
		public AdvancedCharacterMenuGump() : base( 60, 36 )
		{
			AddPage( 0 );

			AddBackground( 0, 0, 520, 404, 0x13BE );

			AddImageTiled( 10, 10, 500, 20, 0xA40 );

			AddImageTiled( 10, 40, 500, 324, 0xA40 );

			AddImageTiled( 10, 374, 500, 20, 0xA40 );

			AddAlphaRegion( 10, 10, 500, 384 );

			AddButton( 10, 374, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 45, 376, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL

			AddHtmlLocalized( 14, 12, 500, 20, 1073366, 0x7FFF, false, false ); // Choose a template:

			AddPage( 1 );

			AddButtonTileArt( 14, 44, 0x918, 0x919, GumpButtonType.Reply, 0, 100, 10923, 0, 0, 0 );
			AddToolTip( 1073844 ); // <body>85 Bushido<br>85 Swordsmanship<br>85 Parrying<br>70 Tactics<br>70 Focus<br>85 Strength<br>85 Dexterity<br>55 Intelligence</body>
			AddHtmlLocalized( 98, 44, 250, 60, 1073287, 0x7FFF, false, false ); // Samurai

			AddButtonTileArt( 264, 44, 0x918, 0x919, GumpButtonType.Reply, 0, 101, 10924, 0, 0, 0 );
			AddToolTip( 1073845 ); // <body>85 Ninjitsu<br>85 Hiding<br>85 Stealth<br>70 Fencing<br>70 Tactics<br>85 Strength<br>85 Dexterity<br>55 Intelligence</body>
			AddHtmlLocalized( 348, 44, 250, 60, 1073288, 0x7FFF, false, false ); // Ninja

			AddButtonTileArt( 14, 108, 0x918, 0x919, GumpButtonType.Reply, 0, 102, 10925, 0, 0, 0 );
			AddToolTip( 1073846 ); // <body>85 Necromancy<br>85 Spirit Speak<br>85 Resisting Spells<br>70 Fencing<br>70 Meditation<br>80 Strength<br>70 Dexterity<br>75 Intelligence</body>
			AddHtmlLocalized( 98, 108, 250, 60, 1073289, 0x7FFF, false, false ); // Necromancer

			AddButtonTileArt( 264, 108, 0x918, 0x919, GumpButtonType.Reply, 0, 103, 10926, 0, 0, 0 );
			AddToolTip( 1073847 ); // <body>85 Chivalry<br>85 Resist Spells<br>85 Swordsmanship<br>70 Tactics<br>60 Focus<br>85 Strength<br>85 Dexterity<br>55 Intelligence<br>Title set to trustworthy</body>
			AddHtmlLocalized( 348, 108, 250, 60, 1073290, 0x7FFF, false, false ); // Paladin

			AddButtonTileArt( 14, 172, 0x918, 0x919, GumpButtonType.Reply, 0, 104, 10927, 0, 0, 0 );
			AddToolTip( 1073848 ); // <body>85 Lockpicking<br>85 Cartography<br>85 Peacemaking<br>70 Magery<br>70 Musicianship<br>100 Strength<br>25 Dexterity<br>100 Intelligence</body>
			AddHtmlLocalized( 98, 172, 250, 60, 1073291, 0x7FFF, false, false ); // Treasure Hunter

			AddButtonTileArt( 264, 172, 0x918, 0x919, GumpButtonType.Reply, 0, 105, 10928, 0, 0, 0 );
			AddToolTip( 1073849 ); // <body>85 Magery<br>85 Resist Spells<br>85 Evaluate Intelligence<br>70 Inscription<br>70 Meditation<br>80 Strength<br>35 Dexterity<br>110 Intelligence</body>
			AddHtmlLocalized( 348, 172, 250, 60, 1073292, 0x7FFF, false, false ); // Mage

			AddButtonTileArt( 14, 236, 0x918, 0x919, GumpButtonType.Reply, 0, 106, 10929, 0, 0, 0 );
			AddToolTip( 1073850 ); // <body>85 Swordsmanship<br>85 Tactics<br>85 Anatomy<br>70 Healing<br>70 Resist Spells<br>110 Strength<br>80 Dexterity<br>25 Intelligence</body>
			AddHtmlLocalized( 98, 236, 250, 60, 1073293, 0x7FFF, false, false ); // Swordsman

			AddButtonTileArt( 264, 236, 0x918, 0x919, GumpButtonType.Reply, 0, 107, 10930, 0, 0, 0 );
			AddToolTip( 1073851 ); // <body>85 Fencing<br>85 Tactics<br>85 Anatomy<br>70 Healing<br>70 Resist Spells<br>110 Strength<br>80 Dexterity<br>25 Intelligence</body>
			AddHtmlLocalized( 348, 236, 250, 60, 1073294, 0x7FFF, false, false ); // Fencer

			AddButtonTileArt( 14, 300, 0x918, 0x919, GumpButtonType.Reply, 0, 108, 10931, 0, 0, 0 );
			AddToolTip( 1073852 ); // <body>85 Mace Fighting<br>85 Tactics<br>85 Anatomy<br>70 Healing<br>70 Resist Spells<br>110 Strength<br>80 Dexterity<br>25 Intelligence</body>
			AddHtmlLocalized( 98, 300, 250, 60, 1073295, 0x7FFF, false, false ); // Macer

			AddButtonTileArt( 264, 300, 0x918, 0x919, GumpButtonType.Reply, 0, 109, 10932, 0, 0, 0 );
			AddToolTip( 1073853 ); // <body>85 Archery<br>85 Tactics<br>85 Anatomy<br>70 Healing<br>70 Resist Spells<br>110 Strength<br>80 Dexterity<br>25 Intelligence</body>
			AddHtmlLocalized( 348, 300, 250, 60, 1073296, 0x7FFF, false, false ); // Archer

			AddButton( 400, 374, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 2 );
			AddHtmlLocalized( 440, 376, 60, 20, 1043353, 0x7FFF, false, false ); // Next

			AddPage( 2 );

			AddButton( 300, 374, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1 );
			AddHtmlLocalized( 340, 376, 60, 20, 1011393, 0x7FFF, false, false ); // Back

			AddButtonTileArt( 14, 44, 0x918, 0x919, GumpButtonType.Reply, 0, 110, 10933, 0, 0, 0 );
			AddToolTip( 1073854 ); // <body>85 Blacksmithing<br>85 Mining<br>85 Armslore<br>70 Tinkering<br>70 Tailoring<br>100 Strength<br>90 Dexterity<br>25 Intelligence</body>
			AddHtmlLocalized( 98, 44, 250, 60, 1073297, 0x7FFF, false, false ); // Blacksmith

			AddButtonTileArt( 264, 44, 0x918, 0x919, GumpButtonType.Reply, 0, 111, 10934, 0, 0, 0 );
			AddToolTip( 1073855 ); // <body>85 Provocation<br>85 Musicianship<br>85 Peacemaking<br>70 Discordance<br>70 Resist Spells<br>85 Strength<br>85 Dexterity<br>55 Intelligence</body>
			AddHtmlLocalized( 348, 44, 250, 60, 1073298, 0x7FFF, false, false ); // Bard

			AddButtonTileArt( 14, 108, 0x918, 0x919, GumpButtonType.Reply, 0, 112, 10935, 0, 0, 0 );
			AddToolTip( 1073856 ); // <body>85 Taming<br>85 Animal Lore<br>85 Veterinary<br>70 Magery<br>70 Meditation<br>80 Strength<br>55 Dexterity<br>80 Intelligence</body>
			AddHtmlLocalized( 98, 108, 250, 60, 1073299, 0x7FFF, false, false ); // Tamer
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			PlayerMobile pm = from as PlayerMobile;

			switch( info.ButtonID )
			{
				case 0:
				{
					from.SendGump( new AdvancedCharacterChoiceGump() );

					break;
				}
				case 100: // Samurai
				{
					from.Skills[SkillName.Bushido].Base = 85.0;
					from.Skills[SkillName.Swords].Base = 85.0;
					from.Skills[SkillName.Parry].Base = 85.0;
					from.Skills[SkillName.Tactics].Base = 70.0;
					from.Skills[SkillName.Focus].Base = 70.0;

					from.RawStr = 85;
					from.RawDex = 55;
					from.RawInt = 55;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 101: // Ninja
				{
					from.Skills[SkillName.Ninjitsu].Base = 85.0;
					from.Skills[SkillName.Hiding].Base = 85.0;
					from.Skills[SkillName.Stealth].Base = 85.0;
					from.Skills[SkillName.Fencing].Base = 70.0;
					from.Skills[SkillName.Tactics].Base = 70.0;

					from.RawStr = 85;
					from.RawDex = 85;
					from.RawInt = 55;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 102: // Necromancer
				{
					from.Skills[SkillName.Necromancy].Base = 85.0;
					from.Skills[SkillName.SpiritSpeak].Base = 85.0;
					from.Skills[SkillName.MagicResist].Base = 85.0;
					from.Skills[SkillName.Fencing].Base = 70.0;
					from.Skills[SkillName.Meditation].Base = 70.0;

					from.RawStr = 80;
					from.RawDex = 70;
					from.RawInt = 75;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 103: // Paladin
				{
					from.Skills[SkillName.Chivalry].Base = 85.0;
					from.Skills[SkillName.MagicResist].Base = 85.0;
					from.Skills[SkillName.Swords].Base = 85.0;
					from.Skills[SkillName.Tactics].Base = 70.0;
					from.Skills[SkillName.Focus].Base = 60.0;

					from.RawStr = 85;
					from.RawDex = 85;
					from.RawInt = 55;

					from.Karma = 10000;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 104: // Treasure Hunter
				{
					from.Skills[SkillName.Lockpicking].Base = 85.0;
					from.Skills[SkillName.Cartography].Base = 85.0;
					from.Skills[SkillName.Peacemaking].Base = 85.0;
					from.Skills[SkillName.Magery].Base = 70.0;
					from.Skills[SkillName.Musicianship].Base = 70.0;

					from.RawStr = 100;
					from.RawDex = 25;
					from.RawInt = 100;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 105: // Mage
				{
					from.Skills[SkillName.Magery].Base = 85.0;
					from.Skills[SkillName.MagicResist].Base = 85.0;
					from.Skills[SkillName.EvalInt].Base = 85.0;
					from.Skills[SkillName.Inscribe].Base = 70.0;
					from.Skills[SkillName.Meditation].Base = 70.0;

					from.RawStr = 80;
					from.RawDex = 35;
					from.RawInt = 110;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 106: // Swordsman
				{
					from.Skills[SkillName.Swords].Base = 85.0;
					from.Skills[SkillName.Tactics].Base = 85.0;
					from.Skills[SkillName.Anatomy].Base = 85.0;
					from.Skills[SkillName.Healing].Base = 70.0;
					from.Skills[SkillName.MagicResist].Base = 70.0;

					from.RawStr = 110;
					from.RawDex = 80;
					from.RawInt = 25;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 107: // Fencer
				{
					from.Skills[SkillName.Fencing].Base = 85.0;
					from.Skills[SkillName.Tactics].Base = 85.0;
					from.Skills[SkillName.Anatomy].Base = 85.0;
					from.Skills[SkillName.Healing].Base = 70.0;
					from.Skills[SkillName.MagicResist].Base = 70.0;

					from.RawStr = 110;
					from.RawDex = 80;
					from.RawInt = 25;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 108: // Macer
				{
					from.Skills[SkillName.Macing].Base = 85.0;
					from.Skills[SkillName.Tactics].Base = 85.0;
					from.Skills[SkillName.Anatomy].Base = 85.0;
					from.Skills[SkillName.Healing].Base = 70.0;
					from.Skills[SkillName.MagicResist].Base = 70.0;

					from.RawStr = 110;
					from.RawDex = 80;
					from.RawInt = 25;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 109: // Archer
				{
					from.Skills[SkillName.Archery].Base = 85.0;
					from.Skills[SkillName.Tactics].Base = 85.0;
					from.Skills[SkillName.Anatomy].Base = 85.0;
					from.Skills[SkillName.Healing].Base = 70.0;
					from.Skills[SkillName.MagicResist].Base = 70.0;

					from.RawStr = 110;
					from.RawDex = 80;
					from.RawInt = 25;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 110: // Blacksmith
				{
					from.Skills[SkillName.Blacksmith].Base = 85.0;
					from.Skills[SkillName.Mining].Base = 85.0;
					from.Skills[SkillName.ArmsLore].Base = 85.0;
					from.Skills[SkillName.Tinkering].Base = 70.0;
					from.Skills[SkillName.Tailoring].Base = 70.0;

					from.RawStr = 100;
					from.RawDex = 90;
					from.RawInt = 25;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 111: // Bard
				{
					from.Skills[SkillName.Provocation].Base = 85.0;
					from.Skills[SkillName.Musicianship].Base = 85.0;
					from.Skills[SkillName.Peacemaking].Base = 85.0;
					from.Skills[SkillName.Discordance].Base = 70.0;
					from.Skills[SkillName.MagicResist].Base = 70.0;

					from.RawStr = 85;
					from.RawDex = 85;
					from.RawInt = 55;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
				case 112: // Tamer
				{
					from.Skills[SkillName.AnimalTaming].Base = 85.0;
					from.Skills[SkillName.AnimalLore].Base = 85.0;
					from.Skills[SkillName.Veterinary].Base = 85.0;
					from.Skills[SkillName.Magery].Base = 70.0;
					from.Skills[SkillName.Meditation].Base = 70.0;

					from.RawStr = 80;
					from.RawDex = 55;
					from.RawInt = 80;

					pm.ACState = AdvancedCharacterState.None;

					break;
				}
			}	
		}
	}

	public class AdvanceCharacterLimitGump : Gump
	{
		public AdvanceCharacterLimitGump() : base( 100, 100 )
		{
			AddPage( 0 );

			AddBackground( 0, 0, 440, 273, 0x2422 );

			AddHtmlLocalized( 15, 15, 410, 213, 1073842, 0x0, true, false ); // You cannot choose an advancement template for this character because you have over 200 skill points. If you do not have a way to lower your skill point total, please page a GM for assistance.

			AddButton( 210, 233, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 245, 235, 160, 20, 1073843, 0x7FFF, false, false ); // 
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			from.SendGump( new AdvancedCharacterChoiceGump() );
		}
	}

	public class AdvancedCharacterWarningGump : Gump
	{
		private PromotionalToken token;

		public AdvancedCharacterWarningGump( PromotionalToken m_token ) : base( 100, 100 )
		{
			token = m_token;

			AddPage( 0 );

			AddBackground( 0, 0, 440, 273, 0x2422 );

			AddHtmlLocalized( 15, 15, 410, 213, 1073816, 0x0, true, false ); // This character has over 200 skill points. You WILL NOT be able to choose an advanced character template until you reduce your skill point total (for example, by storing a skill using a soulstone). You may redeem this token anyway, or you can cancel this gump and trade your token to another character.<br><br>For more information about the advancement program, <a href="http://uo.custhelp.com/cgi-bin/uo.cfg/php/enduser/std_adp.php?p_faqid=13270">review this article</a> or <a href="http://uo.custhelp.com/cgi-bin/uo.cfg/php/enduser/ask.php?">ask a question</a> at our knowledge base.

			AddButton( 275, 233, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 310, 235, 160, 20, 1073817, 0x7FFF, false, false ); // Redeem It Anyway!

			AddButton( 15, 233, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 50, 235, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			switch( info.ButtonID )
			{
				case 1:
				{
					from.SendGump( new AdvancedCharacterChoiceGump() );

					PlayerMobile pm = from as PlayerMobile;

					pm.ACState = AdvancedCharacterState.InUse;

					token.Delete();

					break;
				}
				case 0:
				{
					break;
				}
			}
		}
	}
}