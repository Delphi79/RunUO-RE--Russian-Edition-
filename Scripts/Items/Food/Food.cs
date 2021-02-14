using System;
using System.Collections;
using Server.Network;

namespace Server.Items
{
	public abstract class Food : Item
	{
		private Mobile m_Poisoner;
		private Poison m_Poison;
		private int m_FillFactor;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Poisoner { get { return m_Poisoner; } set { m_Poisoner = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Poison Poison { get { return m_Poison; } set { m_Poison = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FillFactor { get { return m_FillFactor; } set { m_FillFactor = value; } }

		public Food( int itemID ) : this( 1, itemID )
		{
		}

		public Food( int amount, int itemID ) : base( itemID )
		{
			Stackable = true;
			Amount = amount;
			m_FillFactor = 1;
		}

		public Food( Serial serial ) : base( serial )
		{
		}

		public override void GetContextMenuEntries( Mobile from, ArrayList list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive )
			{
				list.Add( new ContextMenus.EatEntry( from, this ) );
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
			{
				return;
			}

			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				Eat( from );
			}
		}

		public virtual bool Eat( Mobile from )
		{
			// Fill the Mobile with FillFactor
			if ( FillHunger( from, m_FillFactor ) )
			{
				// Play a random "eat" sound
				from.PlaySound( Utility.Random( 0x3A, 3 ) );

				if ( from.Body.IsHuman && !from.Mounted )
				{
					from.Animate( 34, 5, 1, true, false, 0 );
				}

				if ( m_Poison != null )
				{
					from.ApplyPoison( m_Poisoner, m_Poison );
				}

				Consume();

				return true;
			}

			return false;
		}

		public static bool FillHunger( Mobile from, int fillFactor )
		{
			if ( from.Hunger >= 20 )
			{
				from.SendLocalizedMessage( 500867 ); // You are simply too full to eat any more!
				return false;
			}

			int iHunger = from.Hunger + fillFactor;
			if ( from.Stam < from.StamMax )
			{
				from.Stam += Utility.Random( 6, 3 ) + fillFactor/5; //restore some stamina
			}
			if ( iHunger >= 20 )
			{
				from.Hunger = 20;
				from.SendLocalizedMessage( 500872 ); // You manage to eat the food, but you are stuffed!
			}
			else
			{
				from.Hunger = iHunger;

				if ( iHunger < 5 )
				{
					from.SendLocalizedMessage( 500868 ); // You eat the food, but are still extremely hungry.
				}
				else if ( iHunger < 10 )
				{
					from.SendLocalizedMessage( 500869 ); // You eat the food, and begin to feel more satiated.
				}
				else if ( iHunger < 15 )
				{
					from.SendLocalizedMessage( 500870 ); // After eating the food, you feel much less hungry.
				}
				else
				{
					from.SendLocalizedMessage( 500871 ); // You feel quite full after consuming the food.
				}
			}

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 4 ); // version

			writer.Write( m_Poisoner );

			Poison.Serialize( m_Poison, writer );
			writer.Write( m_FillFactor );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
					{
						switch ( reader.ReadInt() )
						{
							case 0:
								m_Poison = null;
								break;
							case 1:
								m_Poison = Poison.Lesser;
								break;
							case 2:
								m_Poison = Poison.Regular;
								break;
							case 3:
								m_Poison = Poison.Greater;
								break;
							case 4:
								m_Poison = Poison.Deadly;
								break;
						}

						break;
					}
				case 2:
					{
						m_Poison = Poison.Deserialize( reader );
						break;
					}
				case 3:
					{
						m_Poison = Poison.Deserialize( reader );
						m_FillFactor = reader.ReadInt();
						break;
					}
				case 4:
					{
						m_Poisoner = reader.ReadMobile();
						goto case 3;
					}
			}
		}
	}

	[FlipableAttribute( 0x15F9, 0x15FE )]
	public class BowlOfCarrots : Food
	{
		public override int LabelNumber { get { return 1025625; } } // bowl of carrots

		[Constructable]
		public BowlOfCarrots() : this( 1 )
		{
		}

