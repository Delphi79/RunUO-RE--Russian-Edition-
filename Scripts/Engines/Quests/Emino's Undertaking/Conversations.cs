using System;
using Server;

namespace Server.Engines.Quests.SE
{
	public class DaimyoEminoBeginConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* Ah, another fledgling unfurls its wings. Welcome to my
				 * home young one.<BR><BR>
				 * 
				 * I am Daimyo Emino, a passionate collector of sorts. One
				 * who is vengeful towards those impeding my reign. <BR><BR>
				 *
				 * You have the look of someone who could help me but
				 * your skills are untested. Are you willing to prove your
				 * mettle as my hireling? <BR><BR>
				 *
				 * Elite Ninja Zoel awaits you in the backyard. He will lead
				 * you to the first trial. You will be directed further when
				 * you arrive at your destination. You should speak to him   
				 * before exploring the yard or cave entrance.
				 */
				return 1063175;
			}
		}

		public override bool Logged { get { return false; } }

		public DaimyoEminoBeginConversation()
		{
		}
	}

	public class RadarSEConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* To view the surrounding area, you should learn about the
				 * Radar Map.<BR><BR>
				 * 
				 * The Radar Map (or Overhead View) can be opened by
				 * pressing 'ALT-R' on your keyboard. It shows your
				 * immediate surroundings from a bird's eye view.<BR><BR>
				 *
				 * Pressing ALT-R twice, will enlarge the Radar Map a
				 * little. Use the Radar Map often as you travel throughout
				 * the world to familiarize yourself with your surroundings.
				 */
				return 1063033;
			}
		}

		public override bool Logged { get { return false; } }

		public RadarSEConversation()
		{
		}
	}

	public class ZoelBeginConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>Zoel studies your face as you approach him.
				 * Wryly, he says:</I><BR><BR>
				 * 
				 *  Daimyo Emino has sent another already? The stains
				 * from the last have not yet dried! <BR><BR>
                                 *
				 * No matter, we'll finish you off and clean it all at once,
				 * eh? <BR><BR>
				 *
				 * Now to the point, your only task is to survive in the
				 * abandoned inn.<BR><BR>
				 *
				 * You will be instructed when you need to act and when
				 * you should return to one of us. <BR><BR>
				 *
				 * Only a true Ninja is deft enough to finish and remain
				 * alive.<BR><BR>
				 *
				 * Your future... or your demise... lies in this cave beyond. <BR><BR>
				 *
				 * Now go.
				 */
				return 1063177;
			}
		}

		public override bool Logged { get { return false; } }

		public ZoelBeginConversation()
		{
		}
	}

	public class StandsConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* A glowing archway stands before you. <BR><BR>
				 * 
				 * To either side of its frame are mounted demon heads,
				 * breathing fire and watching your every move. <BR><BR>
				 * 
                                 * To pass through, you must first vanish from the
				 * demons' sight. Only then can you slowly traverse the
				 * entryway.
				 */
				return 1063180;
			}
		}

		public override bool Logged { get { return false; } }

		public StandsConversation()
		{
		}

		public override void OnRead()
		{
			EminosUndertakingQuest euq = System as EminosUndertakingQuest;

			euq.CanSendWarning = true;
		}
	}

	public class StandsWarningConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* You'll need to hide in order to pass through the door. <BR><BR>
				 *
				 * To find out how to use active skills, visit the <a href = "?ForceTopic73">Codex of
				 * Wisdom</a>. To activate a skill, locate it on your skills list
				 * and click the blue button located to the left of the
				 * skill's name.<br><br>
                                 * 
				 * Once you have successfully hidden, you may move slowly
				 * through the door.
				 */
				return 1063181;
			}
		}

		public override bool Logged { get { return false; } }

		public StandsWarningConversation()
		{
		}
	}

	public class StrangePassageConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* Through the door lies a short passageway. The path ends
				 * abruptly at a strange tile on the floor.  The special tile
				 * is known as a teleporter.  Step on the teleporter tile
				 * and you will be transported to a new location.
				 */
				return 1063182;
			}
		}

		public override bool Logged { get { return false; } }

		public StrangePassageConversation()
		{
		}
	}

	public class EminoSecondConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>Daimyo Emino smiles as you approach him:</I> <BR><BR>
				 * 
				 * I see that you have survived both the first trial and
				 * Zoel's temper. <BR><BR>
				 *
				 * For that you have been rewarded with Leggings and
				 * Gloves befitting your occupation. The material is the only
				 * armor a <b><I>True Ninja</i></b> needs. <BR><BR>
				 *
			         * You have yet to prove yourself fully, young hireling.
				 * Another trial must be met. Off to Zoel you go. Bring
				 * him this note so he knows we have spoken.
				 */
				return 1063184;
			}
		}

		public override bool Logged { get { return false; } }

		public EminoSecondConversation()
		{
		}
	}

	public class ZoelGrubsNoteConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>Zoel quickly grabs the scroll from your hand and
				 * reads the note:</i> <BR><BR>
				 * 
				 * Still alive then? You'll have to impress me further
				 * before I will give my approval of you to Daimyo Emino. <BR><BR>
				 * 
				 * You must return to the inn and begin the next trial. <BR><BR>
				 * 
				 * We believe an associate has, shall we say, inadvertently
			         * negated our contract. <BR><BR>
				 * 
				 * Find out what information you can and return to Daimyo
				 * Emino with the news. And be careful not to lose your
				 * head.<BR><BR>
				 *
				 * The Blue Teleporter Tile in Daimyo Emino's residence will
				 * lead you to your fate. I suggest you hurry. <BR><BR>
				 *
				 * ...And take care to tread softly. There is no greater
				 * traitor than a heavy footfall upon a path.
				 */
				return 1063189;
			}
		}

		public override bool Logged { get { return false; } }

		public ZoelGrubsNoteConversation()
		{
		}
	}

	public class ApproachTheDoorConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* You quietly approach the door and see a woman named
				 * Jedah Entille speaking to a shady figure in dark clothing.
				 * You move closer so you can overhear the conversation.
				 * Fortunately, your entrance did not alert the preoccupied
				 * party. <BR><BR>
				 * 
				 * Jedah's brash voice permeates the air:<I><BR><BR>
				 * 
				 *  Now that it is hidden, we'll hide out here until
			         * Daimyo Emino forgets about us. Once he realizes
				 * his beloved sword is missing, he'll surely start
				 * looking for the thieves. We will be long gone by
				 * that time. </I><BR><BR>
				 * 
				 * After overhearing the conversation, you understand why
				 * you were sent on this trial. You must immediately tell
				 * Daimyo Emino what you have learned.
				 */
				return 1063196;
			}
		}

		public override bool Logged { get { return false; } }

		public ApproachTheDoorConversation()
		{
		}
	}

	public class FrownsConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>Daimyo Emino frowns as you relay the information.
				 * He pauses for a moment before speaking to you:</i> <BR><BR>
				 * 
				 * Jedah was once one of my most promising students, but
				 * her greed will be her downfall. <BR><BR>
				 * 
				 * I will send one of my disciples to deal with her later. It
				 * is more important to get that sword back first. <BR><BR>
				 * 
			         * I'm counting on you to find it. She would have kept it
				 * close to her. Take the White Teleporter, located in my
				 * backyard, and check inside boxes and chests around the
				 * treasure room of the inn and return it to me when you
				 * find it.<BR><BR>
				 * 
				 * Be very careful. Jedah was an expert with traps and no
				 * doubt she's protecting the sword with them. <BR><BR>
				 *
				 * If you find a trap, try timing it and you may be able to
				 * avoid damage. <BR><BR>
				 *
				 * I've provided you with several heal potions in case you
				 * become injured. <BR><BR>
				 * 
				 * In the bag you will also find more clothing appropriate to
				 * your new found profession. <BR><BR>
				 * 
				 * Please return the sword to me. I implore you not to
				 * take anything else that may be hidden in the Inn. <BR><BR>
				 *
				 * Thank you.
				 */
				return 1063199;
			}
		}

		public override bool Logged { get { return false; } }

		public FrownsConversation()
		{
		}
	}

	public class NarrowsConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* A narrow hallway greets the teleporter. The enclosed
				 * space is the perfect setting for dangerous traps. Walk
				 * through the hallway being careful to avoid the traps. You
				 * may be able to time the traps to avoid injury.
				 */
				return 1063201;
			}
		}

		public override bool Logged { get { return false; } }

		public NarrowsConversation()
		{
		}
	}

	public class OpenChestConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* The lid of the chest refuses to budge at first, but
				 * slowly you are able to pry the lid open. <BR><BR>
				 * 
				 * Inside lies the sword you have been in search of.  You
				 * quickly take the sword and stash it in your backpack.
				 * Bring the sword back to Daimyo Emino.
				 */
				return 1063203;
			}
		}

		public override bool Logged { get { return false; } }

		public OpenChestConversation()
		{
		}
	}

	public class ScreamsEchoConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* Screams echo through the chamber as you walk away
				 * from the chest. Jedah's Henchmen have become cognizant
				 * of your presence. <BR><BR>
				 * 
				 * It is time for your Ninja Spirit to come alive. Slay 3
				 * of the Henchmen before returning to Daimyo Emino.
				 */
				return 1063205;
			}
		}

		public override bool Logged { get { return false; } }

		public ScreamsEchoConversation()
		{
		}
	}


	public class GoToEminoConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* Go to Daimyo Emino. Go back through the chamber the
				 * way you came.<BR><BR>
				 * 
				 *  Give Daimyo Emino the sword when you've returned to
				 * his side.
				 */
				return 1063211;
			}
		}

		public override bool Logged { get { return false; } }

		public GoToEminoConversation()
		{
		}
	}

	public class ContinueKillHenchmensConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				// Continue killing the henchmen!
				return 1063208;
			}
		}

		public override bool Logged { get { return false; } }

		public ContinueKillHenchmensConversation()
		{
		}
	}

	public class TakeSwordAgainConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* What? You have returned without the sword? You need
				 * to go back and get it again!
				 */
				return 1063212;
			}
		}

		public override bool Logged { get { return false; } }

		public TakeSwordAgainConversation()
		{
		}
	}

	public class GiftsConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* Beyond this path lies Zento City, your future home. To
				 * the right of the cave entrance you will find a luminous
				 * oval object known as a Moongate, step through it and
				 * you'll find yourself in Zento.<BR><BR>
				 *
				 * You may want to visit Ansella Gryen when you arrive. <BR><BR>
				 * 
				 * Please accept the gifts I have placed in your pack. You
				 * have earned them. Farewell for now.
				 */
				return 1063216;
			}
		}

		public GiftsConversation()
		{
		}
	}
}