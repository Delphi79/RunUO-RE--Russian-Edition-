using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Bushido;
using Server.Mobiles;

namespace Server.Misc
{
	public class RegenRates
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
			Mobile.DefaultHitsRate = TimeSpan.FromSeconds( 11.0 );
			Mobile.DefaultStamRate = TimeSpan.FromSeconds( 7.0 );
			Mobile.DefaultManaRate = TimeSpan.FromSeconds( 7.0 );

			Mobile.ManaRegenRateHandler = new RegenRateHandler( Mobile_ManaRegenRate );

			if ( Core.AOS )
			{
				Mobile.StamRegenRateHandler = new RegenRateHandler( Mobile_StamRegenRate );
				Mobile.HitsRegenRateHandler = new RegenRateHandler( Mobile_HitsRegenRate );
			}
		}

		private static void CheckBonusSkill( Mobile m, int cur, int max, SkillName skill )
		{
			if ( !m.Alive )
			{
				return;
			}

			double n = (double) cur/max;
			double v = Math.Sqrt( m.Skills[ skill ].Value*0.005 );

			n *= (1.0 - v);
			n += v;

			m.CheckSkill( skill, n );
		}

		private static bool CheckTransform( Mobile m, Type type )
		{
			return TransformationSpell.UnderTransformation( m, type );
		}

		private static TimeSpan Mobile_HitsRegenRate( Mobile from )
		{
			int points = AosAttributes.GetValue( from, AosAttribute.RegenHits );

			if ( from is Mobiles.BaseCreature && !((Mobiles.BaseCreature) from).IsAnimatedDead )
			{
				points += 4;
			}

			if ( (from is Mobiles.BaseCreature && ((Mobiles.BaseCreature) from).IsParagon) || from is Leviathan )
			{
				points += 40;
			}

			if ( CheckTransform( from, typeof( HorrificBeastSpell ) ) )
			{
				points += 20;
			}

			if ( from.BodyMod == 0xD9 || from.BodyMod == 0xC9 )
			{
				// TODO: verify
				points += (int) (from.Skills[ SkillName.Ninjitsu ].Value/30.0);
			}

			if ( points < 0 )
			{
				points = 0;
			}

			return TimeSpan.FromSeconds( 1.0/(0.1*(1 + points)) );
		}

		private static TimeSpan Mobile_StamRegenRate( Mobile from )
		{
			if ( from.Skills == null )
			{
				return Mobile.DefaultStamRate;
			}

			CheckBonusSkill( from, from.Stam, from.StamMax, SkillName.Focus );

			int points = AosAttributes.GetValue( from, AosAttribute.RegenStam ) + (int) (from.Skills[ SkillName.Focus ].Value*0.1);

			if ( CheckTransform( from, typeof( VampiricEmbraceSpell ) ) )
			{
				points += 15;
			}

			if ( from is Mobiles.BaseCreature && ((Mobiles.BaseCreature) from).IsParagon )
			{
				points += 40;
			}

			if ( from.BodyMod == 0xD9 || from.BodyMod == 0xC9 )
			{
				// TODO: verify
				points += (int) (from.Skills[ SkillName.Ninjitsu ].Value/30.0);
			}

			if ( from.BodyMod == 0x84 )
			{
				// TODO: verify
				points += (int) (from.Skills[ SkillName.Ninjitsu ].Value*0.1);
			} 

			if ( points < -1 )
			{
				points = -1;
			}

			return TimeSpan.FromSeconds( 1.0/(0.1*(2 + points)) );
		}

		private static TimeSpan Mobile_ManaRegenRate( Mobile from )
		{
			if ( from.Skills == null )
			{
				return Mobile.DefaultManaRate;
			}

			if ( !from.Meditating )
			{
				CheckBonusSkill( from, from.Mana, from.ManaMax, SkillName.Meditation );
			}

			double rate;
			double armorPenalty = GetArmorOffset( from );

			if ( Core.AOS )
			{
				double medPoints = from.Int + (from.Skills[ SkillName.Meditation ].Value*3);

				medPoints *= (from.Skills[ SkillName.Meditation ].Value < 100.0) ? 0.025 : 0.0275;

				CheckBonusSkill( from, from.Mana, from.ManaMax, SkillName.Focus );

				double focusPoints = (int) (from.Skills[ SkillName.Focus ].Value*0.05);

				if ( armorPenalty > 0 )
				{
					medPoints = 0; // In AOS, wearing any meditation-blocking armor completely removes meditation bonus
				}

				double totalPoints = AosAttributes.GetValue( from, AosAttribute.RegenMana ) + focusPoints + medPoints + (from.Meditating ? (medPoints > 13.0 ? 13.0 : medPoints) : 0.0);

				if ( CheckTransform( from, typeof( VampiricEmbraceSpell ) ) )
				{
					totalPoints += 3;
				}
				else if ( CheckTransform( from, typeof( LichFormSpell ) ) )
				{
					totalPoints += 13;
				}

				if ( (from is Mobiles.BaseCreature && ((Mobiles.BaseCreature) from).IsParagon) || from is Leviathan )
				{
					totalPoints += 40;
				}

				if ( from.BodyMod == 0xD9 || from.BodyMod == 0xC9 )
				{
					// TODO: verify
					totalPoints += (int) (from.Skills[ SkillName.Ninjitsu ].Value/30.0);
				} 

				if ( totalPoints < -1 )
				{
					totalPoints = -1;
				}

				rate = 1.0/(0.1*(2 + (int) totalPoints));
			}
			else
			{
				double medPoints = (from.Int + from.Skills[ SkillName.Meditation ].Value)*0.5;

				if ( medPoints <= 0 )
				{
					rate = 7.0;
				}
				else if ( medPoints <= 100 )
				{
					rate = 7.0 - (239*medPoints/2400) + (19*medPoints*medPoints/48000);
				}
				else if ( medPoints < 120 )
				{
					rate = 1.0;
				}
				else
				{
					rate = 0.75;
				}

				rate += armorPenalty;

				if ( from.Meditating )
				{
					rate *= 0.5;
				}

				if ( rate < 0.5 )
				{
					rate = 0.5;
				}
				else if ( rate > 7.0 )
				{
					rate = 7.0;
				}
			}

			return TimeSpan.FromSeconds( rate );
		}

		private static double GetArmorOffset( Mobile from )
		{
			double rating = 0.0;

			if ( !Core.AOS )
			{
				rating += GetArmorMeditationValue( from.ShieldArmor as BaseArmor );
			}

			rating += GetArmorMeditationValue( from.NeckArmor as BaseArmor );
			rating += GetArmorMeditationValue( from.HandArmor as BaseArmor );
			rating += GetArmorMeditationValue( from.HeadArmor as BaseArmor );
			rating += GetArmorMeditationValue( from.ArmsArmor as BaseArmor );
			rating += GetArmorMeditationValue( from.LegsArmor as BaseArmor );
			rating += GetArmorMeditationValue( from.ChestArmor as BaseArmor );

			return rating/4;
		}

		private static double GetArmorMeditationValue( BaseArmor ar )
		{
			if ( ar == null || ar.ArmorAttributes.MageArmor != 0 || ar.Attributes.SpellChanneling != 0 )
			{
				return 0.0;
			}

			switch ( ar.MeditationAllowance )
			{
				default:
				case ArmorMeditationAllowance.None:
					return ar.BaseArmorRatingScaled;
				case ArmorMeditationAllowance.Half:
					return ar.BaseArmorRatingScaled/2.0;
				case ArmorMeditationAllowance.All:
					return 0.0;
			}
		}
	}
}