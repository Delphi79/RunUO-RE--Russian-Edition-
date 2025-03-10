using System;
using Server;

namespace Server.Items
{
	public class LysanderNotebook1 : BaseBook
	{
		[Constructable]
		public LysanderNotebook1() : base( Utility.Random( 0xFF1, 2 ), "Lysander's Notebook", "L. Gathenwale", 3, false )
		{
			Pages[ 0 ].Lines = new string[] {"Day One :", "", "At last, it stands", "before me.  The doors", "of Thy Sanctum will", "open to me now, after", "all these years of", "searching.  I give"};

			Pages[ 1 ].Lines = new string[] {"myself unto Thee,", "Khal Ankur, I have", "come for Thy secrets", "and I will kneel", "prostrate before Thee.", " Blessed are the", "Keepers, praise unto", "Thee, a thousand"};

			Pages[ 2 ].Lines = new string[] {"fortunes in the night."};
		}

		public LysanderNotebook1( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class LysanderNotebook2 : BaseBook
	{
		[Constructable]
		public LysanderNotebook2() : base( Utility.Random( 0xFF1, 2 ), "Lysander's Notebook", "L. Gathenwale", 6, false )
		{
			Pages[ 0 ].Lines = new string[] {"Day Two:", "", "The woman, Tavara", "Sewel, is unbearable.", "Her entire demeanor", "sickens me.  I would", "take her life for Thee", "now, my Lord.  But I"};

			Pages[ 1 ].Lines = new string[] {"cannot alert the", "others.  Progress is", "made too slowly, I", "cannot stand this", "perpetual waiting.", "Today I knelt down", "with the workers,", "tossing stones and dirt"};

			Pages[ 2 ].Lines = new string[] {"aside with my very", "hands as they dug at", "the last of the rubble", "covering the entrance", "to Thy Sanctum.  The", "Sewel woman was", "shocked at my", "demeanor, dirtying"};

			Pages[ 3 ].Lines = new string[] {"my robes, on my", "knees in the muck as I", "clawed at the rocks.", "She thought I did this", "for those sickly", "scholars, or for her,", "or for what she", "laughably calls 'The"};

			Pages[ 4 ].Lines = new string[] {"Gift of Discovery', of", "learning.  As if I did", "not know what I went", "to find!  I come for", "Thee, Master.  Soon", "shall I receive Thy", "gifts, Thy blessings.", "Patience, eternal"};

			Pages[ 5 ].Lines = new string[] {"patience.  I must take", "my lessons well.  I", "have learned from", "Thee, Master, I have."};
		}

		public LysanderNotebook2( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class LysanderNotebook3 : BaseBook
	{
		[Constructable]
		public LysanderNotebook3() : base( Utility.Random( 0xFF1, 2 ), "Lysander's Notebook", "L. Gathenwale", 5, false )
		{
			Pages[ 0 ].Lines = new string[] {"Day Three - Day Six:", "", "What are these Beasts", "that dare to defy our", "presence here?  Hast", "Thou sent them,", "Master?  To tear", "apart these foolish"};

			Pages[ 1 ].Lines = new string[] {"ones that accompany", "me?  That repugnant", "pustule, Drummel, put", "forth his absurd little", "theories as to the", "nature of the Beasts", "that attacked our", "camp, but I'll have"};

			Pages[ 2 ].Lines = new string[] {"none of his words.  He", "asks too many", "questions.  He is a", "taint upon the grounds", "of Thy Sanctum,", "Master - I will deal", "with him after the", "Sewel woman."};

			Pages[ 3 ].Lines = new string[] {"Speaking of Sewel, I", "have convinced that", "empty-headed harlot", "that we should move", "our encampment", "within the", "antechamber.  She", "thinks I worry for"};

			Pages[ 4 ].Lines = new string[] {"her safety.  I come", "for thee, Master.  I", "make my camp in Thy", "chambers.  I sleep", "under Thy roof.  I can", "feel Thine presence", "even now.  Soon,", "Master.  Soon."};
		}

		public LysanderNotebook3( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class LysanderNotebook7 : BaseBook
	{
		[Constructable]
		public LysanderNotebook7() : base( Utility.Random( 0xFF1, 2 ), "Lysander's Notebook", "L. Gathenwale", 7, false )
		{
			Pages[ 0 ].Lines = new string[] {"Day Seven :", "", "The Sewel woman", "pratters on endlessly.", "And she dares to", "speak Thy Name,", "Master!  I wish so", "vehemently to take a"};

			Pages[ 1 ].Lines = new string[] {"knife to that little", "neck of hers.  She", "struts around the", "chambers of Thy", "Sanctum with her", "repugnant airs, her", "scholarly conjecture", "on this or that.  That I"};

			Pages[ 2 ].Lines = new string[] {"could peel the skin", "from her face and", "show her how vile and", "ugly she truly is, how", "unworthy of entrance", "to Thy Sanctum.  I", "must take her,", "Master.  I must rend"};

			Pages[ 3 ].Lines = new string[] {"that little wench to", "pieces.  I ask this gift", "of Thee, that I might", "cleanse Thy Sanctum", "of her presence.  Give", "me the Sewel woman", "and I shall show you", "my mastery of Death,"};

			Pages[ 4 ].Lines = new string[] {"Master.  I shall cut", "her to bits and scatter", "them before the", "others as a warning.", "I cannot stand her", "presence, I cannot", "abide it.  And", "Drummel!  He is a"};

			Pages[ 5 ].Lines = new string[] {"pustule that must be", "lanced, a sickness that", "I must cure by blade", "and fire.  Not a trace", "of him will be left", "when I'm done with", "him.  Praises to Thee,", "Master.  I shall honor"};

			Pages[ 6 ].Lines = new string[] {"Thee with many", "sacrifices, soon", "enough."};
		}

		public LysanderNotebook7( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class LysanderNotebook8 : BaseBook
	{
		[Constructable]
		public LysanderNotebook8() : base( Utility.Random( 0xFF1, 2 ), "Lysander's Notebook", "L. Gathenwale", 6, false )
		{
			Pages[ 0 ].Lines = new string[] {"Day Eight - Day Ten :", "", "Have you taken them,", "Master?  They could", "not have found a way", "past the stones that", "block our path!  The", "three workers, My"};

			Pages[ 1 ].Lines = new string[] {"Master, where have", "they gone?  Curses", "upon them!  I'll cut", "them all to pieces if", "they show their faces", "again, then burn the", "rest alive upon a pyre,", "for all to see, as a"};

			Pages[ 2 ].Lines = new string[] {"warning of Thy", "Power.  How could", "they have gotten past", "me?  I sleep against", "the very walls, to", "hear Thy Words, to", "feel Thy Breath.  I", "can find no egress"};

			Pages[ 3 ].Lines = new string[] {"from the chambers", "that the Sewel woman", "does not know of nor", "have men working at", "excavating.  Where", "have they gone,", "Master?  Have you", "taken them, or do they"};

			Pages[ 4 ].Lines = new string[] {"truly flee from Thy", "Presence?  I will kill", "them if they show", "their faces again.", "Give me Strength, my", "Master, to let them", "live a while longer,", "until they have"};

			Pages[ 5 ].Lines = new string[] {"fulfilled their", "purpose and I kneel", "before Thee, covered", "in their blood."};
		}

		public LysanderNotebook8( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class LysanderNotebook11 : BaseBook
	{
		[Constructable]
		public LysanderNotebook11() : base( Utility.Random( 0xFF1, 2 ), "Lysander's Notebook", "L. Gathenwale", 6, false )
		{
			Pages[ 0 ].Lines = new string[] {"Day Eleven - Day", "Thirteen:", "", "I come for Thee, my", "Master.  I come!  The", "way is clear, I have", "found Thy path and", "washed it in the blood"};

			Pages[ 1 ].Lines = new string[] {"of the two workers", "that caught sight of", "me.  Ah, how sweet it", "was to cut them open,", "to see the blood pour", "out in great torrents, to", "stand in it, to revel in", "it.  If only I had time"};

			Pages[ 2 ].Lines = new string[] {"for the Sewel woman.", "But there will be time", "enough for her.  I", "have learned Thy", "Patience, Master.  I", "come for Thee.  I walk", "Thy halls in penance,", "my last steps in this"};

			Pages[ 3 ].Lines = new string[] {"repulsive living", "frame.  I come for", "Thee and Thy Gifts,", "my Master.  Glory", "Unto Thee, Khal", "Ankur, Keeper of the", "Seventh Death,", "Master, Leader of the"};

			Pages[ 4 ].Lines = new string[] {"Chosen, the Khaldun.", "Praises in Thy", "Name, Master of Life", "and Death, Lord of All.", " Khal Ankur, Master,", "Prophet, I join Thy", "ranks this night, a", "member of the"};

			Pages[ 5 ].Lines = new string[] {"Khaldun at last!"};
		}

		public LysanderNotebook11( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}