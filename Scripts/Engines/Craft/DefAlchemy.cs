using System;
using Server.Items;

namespace Server.Engines.Craft
{
	public class DefAlchemy : CraftSystem
	{
		public override SkillName MainSkill { get { return SkillName.Alchemy; } }

		public override int GumpTitleNumber
		{
			get { return 1044001; } // <CENTER>ALCHEMY MENU</CENTER>
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
				{
					m_CraftSystem = new DefAlchemy();
				}

				return m_CraftSystem;
			}
		}

		public override double GetChanceAtMin( CraftItem item )
		{
			return 0.0; // 0%
		}

		private DefAlchemy() : base( 1, 1, 1.25 ) // base( 1, 1, 3.1 )
		{
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if ( tool.Deleted || tool.UsesRemaining < 0 )
			{
				return 1044038; // You have worn out your tool!
			} 
			else if ( !BaseTool.CheckAccessible( tool, from ) )
			{
				return 1044263; // The tool must be on your person to use.
			} 

			return 0;
		}

		public override void PlayCraftEffect( Mobile from )
		{
			from.PlaySound( 0x242 );
		}

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
		{
			if ( toolBroken )
			{
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool
			} 

			Type type = item.ItemType;

			if ( failed )
			{
				if ( type != typeof( SmokeBomb ) )
				{
					from.AddToBackpack( new Bottle() );
				}

				return 1044043; // You failed to create the item, and some of your materials are lost.
			}
			else
			{
				if ( type != typeof( SmokeBomb ) )
				{
					from.PlaySound( 0x240 ); // Sound of a filling bottle

					if ( quality == -1 )
					{
						return 1048136; // You create the potion and pour it into a keg.
					} 
				}

				return 1044154; // You create the item.
			}
		}

		public override void InitCraftList()
		{
			int index = -1;

			// Refresh Potion
			index = AddCraft( typeof( RefreshPotion ), 1044530, 1044538, -25.0, 25.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( BlackPearl ), 1044353, 1, 1042081 );
			index = AddCraft( typeof( TotalRefreshPotion ), 1044530, 1044539, 25.0, 75.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( BlackPearl ), 1044353, 5, 1042081 );

			// Agility Potion
			index = AddCraft( typeof( AgilityPotion ), 1044531, 1044540, 15.0, 65.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Bloodmoss ), 1044354, 1, 1042081 );
			index = AddCraft( typeof( GreaterAgilityPotion ), 1044531, 1044541, 35.0, 85.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Bloodmoss ), 1044354, 3, 1042081 );

			// Nightsight Potion
			index = AddCraft( typeof( NightSightPotion ), 1044532, 1044542, -25.0, 25.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( SpidersSilk ), 1044360, 1, 1042081 );

			// Heal Potion
			index = AddCraft( typeof( LesserHealPotion ), 1044533, 1044543, -25.0, 25.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Ginseng ), 1044356, 1, 1042081 );
			index = AddCraft( typeof( HealPotion ), 1044533, 1044544, 15.0, 65.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Ginseng ), 1044356, 3, 1042081 );
			index = AddCraft( typeof( GreaterHealPotion ), 1044533, 1044545, 55.0, 105.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Ginseng ), 1044356, 7, 1042081 );

			// Strength Potion
			index = AddCraft( typeof( StrengthPotion ), 1044534, 1044546, 25.0, 75.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( MandrakeRoot ), 1044357, 2, 1042081 );
			index = AddCraft( typeof( GreaterStrengthPotion ), 1044534, 1044547, 45.0, 95.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( MandrakeRoot ), 1044357, 5, 1042081 );

			// Poison Potion
			index = AddCraft( typeof( LesserPoisonPotion ), 1044535, 1044548, -5.0, 45.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Nightshade ), 1044358, 1, 1042081 );
			index = AddCraft( typeof( PoisonPotion ), 1044535, 1044549, 15.0, 65.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Nightshade ), 1044358, 2, 1042081 );
			index = AddCraft( typeof( GreaterPoisonPotion ), 1044535, 1044550, 55.0, 105.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Nightshade ), 1044358, 4, 1042081 );
			index = AddCraft( typeof( DeadlyPoisonPotion ), 1044535, 1044551, 90.0, 140.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Nightshade ), 1044358, 8, 1042081 );

			// Cure Potion
			index = AddCraft( typeof( LesserCurePotion ), 1044536, 1044552, -10.0, 40.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Garlic ), 1044355, 1, 1042081 );
			index = AddCraft( typeof( CurePotion ), 1044536, 1044553, 25.0, 75.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Garlic ), 1044355, 3, 1042081 );
			index = AddCraft( typeof( GreaterCurePotion ), 1044536, 1044554, 65.0, 115.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( Garlic ), 1044355, 6, 1042081 );

			// Explosion Potion
			index = AddCraft( typeof( LesserExplosionPotion ), 1044537, 1044555, 5.0, 55.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( SulfurousAsh ), 1044359, 3, 1042081 );
			index = AddCraft( typeof( ExplosionPotion ), 1044537, 1044556, 35.0, 85.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( SulfurousAsh ), 1044359, 5, 1042081 );
			index = AddCraft( typeof( GreaterExplosionPotion ), 1044537, 1044557, 65.0, 115.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( SulfurousAsh ), 1044359, 10, 1042081 );

			if ( Core.SE )
			{
				index = AddCraft( typeof( SmokeBomb ), 1044537, 1030248, 90.0, 120.0, typeof( Eggs ), 1044477, 1, 1044253 );
				AddRes( index, typeof( Ginseng ), 1044356, 3, 1044364 );
				SetNeedSE( index, true );
			}

			// Conflagration Potion
			index = AddCraft( typeof( ConflagrationPotion ), 1044109, 1072096, 55.0, 105.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( GraveDust ), 1023983, 5, 1042081 );
			index = AddCraft( typeof( GreaterConflagrationPotion ), 1044109, 1072099, 70.0, 120.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( GraveDust ), 1023983, 10, 1042081 );

			// Mask Of Death Potion
			/*
			// I hate OSI!
			index = AddCraft( typeof( MaskOfDeathPotion ), 1044109, 1072101, 55.0, 105.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof ( DaemonBlood ), 1023965, 5, 1042081 );
			index = AddCraft( typeof( GreaterMaskOfDeathPotion ), 1044109, 1072104, 85.0, 135.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof ( DaemonBlood ), 1023965, 10, 1042081 );
			*/

			// Confusion Blast Potion
			index = AddCraft( typeof( ConfusionBlastPotion ), 1044109, 1072106, 50.0, 100.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( PigIron ), 1023978, 5, 1042081 );
			index = AddCraft( typeof( GreaterConfusionBlastPotion ), 1044109, 1072109, 65.0, 115.0, typeof( Bottle ), 1044529, 1, 1044558 );
			AddRes( index, typeof( PigIron ), 1023978, 10, 1042081 );
		}
	}
}