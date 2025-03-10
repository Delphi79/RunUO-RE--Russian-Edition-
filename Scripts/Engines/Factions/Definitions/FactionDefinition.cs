using System;

namespace Server.Factions
{
	public class FactionDefinition
	{
		private int m_Sort;

		private int m_HuePrimary;
		private int m_HueSecondary;
		private int m_HueJoin;
		private int m_HueBroadcast;

		private int m_WarHorseBody;
		private int m_WarHorseItem;

		private string m_FriendlyName;
		private string m_Keyword;

		private TextDefinition m_Name;
		private TextDefinition m_PropName;
		private TextDefinition m_Header;
		private TextDefinition m_About;
		private TextDefinition m_CityControl;
		private TextDefinition m_SigilControl;
		private TextDefinition m_SignupName;
		private TextDefinition m_FactionStoneName;
		private TextDefinition m_OwnerLabel;

		private TextDefinition m_GuardIgnore, m_GuardWarn, m_GuardAttack;

		private StrongholdDefintion m_Stronghold;

		private RankDefinition[] m_Ranks;
		private GuardDefinition[] m_Guards;

		public int Sort { get { return m_Sort; } }

		public int HuePrimary { get { return m_HuePrimary; } }
		public int HueSecondary { get { return m_HueSecondary; } }
		public int HueJoin { get { return m_HueJoin; } }
		public int HueBroadcast { get { return m_HueBroadcast; } }

		public int WarHorseBody { get { return m_WarHorseBody; } }
		public int WarHorseItem { get { return m_WarHorseItem; } }

		public string FriendlyName { get { return m_FriendlyName; } }
		public string Keyword { get { return m_Keyword; } }

		public TextDefinition Name { get { return m_Name; } }
		public TextDefinition PropName { get { return m_PropName; } }
		public TextDefinition Header { get { return m_Header; } }
		public TextDefinition About { get { return m_About; } }
		public TextDefinition CityControl { get { return m_CityControl; } }
		public TextDefinition SigilControl { get { return m_SigilControl; } }
		public TextDefinition SignupName { get { return m_SignupName; } }
		public TextDefinition FactionStoneName { get { return m_FactionStoneName; } }
		public TextDefinition OwnerLabel { get { return m_OwnerLabel; } }

		public TextDefinition GuardIgnore { get { return m_GuardIgnore; } }
		public TextDefinition GuardWarn { get { return m_GuardWarn; } }
		public TextDefinition GuardAttack { get { return m_GuardAttack; } }

		public StrongholdDefintion Stronghold { get { return m_Stronghold; } }

		public RankDefinition[] Ranks { get { return m_Ranks; } }
		public GuardDefinition[] Guards { get { return m_Guards; } }

		public FactionDefinition( int sort, int huePrimary, int hueSecondary, int hueJoin, int hueBroadcast, int warHorseBody, int warHorseItem, string friendlyName, string keyword, TextDefinition name, TextDefinition propName, TextDefinition header, TextDefinition about, TextDefinition cityControl, TextDefinition sigilControl, TextDefinition signupName, TextDefinition factionStoneName, TextDefinition ownerLabel, TextDefinition guardIgnore, TextDefinition guardWarn, TextDefinition guardAttack, StrongholdDefintion stronghold, RankDefinition[] ranks, GuardDefinition[] guards )
		{
			m_Sort = sort;
			m_HuePrimary = huePrimary;
			m_HueSecondary = hueSecondary;
			m_HueJoin = hueJoin;
			m_HueBroadcast = hueBroadcast;
			m_WarHorseBody = warHorseBody;
			m_WarHorseItem = warHorseItem;
			m_FriendlyName = friendlyName;
			m_Keyword = keyword;
			m_Name = name;
			m_PropName = propName;
			m_Header = header;
			m_About = about;
			m_CityControl = cityControl;
			m_SigilControl = sigilControl;
			m_SignupName = signupName;
			m_FactionStoneName = factionStoneName;
			m_OwnerLabel = ownerLabel;
			m_GuardIgnore = guardIgnore;
			m_GuardWarn = guardWarn;
			m_GuardAttack = guardAttack;
			m_Stronghold = stronghold;
			m_Ranks = ranks;
			m_Guards = guards;
		}
	}
}