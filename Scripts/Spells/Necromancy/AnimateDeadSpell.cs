using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Engines.Quests;
using Server.Engines.Quests.Necro;

namespace Server.Spells.Necromancy
{
	public class AnimateDeadSpell : NecromancerSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Animate Dead", "Uus Corp", SpellCircle.Fourth, 203, 9031, Reagent.GraveDust, Reagent.DaemonBlood );

		public override double RequiredSkill { get { return 40.0; } }
		public override int RequiredMana { get { return 23; } }

		public AnimateDeadSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendLocalizedMessage( 1061083 ); // Animate what corpse?
		}

		private class CreatureGroup
		{
			public Type[] m_Types;
			public SummonEntry[] m_Entries;

			public CreatureGroup( Type[] types, SummonEntry[] entries )
			{
				m_Types = types;
				m_Entries = entries;
			}
		}

		private class SummonEntry
		{
			public Type[] m_ToSummon;
			public int m_Requirement;

			public SummonEntry( int requirement, params Type[] toSummon )
			{
				m_ToSummon = toSummon;
				m_Requirement = requirement;
			}
		}

		private static CreatureGroup FindGroup( Type type )
		{
			for ( int i = 0; i < m_Groups.Length; ++i )
			{
				CreatureGroup group = m_Groups[ i ];
				Type[] types = group.m_Types;

				bool contains = (types.Length == 0);

				for ( int j = 0; !contains && j < types.Length; ++j )
				{
					contains = (types[ j ] == type);
				}

				if ( contains )
				{
					return group;
				}
			}

			return null;
		}

		private static CreatureGroup[] m_Groups = new CreatureGroup[] {// Undead group--empty
			new CreatureGroup( SlayerGroup.GetEntryByName( SlayerName.Silver ).Types, new SummonEntry[0] ), // Insects
			new CreatureGroup( new Type[] {typeof( DreadSpider ), typeof( FrostSpider ), typeof( GiantSpider ), typeof( GiantBlackWidow ), typeof( BlackSolenInfiltratorQueen ), typeof( BlackSolenInfiltratorWarrior ), typeof( BlackSolenQueen ), typeof( BlackSolenWarrior ), typeof( BlackSolenWorker ), typeof( RedSolenInfiltratorQueen ), typeof( RedSolenInfiltratorWarrior ), typeof( RedSolenQueen ), typeof( RedSolenWarrior ), typeof( RedSolenWorker ), typeof( TerathanAvenger ), typeof( TerathanDrone ), typeof( TerathanMatriarch ), typeof( TerathanWarrior )
				// TODO: Giant beetle? Ant lion? Ophidians?
			}, new SummonEntry[] {new SummonEntry( 0, typeof( MoundOfMaggots ) )} ), // Mounts
			new CreatureGroup( new Type[] {typeof( Horse ), typeof( Nightmare ), typeof( FireSteed ), typeof( Kirin ), typeof( Unicorn )}, new SummonEntry[] {new SummonEntry( 10000, typeof( HellSteed ) ), new SummonEntry( 0, typeof( SkeletalMount ) )} ), // Elementals
			new CreatureGroup( new Type[] {typeof( BloodElemental ), typeof( EarthElemental ), typeof( SummonedEarthElemental ), typeof( AgapiteElemental ), typeof( BronzeElemental ), typeof( CopperElemental ), typeof( DullCopperElemental ), typeof( GoldenElemental ), typeof( ShadowIronElemental ), typeof( ValoriteElemental ), typeof( VeriteElemental ), typeof( PoisonElemental ), typeof( FireElemental ), typeof( SummonedFireElemental ), typeof( SnowElemental ), typeof( AirElemental ), typeof( SummonedAirElemental ), typeof( WaterElemental ), typeof( SummonedAirElemental )}, new SummonEntry[] {new SummonEntry( 5000, typeof( WailingBanshee ) ), new SummonEntry( 0, typeof( Wraith ) )} ), // Dragons
			new CreatureGroup( new Type[] {typeof( AncientWyrm ), typeof( Dragon ), typeof( SerpentineDragon ), typeof( ShadowWyrm ), typeof( SkeletalDragon ), typeof( WhiteWyrm ), typeof( Drake ), typeof( Wyvern )}, new SummonEntry[] {new SummonEntry( 18000, typeof( SkeletalDragon ) ), new SummonEntry( 10000, typeof( FleshGolem ) ), new SummonEntry( 5000, typeof( Lich ) ), new SummonEntry( 3000, typeof( SkeletalKnight ), typeof( BoneKnight ) ), new SummonEntry( 2000, typeof( Mummy ) ), new SummonEntry( 1000, typeof( SkeletalMage ), typeof( BoneMagi ) ), new SummonEntry( 0, typeof( PatchworkSkeleton ) )} ), // Default group
			new CreatureGroup( new Type[0], new SummonEntry[] {new SummonEntry( 18000, typeof( LichLord ) ), new SummonEntry( 10000, typeof( FleshGolem ) ), new SummonEntry( 5000, typeof( Lich ) ), new SummonEntry( 3000, typeof( SkeletalKnight ), typeof( BoneKnight ) ), new SummonEntry( 2000, typeof( Mummy ) ), new SummonEntry( 1000, typeof( SkeletalMage ), typeof( BoneMagi ) ), new SummonEntry( 0, typeof( PatchworkSkeleton ) )} ),};

		public void Target( object obj )
		{
			MaabusCoffinComponent comp = obj as MaabusCoffinComponent;

			if ( comp != null )
			{
				MaabusCoffin addon = comp.Addon as MaabusCoffin;

				if ( addon != null )
				{
					PlayerMobile pm = Caster as PlayerMobile;

					if ( pm != null )
					{
						QuestSystem qs = pm.Quest;

						if ( qs is DarkTidesQuest )
						{
							QuestObjective objective = qs.FindObjective( typeof( AnimateMaabusCorpseObjective ) );

							if ( objective != null && !objective.Completed )
							{
								addon.Awake( Caster );
								objective.Complete();
							}
						}
					}

					return;
				}
			}

			Corpse c = obj as Corpse;

			if ( c == null || c.Owner is DemonKnight )
			{
				Caster.SendLocalizedMessage( 1061084 ); // You cannot animate that.
			}
			else
			{
				Type type = null;

				if ( c.Owner != null )
				{
					type = c.Owner.GetType();
				}

				if ( c.ItemID != 0x2006 || c.Channeled || type == typeof( PlayerMobile ) || type == null || (c.Owner != null && c.Owner.Fame < 100) || ((c.Owner != null) && (c.Owner is BaseCreature) && (((BaseCreature) c.Owner).Summoned || ((BaseCreature) c.Owner).IsBonded)) )
				{
					Caster.SendLocalizedMessage( 1061085 ); // There's not enough life force there to animate.
				}
				else
				{
					CreatureGroup group = FindGroup( type );

					if ( group != null )
					{
						if ( group.m_Entries.Length == 0 )
						{
							Caster.SendLocalizedMessage( 1061086 ); // You cannot animate undead remains.
						}
						else if ( CheckSequence() )
						{
							Point3D p = c.GetWorldLocation();
							Map map = c.Map;

							if ( map != null )
							{
								Effects.PlaySound( p, map, 0x1FB );
								Effects.SendLocationParticles( EffectItem.Create( p, map, EffectItem.DefaultDuration ), 0x3789, 1, 40, 0x3F, 3, 9907, 0 );

								Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( SummonDelay_Callback ), new object[] {Caster, c, p, map, group} );
							}
						}
					}
				}
			}

			FinishSequence();
		}

		private static Hashtable m_Table = new Hashtable();

		public static void Unregister( Mobile master, Mobile summoned )
		{
			if ( master == null )
			{
				return;
			}

			ArrayList list = (ArrayList) m_Table[ master ];

			if ( list == null )
			{
				return;
			}

			list.Remove( summoned );

			if ( list.Count == 0 )
			{
				m_Table.Remove( master );
			}
		}

		public static void Register( Mobile master, Mobile summoned )
		{
			if ( master == null )
			{
				return;
			}

			ArrayList list = (ArrayList) m_Table[ master ];

			if ( list == null )
			{
				m_Table[ master ] = list = new ArrayList();
			}

			for ( int i = list.Count - 1; i >= 0; --i )
			{
				if ( i >= list.Count )
				{
					continue;
				}

				Mobile mob = (Mobile) list[ i ];

				if ( mob.Deleted )
				{
					list.RemoveAt( i-- );
				}
			}

			list.Add( summoned );

			if ( list.Count > 3 )
			{
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ((Mobile) list[ 0 ]).Kill ) );
			}

			Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( Summoned_Damage ), summoned );
		}

		private static void Summoned_Damage( object state )
		{
			Mobile mob = (Mobile) state;

			if ( mob.Hits > 0 )
			{
				--mob.Hits;
			}
			else
			{
				mob.Kill();
			}
		}

		private static void SummonDelay_Callback( object state )
		{
			object[] states = (object[]) state;

			Mobile caster = (Mobile) states[ 0 ];
			Corpse corpse = (Corpse) states[ 1 ];
			Point3D loc = (Point3D) states[ 2 ];
			Map map = (Map) states[ 3 ];
			CreatureGroup group = (CreatureGroup) states[ 4 ];

			if ( corpse.ItemID != 0x2006 )
			{
				return;
			}

			Mobile owner = corpse.Owner;

			if ( owner == null )
			{
				return;
			}

			double necromancy = caster.Skills[ SkillName.Necromancy ].Value;
			double spiritSpeak = caster.Skills[ SkillName.SpiritSpeak ].Value;

			int casterAbility = 0;

			casterAbility += (int) (necromancy*30);
			casterAbility += (int) (spiritSpeak*70);
			casterAbility /= 10;
			casterAbility *= 18;

			if ( casterAbility > owner.Fame )
			{
				casterAbility = owner.Fame;
			}

			if ( casterAbility < 0 )
			{
				casterAbility = 0;
			}

			Type toSummon = null;
			SummonEntry[] entries = group.m_Entries;

			for ( int i = 0; toSummon == null && i < entries.Length; ++i )
			{
				SummonEntry entry = entries[ i ];

				if ( casterAbility < entry.m_Requirement )
				{
					continue;
				}

				Type[] animates = entry.m_ToSummon;

				if ( animates.Length >= 0 )
				{
					toSummon = animates[ Utility.Random( animates.Length ) ];
				}
			}

			if ( toSummon == null )
			{
				return;
			}

			Mobile summoned = null;

			try
			{
				summoned = Activator.CreateInstance( toSummon ) as Mobile;
			} 
			catch
			{
			}

			if ( summoned == null )
			{
				return;
			}

			if ( summoned is BaseCreature )
			{
				BaseCreature bc = (BaseCreature) summoned;

				// to be sure
				bc.Tamable = false;

				if ( bc is BaseMount )
				{
					bc.ControlSlots = 1;
				}
				else
				{
					bc.ControlSlots = 0;
				}

				Effects.PlaySound( loc, map, bc.GetAngerSound() );

				BaseCreature.Summon( (BaseCreature) summoned, false, caster, loc, 0x28, TimeSpan.FromDays( 1.0 ) );
			}

			if ( summoned is SkeletalDragon )
			{
				Scale( (SkeletalDragon) summoned, 50 ); // lose 50% hp and strength
			}

			summoned.Fame = 0;
			summoned.Karma = -1500;

			summoned.MoveToWorld( loc, map );

			corpse.ProcessDelta();
			corpse.SendRemovePacket();
			corpse.ItemID = Utility.Random( 0xECA, 9 ); // bone graphic
			corpse.Hue = 0;
			corpse.ProcessDelta();

			Register( caster, summoned );
		}

		private static void Scale( BaseCreature bc, int scalar )
		{
			int toScale;

			toScale = bc.RawStr;
			bc.RawStr = AOS.Scale( toScale, scalar );

			toScale = bc.HitsMaxSeed;

			if ( toScale > 0 )
			{
				bc.HitsMaxSeed = AOS.Scale( toScale, scalar );
			}

			bc.Hits = bc.Hits; // refresh hits
		}

		private class InternalTarget : Target
		{
			private AnimateDeadSpell m_Owner;

			public InternalTarget( AnimateDeadSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				m_Owner.Target( o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}