using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Engines.CannedEvil;
using Server.Guilds;
using Server.Factions;
using Server.Engines.PartySystem;

namespace Server.Spells.Necromancy
{
	public class ExorcismSpell : NecromancerSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Exorcism", "Ort Corp Grav", SpellCircle.Sixth, 116, 9031, Reagent.NoxCrystal, Reagent.GraveDust );

		public override double RequiredSkill { get { return 80.0; } }
		public override int RequiredMana { get { return 40; } }

		public ExorcismSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public Point3D FindNearestShrine( Mobile from, ArrayList shrines )
		{
			if ( shrines.Count <= 0 || from == null )
			{
				return Point3D.Zero;
			}

			double min_distance = 0;
			int min_number = 0;

			for ( int i = 0; i < shrines.Count; i++ )
			{
				Item shrine = shrines[ i ] as Item;

				double distance = from.GetDistanceToSqrt( shrine.Location );

				if ( i == 0 )
				{
					min_distance = distance;
				}

				Console.WriteLine( distance.ToString() );

				if ( min_distance != 0 )
				{
					if ( distance > 0 && distance < min_distance )
					{
						min_distance = distance;

						min_number = i;
					}
				}
			}

			Console.WriteLine( min_distance.ToString() );


			if ( min_distance > 0 )
			{
				Item shrine = shrines[ min_number ] as Item;

				return shrine.Location;
			}

			return Point3D.Zero;
		}

		public override void OnCast()
		{
			bool found = false;

			foreach ( Item item in Caster.GetItemsInRange( 18 ) )
			{
				if ( item is ChampionSpawn )
				{
					found = true;
				}
			}

			if ( !found )
			{
				Caster.SendLocalizedMessage( 1072111 ); // You are not in a valid exorcism region.
			}
			else if ( Caster.Skills[ SkillName.SpiritSpeak ].Value < 100.0 )
			{
				Caster.SendLocalizedMessage( 1072112 ); // You must have GM Spirit Speak to use this spell
			}
			else
			{
				if ( CheckSequence() )
				{
					ArrayList targets = new ArrayList();

					foreach ( Mobile m in Caster.GetMobilesInRange( 5 ) )
					{
						if ( m is PlayerMobile )
						{
							PlayerMobile pm = m as PlayerMobile;

							Faction faction_pm = Faction.Find( pm );

							Faction faction_caster = Faction.Find( Caster );

							Guild guild_pm = (Guild) pm.Guild;

							Guild guild_caster = (Guild) Caster.Guild;

							Party party_pm = Party.Get( pm );

							Party party_caster = Party.Get( Caster );

							bool faction = (faction_pm != null && faction_caster != null && faction_pm == faction_caster);

							bool guild = (guild_pm != null && guild_caster != null && guild_pm == guild_caster);

							bool party = (party_pm != null && party_caster != null && party_pm == party_caster);

							Container corpse = pm.Corpse;

							if ( Caster != pm && !pm.Alive && !faction && !guild && !party && corpse == null )
							{
								targets.Add( m );
							}
						}
					}

					ArrayList shrines = new ArrayList();

					foreach ( Item item in World.Items.Values )
					{
						if ( item.Map == Caster.Map && (item is AnkhWest || item is AnkhEast) )
						{
							shrines.Add( item );
						}
					}

					Console.WriteLine( shrines.Count.ToString() );

					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = (Mobile) targets[ i ];

						Point3D location = FindNearestShrine( m, shrines );

						Console.WriteLine( location.ToString() );

						if ( location != Point3D.Zero )
						{
							m.MoveToWorld( location, Caster.Map );
						}
					}
				}

				FinishSequence();
			}
		}
	}
}