		[Constructable]
		public BowlOfCarrots( int amount ) : base( 0x15F9 )
		{
			this.Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public BowlOfCarrots( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new BowlOfCarrots(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x15FA, 0x15FF )]
	public class BowlOfCorn : Food
	{
		public override int LabelNumber { get { return 1025626; } } // bowl of corn

		[Constructable]
		public BowlOfCorn() : this( 1 )
		{
		}

		[Constructable]
		public BowlOfCorn( int amount ) : base( 0x15FA )
		{
			this.Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public BowlOfCorn( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new BowlOfCorn(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x15FB, 0x1600 )]
	public class BowlOfLettuce : Food
	{
		public override int LabelNumber { get { return 1025627; } } // bowl of lettuce

		[Constructable]
		public BowlOfLettuce() : this( 1 )
		{
		}

		[Constructable]
		public BowlOfLettuce( int amount ) : base( 0x15FB )
		{
			this.Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public BowlOfLettuce( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new BowlOfLettuce(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x15FC, 0x1601 )]
	public class BowlOfPeas : Food
	{
		public override int LabelNumber { get { return 1025628; } } // bowl of peas

		[Constructable]
		public BowlOfPeas() : this( 1 )
		{
		}

		[Constructable]
		public BowlOfPeas( int amount ) : base( 0x15FC )
		{
			this.Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public BowlOfPeas( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new BowlOfPeas(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class BowlOfPotatoes : Food
	{
		public override int LabelNumber { get { return 1025634; } } // bowl of potatoes

		[Constructable]
		public BowlOfPotatoes() : this( 1 )
		{
		}

		[Constructable]
		public BowlOfPotatoes( int amount ) : base( 0x1602 )
		{
			this.Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public BowlOfPotatoes( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new BowlOfPotatoes(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class BowlOfStew : Food
	{
		public override int LabelNumber { get { return 1025636; } } // bowl of stew

		[Constructable]
		public BowlOfStew() : this( 1 )
		{
		}

		[Constructable]
		public BowlOfStew( int amount ) : base( 0x1604 )
		{
			this.Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public BowlOfStew( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new BowlOfStew(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class TomatoSoup : Food
	{
		public override int LabelNumber { get { return 1025638; } } // tomato soup

		[Constructable]
		public TomatoSoup() : this( 1 )
		{
		}

		[Constructable]
		public TomatoSoup( int amount ) : base( 0x1606 )
		{
			this.Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public TomatoSoup( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new TomatoSoup(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class PewterBowl : Food
	{
		public override int LabelNumber { get { return 1025629; } } // pewter bowl

		[Constructable]
		public PewterBowl() : this( 1 )
		{
		}

		[Constructable]
		public PewterBowl( int amount ) : base( 0x15FD )
		{
			this.Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public PewterBowl( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new PewterBowl(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x2836, 0x2837 )]
	public class BentoBox : Food
	{
		public override int LabelNumber { get { return 1030292; } } // bento box

		[Constructable]
		public BentoBox() : this( 1 )
		{
		}

		[Constructable]
		public BentoBox( int amount ) : base( 0x2836 )
		{
			Stackable = false;
			Weight = 5.0;
			FillFactor = 2;
		}

		public BentoBox( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new BentoBox(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class WasabiClumps : Food
	{
		public override int LabelNumber { get { return 1029451; } } // wasabi clumps

		[Constructable]
		public WasabiClumps() : this( 1 )
		{
		}

		[Constructable]
		public WasabiClumps( int amount ) : base( 0x24EB )
		{
			Stackable = false;
			Weight = 1.0;
			FillFactor = 2;
		}

		public WasabiClumps( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new WasabiClumps(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x24E8, 0x24E9 )]
	public class Wasabi : Food
	{
		public override int LabelNumber { get { return 1029449; } } // wasabi

		[Constructable]
		public Wasabi() : this( 1 )
		{
		}

		[Constructable]
		public Wasabi( int amount ) : base( 0x24E8 )
		{
			Stackable = false;
			Weight = 1.0;
			FillFactor = 2;
		}

		public Wasabi( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Wasabi(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class GreenTeaBasket : Item
	{
		[Constructable]
		public GreenTeaBasket() : base( 0x284B )
		{
			Weight = 10.0;
		}

		public GreenTeaBasket( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class GreenTea : Food
	{
		public override int LabelNumber { get { return 1030316; } } // green tea

		[Constructable]
		public GreenTea() : this( 1 )
		{
		}

		[Constructable]
		public GreenTea( int amount ) : base( 0x284C )
		{
			Stackable = false;
			Weight = 4.0;
			FillFactor = 2;
		}

		public GreenTea( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new GreenTea(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x2840, 0x2841 )]
	public class SushiPlatter : Food
	{
		public override int LabelNumber { get { return 1030305; } } // sushi platter

		[Constructable]
		public SushiPlatter() : this( 1 )
		{
		}

		[Constructable]
		public SushiPlatter( int amount ) : base( 0x2840 )
		{
			Stackable = false;
			Weight = 3.0;
			FillFactor = 2;
		}

		public SushiPlatter( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new SushiPlatter(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x283E, 0x283F )]
	public class SushiRolls : Food
	{
		public override int LabelNumber { get { return 1030303; } } // sushi rolls

		[Constructable]
		public SushiRolls() : this( 1 )
		{
		}

		[Constructable]
		public SushiRolls( int amount ) : base( 0x283E )
		{
			Stackable = false;
			Weight = 3.0;
			FillFactor = 2;
		}

		public SushiRolls( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new SushiRolls(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class AwaseMisoSoup : Food
	{
		public override int LabelNumber { get { return 1030320; } } // awase miso soup

		[Constructable]
		public AwaseMisoSoup() : this( 1 )
		{
		}

		[Constructable]
		public AwaseMisoSoup( int amount ) : base( 0x2850 )
		{
			Stackable = false;
			Weight = 4.0;
			FillFactor = 2;
		}

		public AwaseMisoSoup( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new AwaseMisoSoup(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class RedMisoSoup : Food
	{
		public override int LabelNumber { get { return 1030319; } } // red miso soup

		[Constructable]
		public RedMisoSoup() : this( 1 )
		{
		}

		[Constructable]
		public RedMisoSoup( int amount ) : base( 0x284F )
		{
			Stackable = false;
			Weight = 4.0;
			FillFactor = 2;
		}

		public RedMisoSoup( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new RedMisoSoup(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class WhiteMisoSoup : Food
	{
		public override int LabelNumber { get { return 1030318; } } // white miso soup

		[Constructable]
		public WhiteMisoSoup() : this( 1 )
		{
		}

		[Constructable]
		public WhiteMisoSoup( int amount ) : base( 0x284E )
		{
			Stackable = false;
			Weight = 4.0;
			FillFactor = 2;
		}

		public WhiteMisoSoup( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new WhiteMisoSoup(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class MisoSoup : Food
	{
		public override int LabelNumber { get { return 1030317; } } // miso soup

		[Constructable]
		public MisoSoup() : this( 1 )
		{
		}

		[Constructable]
		public MisoSoup( int amount ) : base( 0x284D )
		{
			Stackable = false;
			Weight = 4.0;
			FillFactor = 2;
		}

		public MisoSoup( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new MisoSoup(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class BreadLoaf : Food
	{
		[Constructable]
		public BreadLoaf() : this( 1 )
		{
		}

		[Constructable]
		public BreadLoaf( int amount ) : base( amount, 0x103B )
		{
			this.Weight = 1.0;
			this.FillFactor = 3;
		}

		public BreadLoaf( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new BreadLoaf(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Bacon : Food
	{
		[Constructable]
		public Bacon() : this( 1 )
		{
		}

		[Constructable]
		public Bacon( int amount ) : base( amount, 0x979 )
		{
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public Bacon( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Bacon(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class SlabOfBacon : Food
	{
		[Constructable]
		public SlabOfBacon() : this( 1 )
		{
		}

		[Constructable]
		public SlabOfBacon( int amount ) : base( amount, 0x976 )
		{
			this.Weight = 1.0;
			this.FillFactor = 3;
		}

		public SlabOfBacon( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new SlabOfBacon(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class FishSteak : Food
	{
		[Constructable]
		public FishSteak() : this( 1 )
		{
		}

		[Constructable]
		public FishSteak( int amount ) : base( amount, 0x97B )
		{
			this.Weight = 0.1;
			this.FillFactor = 3;
		}

		public FishSteak( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new FishSteak(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class CheeseWheel : Food
	{
		[Constructable]
		public CheeseWheel() : this( 1 )
		{
		}

		[Constructable]
		public CheeseWheel( int amount ) : base( amount, 0x97E )
		{
			this.Weight = 0.1;
			this.FillFactor = 3;
		}

		public CheeseWheel( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new CheeseWheel(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class CheeseWedge : Food
	{
		[Constructable]
		public CheeseWedge() : this( 1 )
		{
		}

		[Constructable]
		public CheeseWedge( int amount ) : base( amount, 0x97D )
		{
			this.Weight = 0.1;
			this.FillFactor = 3;
		}

		public CheeseWedge( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new CheeseWedge(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class CheeseSlice : Food
	{
		[Constructable]
		public CheeseSlice() : this( 1 )
		{
		}

		[Constructable]
		public CheeseSlice( int amount ) : base( amount, 0x97C )
		{
			this.Weight = 0.1;
			this.FillFactor = 1;
		}

		public CheeseSlice( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new CheeseSlice(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class FrenchBread : Food
	{
		[Constructable]
		public FrenchBread() : this( 1 )
		{
		}

		[Constructable]
		public FrenchBread( int amount ) : base( amount, 0x98C )
		{
			this.Weight = 2.0;
			this.FillFactor = 3;
		}

		public FrenchBread( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new FrenchBread(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class FriedEggs : Food
	{
		[Constructable]
		public FriedEggs() : this( 1 )
		{
		}

		[Constructable]
		public FriedEggs( int amount ) : base( amount, 0x9B6 )
		{
			this.Weight = 1.0;
			this.FillFactor = 4;
		}

		public FriedEggs( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new FriedEggs(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class CookedBird : Food
	{
		[Constructable]
		public CookedBird() : this( 1 )
		{
		}

		[Constructable]
		public CookedBird( int amount ) : base( amount, 0x9B7 )
		{
			this.Weight = 1.0;
			this.FillFactor = 5;
		}

		public CookedBird( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new CookedBird(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class RoastPig : Food
	{
		[Constructable]
		public RoastPig() : this( 1 )
		{
		}

		[Constructable]
		public RoastPig( int amount ) : base( amount, 0x9BB )
		{
			this.Weight = 45.0;
			this.FillFactor = 20;
		}

		public RoastPig( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new RoastPig(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Sausage : Food
	{
		[Constructable]
		public Sausage() : this( 1 )
		{
		}

		[Constructable]
		public Sausage( int amount ) : base( amount, 0x9C0 )
		{
			this.Weight = 1.0;
			this.FillFactor = 4;
		}

		public Sausage( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Sausage(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Ham : Food
	{
		[Constructable]
		public Ham() : this( 1 )
		{
		}

		[Constructable]
		public Ham( int amount ) : base( amount, 0x9C9 )
		{
			this.Weight = 1.0;
			this.FillFactor = 5;
		}

		public Ham( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Ham(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Cake : Food
	{
		[Constructable]
		public Cake() : base( 0x9E9 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 10;
		}

		public Cake( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Ribs : Food
	{
		[Constructable]
		public Ribs() : this( 1 )
		{
		}

		[Constructable]
		public Ribs( int amount ) : base( amount, 0x9F2 )
		{
			this.Weight = 1.0;
			this.FillFactor = 5;
		}

		public Ribs( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Ribs(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Cookies : Food
	{
		[Constructable]
		public Cookies() : base( 0x160b )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 4;
		}

		public Cookies( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Muffins : Food
	{
		[Constructable]
		public Muffins() : base( 0x9eb )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 4;
		}

		public Muffins( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[TypeAlias( "Server.Items.Pizza" )]
	public class CheesePizza : Food
	{
		public override int LabelNumber { get { return 1044516; } } // cheese pizza

		[Constructable]
		public CheesePizza() : base( 0x1040 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 6;
		}

		public CheesePizza( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class SausagePizza : Food
	{
		public override int LabelNumber { get { return 1044517; } } // sausage pizza

		[Constructable]
		public SausagePizza() : base( 0x1040 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 6;
		}

		public SausagePizza( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

#if false
	public class Pizza : Food
	{
		[Constructable]
		public Pizza() : base( 0x1040 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 6;
		}

		public Pizza( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
#endif

	public class FruitPie : Food
	{
		public override int LabelNumber { get { return 1041346; } } // baked fruit pie

		[Constructable]
		public FruitPie() : base( 0x1041 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 5;
		}

		public FruitPie( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class MeatPie : Food
	{
		public override int LabelNumber { get { return 1041347; } } // baked meat pie

		[Constructable]
		public MeatPie() : base( 0x1041 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 5;
		}

		public MeatPie( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class PumpkinPie : Food
	{
		public override int LabelNumber { get { return 1041348; } } // baked pumpkin pie

		[Constructable]
		public PumpkinPie() : base( 0x1041 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 5;
		}

		public PumpkinPie( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class ApplePie : Food
	{
		public override int LabelNumber { get { return 1041343; } } // baked apple pie

		[Constructable]
		public ApplePie() : base( 0x1041 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 5;
		}

		public ApplePie( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class PeachCobbler : Food
	{
		public override int LabelNumber { get { return 1041344; } } // baked peach cobbler

		[Constructable]
		public PeachCobbler() : base( 0x1041 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 5;
		}

		public PeachCobbler( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Quiche : Food
	{
		public override int LabelNumber { get { return 1041345; } } // baked quiche

		[Constructable]
		public Quiche() : base( 0x1041 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 5;
		}

		public Quiche( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class LambLeg : Food
	{
		[Constructable]
		public LambLeg() : this( 1 )
		{
		}

		[Constructable]
		public LambLeg( int amount ) : base( amount, 0x160a )
		{
			this.Weight = 2.0;
			this.FillFactor = 5;
		}

		public LambLeg( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new LambLeg(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class ChickenLeg : Food
	{
		[Constructable]
		public ChickenLeg() : this( 1 )
		{
		}

		[Constructable]
		public ChickenLeg( int amount ) : base( amount, 0x1608 )
		{
			this.Weight = 1.0;
			this.FillFactor = 4;
		}

		public ChickenLeg( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new ChickenLeg(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0xC74, 0xC75 )]
	public class HoneydewMelon : Food
	{
		[Constructable]
		public HoneydewMelon() : this( 1 )
		{
		}

		[Constructable]
		public HoneydewMelon( int amount ) : base( amount, 0xC74 )
		{
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public HoneydewMelon( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new HoneydewMelon(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0xC64, 0xC65 )]
	public class YellowGourd : Food
	{
		[Constructable]
		public YellowGourd() : this( 1 )
		{
		}

		[Constructable]
		public YellowGourd( int amount ) : base( amount, 0xC64 )
		{
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public YellowGourd( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new YellowGourd(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0xC66, 0xC67 )]
	public class GreenGourd : Food
	{
		[Constructable]
		public GreenGourd() : this( 1 )
		{
		}

		[Constructable]
		public GreenGourd( int amount ) : base( amount, 0xC66 )
		{
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public GreenGourd( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new GreenGourd(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0xC7F, 0xC81 )]
	public class EarOfCorn : Food
	{
		[Constructable]
		public EarOfCorn() : this( 1 )
		{
		}

		[Constructable]
		public EarOfCorn( int amount ) : base( amount, 0xC81 )
		{
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public EarOfCorn( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new EarOfCorn(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Turnip : Food
	{
		[Constructable]
		public Turnip() : this( 1 )
		{
		}

		[Constructable]
		public Turnip( int amount ) : base( amount, 0xD3A )
		{
			this.Weight = 1.0;
			this.FillFactor = 1;
		}

		public Turnip( Serial serial ) : base( serial )
		{
		}

		public override Item Dupe( int amount )
		{
			return base.Dupe( new Turnip(), amount );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class SheafOfHay : Item
	{
		[Constructable]
		public SheafOfHay() : base( 0xF36 )
		{
			this.Weight = 10.0;
		}

		public SheafOfHay( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}