using System;
using Server;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Chivalry
{
	public class SacredJourneySpell : PaladinSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Sacred Journey", "Sanctum Viatas", SpellCircle.Seventh, -1, 9002 );

		public override double RequiredSkill { get { return 15.0; } }
		public override int RequiredMana { get { return 10; } }
		public override int RequiredTithing { get { return 15; } }
		public override int MantraNumber { get { return 1060727; } } // Sanctum Viatas
		public override bool BlocksMovement { get { return false; } }

		private RunebookEntry m_Entry;
		private Runebook m_Book;

		public SacredJourneySpell( Mobile caster, Item scroll ) : this( caster, scroll, null, null )
		{
		}

		public SacredJourneySpell( Mobile caster, Item scroll, RunebookEntry entry, Runebook book ) : base( caster, scroll, m_Info )
		{
			m_Entry = entry;
			m_Book = book;
		}

		public override void OnCast()
		{
			if ( m_Entry == null )
			{
				Caster.Target = new InternalTarget( this );
			}
			else
			{
				Effect( m_Entry.Location, m_Entry.Map, true );
			}
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			{
				return false;
			}

			if ( Factions.Sigil.ExistsOn( Caster ) )
			{
				Caster.SendLocalizedMessage( 1061632 ); // You can't do that while carrying the sigil.
				return false;
			}
			else if ( Caster.Criminal )
			{
				Caster.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
				return false;
			}
			else if ( SpellHelper.CheckCombat( Caster ) )
			{
				Caster.SendLocalizedMessage( 1061282 ); // You cannot use the Sacred Journey ability to flee from combat.
				return false;
			}
			else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
				return false;
			}

			return SpellHelper.CheckTravel( Caster, TravelCheckType.RecallFrom );
		}

		public void Effect( Point3D loc, Map map, bool checkMulti )
		{
			if ( Factions.Sigil.ExistsOn( Caster ) )
			{
				Caster.SendLocalizedMessage( 1061632 ); // You can't do that while carrying the sigil.
			}
			else if ( map == null || (!Core.AOS && Caster.Map != map) )
			{
				Caster.SendLocalizedMessage( 1005569 ); // You can not recall to another facet.
			}
			else if ( !SpellHelper.CheckTravel( Caster, TravelCheckType.RecallFrom ) )
			{
			}
			else if ( !SpellHelper.CheckTravel( Caster, map, loc, TravelCheckType.RecallTo ) )
			{
			}
			else if ( map == Map.Felucca && Caster is PlayerMobile && ((PlayerMobile) Caster).Young )
			{
				Caster.SendLocalizedMessage( 1049543 ); // You decide against traveling to Felucca while you are still young.
			}
			else if ( Caster.Kills >= 5 && map != Map.Felucca )
			{
				Caster.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.
			}
			else if ( Caster.Criminal )
			{
				Caster.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
			}
			else if ( SpellHelper.CheckCombat( Caster ) )
			{
				Caster.SendLocalizedMessage( 1061282 ); // You cannot use the Sacred Journey ability to flee from combat.
			}
			else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
			}
			else if ( !map.CanSpawnMobile( loc.X, loc.Y, loc.Z ) )
			{
				Caster.SendLocalizedMessage( 501025 ); // Something is blocking the location.
			}
			else if ( (checkMulti && SpellHelper.CheckMulti( loc, map )) )
			{
				Caster.SendLocalizedMessage( 501025 ); // Something is blocking the location.
			}
			else if ( m_Book != null && m_Book.CurCharges <= 0 )
			{
				Caster.SendLocalizedMessage( 502412 ); // There are no charges left on that item.
			}
			else if ( CheckSequence() )
			{
				BaseCreature.TeleportPets( Caster, loc, map, true );

				if ( m_Book != null )
				{
					--m_Book.CurCharges;
				}

				Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0, 0, 0, 5033 );

				Caster.MoveToWorld( loc, map );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private SacredJourneySpell m_Owner;

			public InternalTarget( SacredJourneySpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is RecallRune )
				{
					RecallRune rune = (RecallRune) o;

					if ( rune.Marked )
					{
						m_Owner.Effect( rune.Target, rune.TargetMap, true );
					}
					else
					{
						from.SendLocalizedMessage( 501805 ); // That rune is not yet marked.
					}
				}
				else if ( o is Runebook )
				{
					RunebookEntry e = ((Runebook) o).Default;

					if ( e != null )
					{
						m_Owner.Effect( e.Location, e.Map, true );
					}
					else
					{
						from.SendLocalizedMessage( 502354 ); // Target is not marked.
					}
				}
				else if ( o is Key && ((Key) o).KeyValue != 0 && ((Key) o).Link is BaseBoat )
				{
					BaseBoat boat = ((Key) o).Link as BaseBoat;

					if ( !boat.Deleted && boat.CheckKey( ((Key) o).KeyValue ) )
					{
						m_Owner.Effect( boat.GetMarkedLocation(), boat.Map, false );
					}
					else
					{
						from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // I can not recall from that object.
					}
				}
				else
				{
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // I can not recall from that object.
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}