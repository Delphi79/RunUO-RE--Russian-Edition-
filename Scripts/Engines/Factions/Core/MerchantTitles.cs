using System;

namespace Server.Factions
{
	public enum MerchantTitle
	{
		None,
		Scribe,
		Carpenter,
		Blacksmith,
		Bowyer,
		Tialor
	}

	public class MerchantTitleInfo
	{
		private SkillName m_Skill;
		private double m_Requirement;
		private TextDefinition m_Title;
		private TextDefinition m_Label;
		private TextDefinition m_Assigned;

		public SkillName Skill { get { return m_Skill; } }
		public double Requirement { get { return m_Requirement; } }
		public TextDefinition Title { get { return m_Title; } }
		public TextDefinition Label { get { return m_Label; } }
		public TextDefinition Assigned { get { return m_Assigned; } }

		public MerchantTitleInfo( SkillName skill, double requirement, TextDefinition title, TextDefinition label, TextDefinition assigned )
		{
			m_Skill = skill;
			m_Requirement = requirement;
			m_Title = title;
			m_Label = label;
			m_Assigned = assigned;
		}
	}

	public class MerchantTitles
	{
		private static MerchantTitleInfo[] m_Info = new MerchantTitleInfo[] {new MerchantTitleInfo( SkillName.Inscribe, 90.0, new TextDefinition( 1060773, "Scribe" ), new TextDefinition( 1011468, "SCRIBE" ), new TextDefinition( 1010121, "You now have the faction title of scribe" ) ), new MerchantTitleInfo( SkillName.Carpentry, 90.0, new TextDefinition( 1060774, "Carpenter" ), new TextDefinition( 1011469, "CARPENTER" ), new TextDefinition( 1010122, "You now have the faction title of carpenter" ) ), new MerchantTitleInfo( SkillName.Tinkering, 90.0, new TextDefinition( 1022984, "Tinker" ), new TextDefinition( 1011470, "TINKER" ), new TextDefinition( 1010123, "You now have the faction title of tinker" ) ), new MerchantTitleInfo( SkillName.Blacksmith, 90.0, new TextDefinition( 1023016, "Blacksmith" ), new TextDefinition( 1011471, "BLACKSMITH" ), new TextDefinition( 1010124, "You now have the faction title of blacksmith" ) ), new MerchantTitleInfo( SkillName.Fletching, 90.0, new TextDefinition( 1023022, "Bowyer" ), new TextDefinition( 1011472, "BOWYER" ), new TextDefinition( 1010125, "You now have the faction title of Bowyer" ) ), new MerchantTitleInfo( SkillName.Tailoring, 90.0, new TextDefinition( 1022982, "Tailor" ), new TextDefinition( 1018300, "TAILOR" ), new TextDefinition( 1042162, "You now have the faction title of Tailor" ) ),};

		public static MerchantTitleInfo[] Info { get { return m_Info; } }

		public static MerchantTitleInfo GetInfo( MerchantTitle title )
		{
			int idx = (int) title - 1;

			if ( idx >= 0 && idx < m_Info.Length )
			{
				return m_Info[ idx ];
			}

			return null;
		}

		public static bool HasMerchantQualifications( Mobile mob )
		{
			for ( int i = 0; i < m_Info.Length; ++i )
			{
				if ( IsQualified( mob, m_Info[ i ] ) )
				{
					return true;
				}
			}

			return false;
		}

		public static bool IsQualified( Mobile mob, MerchantTitle title )
		{
			return IsQualified( mob, GetInfo( title ) );
		}

		public static bool IsQualified( Mobile mob, MerchantTitleInfo info )
		{
			if ( mob == null || info == null )
			{
				return false;
			}

			return (mob.Skills[ info.Skill ].Value >= info.Requirement);
		}
	}
}