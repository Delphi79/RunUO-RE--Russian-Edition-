using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Fifth
{
	public class SummonCreatureSpell : Spell
	{
		private static SpellInfo m_Info = new SpellInfo( "Summon Creature", "Kal Xen", SpellCircle.Fifth, 266, 9040, Reagent.Bloodmoss, Reagent.MandrakeRoot, Reagent.SpidersSilk );

		public SummonCreatureSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static Type[] m_Types = new Type[] {typeof( Scorpion ), typeof( GiantSerpent ), typeof( GreyWolf ), typeof( Chicken ), typeof( Hind ), typeof( Alligator ), typeof( SnowLeopard ), typeof( PolarBear ), typeof( Walrus ), typeof( GrizzlyBear ), typeof( BlackBear ), typeof( Llama ), typeof( Slime ), typeof( Pig ), typeof( Horse ), typeof( Eagle ), typeof( Gorilla )};

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			{
				return false;
			}

			if ( SpellHelper.IsTown( Caster.Location, Caster ) )
			{
				Caster.SendLocalizedMessage( 502745 ); // You cannot summon in this area.
				return false;
			}

			if ( (Caster.Followers + 1) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				try
				{
					BaseCreature creature = (BaseCreature) Activator.CreateInstance( m_Types[ Utility.Random( m_Types.Length ) ] );

					creature.ControlSlots = 1;

					TimeSpan duration;

					if ( Core.AOS )
					{
						duration = TimeSpan.FromSeconds( (2*Caster.Skills.Magery.Fixed)/5 );
					}
					else
					{
						duration = TimeSpan.FromSeconds( 4.0*Caster.Skills[ SkillName.Magery ].Value );
					}

					SpellHelper.Summon( creature, Caster, 0x215, duration, false, false );
				} 
				catch
				{
				}
			}

			FinishSequence();
		}

		public override TimeSpan GetCastDelay()
		{
			if ( Core.AOS )
			{
				return TimeSpan.FromTicks( base.GetCastDelay().Ticks*5 );
			}

			return base.GetCastDelay() + TimeSpan.FromSeconds( 6.0 );
		}
	}
}