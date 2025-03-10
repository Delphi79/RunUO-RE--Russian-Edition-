using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Doom
{
	public class CollectBonesObjective : QuestObjective
	{
		public override object Message
		{
			get
			{
				/* Find 1000 Daemon bones and hand them
				 * to Victoria as you find them.
				 */
				return 1050026;
			}
		}

		public override int MaxProgress { get { return 1000; } }

		public CollectBonesObjective()
		{
		}

		public override void OnComplete()
		{
			Victoria victoria = ((TheSummoningQuest) System).Victoria;

			if ( victoria == null )
			{
				System.From.SendMessage( "Internal error: unable to find Victoria. Quest unable to continue." );
				System.Cancel();
			}
			else
			{
				SummoningAltar altar = victoria.Altar;

				if ( altar == null )
				{
					System.From.SendMessage( "Internal error: unable to find summoning altar. Quest unable to continue." );
					System.Cancel();
				}
				else if ( altar.Daemon == null || !altar.Daemon.Alive )
				{
					System.AddConversation( new VanquishDaemonConversation() );
				}
				else
				{
					victoria.SayTo( System.From, "The devourer has already been summoned. Return when the devourer has been slain and I will summon it for you." );
					((TheSummoningQuest) System).WaitForSummon = true;
				}
			}
		}

		public override void RenderMessage( BaseQuestGump gump )
		{
			if ( CurProgress > 0 && CurProgress < MaxProgress )
			{
				gump.AddHtmlObject( 70, 130, 300, 100, 1050028, BaseQuestGump.Blue, false, false ); // Victoria has accepted the Daemon bones, but the requirement is not yet met.
			}
			else
			{
				base.RenderMessage( gump );
			}
		}

		public override void RenderProgress( BaseQuestGump gump )
		{
			if ( CurProgress > 0 && CurProgress < MaxProgress )
			{
				gump.AddHtmlObject( 70, 260, 270, 100, 1050019, BaseQuestGump.Blue, false, false ); // Number of bones collected:

				gump.AddLabel( 70, 280, 100, CurProgress.ToString() );
				gump.AddLabel( 100, 280, 100, "/" );
				gump.AddLabel( 130, 280, 100, MaxProgress.ToString() );
			}
			else
			{
				base.RenderProgress( gump );
			}
		}
	}

	public class VanquishDaemonObjective : QuestObjective
	{
		private BoneDemon m_Daemon;
		private Corpse m_CorpseWithSkull;

		public Corpse CorpseWithSkull { get { return m_CorpseWithSkull; } set { m_CorpseWithSkull = value; } }

		public override object Message
		{
			get
			{
				/* Go forth and vanquish the devourer that has been summoned!
				 */
				return 1050037;
			}
		}

		public VanquishDaemonObjective( BoneDemon daemon )
		{
			m_Daemon = daemon;
		}

		// Serialization
		public VanquishDaemonObjective()
		{
		}

		public override void CheckProgress()
		{
			if ( m_Daemon == null || !m_Daemon.Alive )
			{
				Complete();
			}
		}

		public override void OnComplete()
		{
			Victoria victoria = ((TheSummoningQuest) System).Victoria;

			if ( victoria != null )
			{
				SummoningAltar altar = victoria.Altar;

				if ( altar != null )
				{
					altar.CheckDaemon();
				}
			}

			PlayerMobile from = System.From;

			if ( !from.Alive )
			{
				from.SendLocalizedMessage( 1050033 ); // The devourer lies dead, unfortunately so do you.  You cannot claim your reward while dead.  You will need to face him again.
				((TheSummoningQuest) System).WaitForSummon = true;
			}
			else
			{
				bool hasRights = true;

				if ( m_Daemon != null )
				{
					ArrayList lootingRights = BaseCreature.GetLootingRights( m_Daemon.DamageEntries, m_Daemon.HitsMax );

					for ( int i = 0; i < lootingRights.Count; ++i )
					{
						DamageStore ds = (DamageStore) lootingRights[ i ];

						if ( ds.m_HasRight && ds.m_Mobile == from )
						{
							hasRights = true;
							break;
						}
					}
				}

				if ( !hasRights )
				{
					from.SendLocalizedMessage( 1050034 ); // The devourer lies dead.  Unfortunately you did not sufficiently prove your worth in combating the devourer.  Victoria shall summon another incarnation of the devourer to the circle of stones.  Try again noble adventurer.
					((TheSummoningQuest) System).WaitForSummon = true;
				}
				else
				{
					from.SendLocalizedMessage( 1050035 ); // The devourer lies dead.  Search his corpse to claim your prize!

					if ( m_Daemon != null )
					{
						m_CorpseWithSkull = m_Daemon.Corpse as Corpse;
					}
				}
			}
		}

		public override void ChildDeserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			m_Daemon = reader.ReadMobile() as BoneDemon;
			m_CorpseWithSkull = reader.ReadItem() as Corpse;
		}

		public override void ChildSerialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (Mobile) m_Daemon );
			writer.Write( (Item) m_CorpseWithSkull );
		}
	}
}