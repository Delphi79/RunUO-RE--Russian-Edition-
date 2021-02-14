using System;
using System.Collections;
using Server.Multis;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public abstract class StealableArtifact : Item
	{
		public override bool ForceShowProperties { get { return true; } }

		public StealableArtifact( int itemID ) : base( itemID )
		{
			Stackable = false;

			Weight = 10.0;

			Movable = false;
		}

		public StealableArtifact( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int ArtifactRarity { get { return 0; } }

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( ArtifactRarity > 0 )
			{
				list.Add( 1061078, ArtifactRarity.ToString() );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			if ( Weight != 10 )
			{
				Weight = 10;
			}
		}
	}

	public abstract class StealableContainerArtifact : BaseContainer
	{
		public override bool ForceShowProperties { get { return true; } }

		public StealableContainerArtifact( int itemID ) : base( itemID )
		{
			Stackable = false;

			Movable = false;

			Weight = 10.0;
		}

		public StealableContainerArtifact( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int ArtifactRarity { get { return 0; } }

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( ArtifactRarity > 0 )
			{
				list.Add( 1061078, ArtifactRarity.ToString() );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			if ( Weight != 10 )
			{
				Weight = 10;
			}
		}
	}

	public abstract class StealableLightArtifact : BaseLight
	{
		public override bool ForceShowProperties { get { return true; } }

		public StealableLightArtifact( int itemID ) : base( itemID )
		{
			Stackable = false;

			Movable = false;

			Weight = 10.0;
		}

		public StealableLightArtifact( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int ArtifactRarity { get { return 0; } }

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( ArtifactRarity > 0 )
			{
				list.Add( 1061078, ArtifactRarity.ToString() );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			if ( Weight != 10 )
			{
				Weight = 10;
			}
		}
	}

	public abstract class StealableLongswordArtifact : Longsword
	{
		public override bool ForceShowProperties { get { return true; } }

		public StealableLongswordArtifact() : base()
		{
			Stackable = false;

			Movable = false;

			Weight = 10.0;
		}

		public StealableLongswordArtifact( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			if ( Weight != 10 )
			{
				Weight = 10;
			}
		}
	}

	public abstract class StealablePlateGlovesArtifact : PlateGloves
	{
		public override bool ForceShowProperties { get { return true; } }

		public StealablePlateGlovesArtifact() : base()
		{
			Stackable = false;

			Movable = false;

			Weight = 10.0;
		}

		public StealablePlateGlovesArtifact( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			if ( Weight != 10 )
			{
				Weight = 10;
			}
		}
	}

	public abstract class StealableWarHammerArtifact : WarHammer
	{
		public override bool ForceShowProperties { get { return true; } }

		public StealableWarHammerArtifact() : base()
		{
			Stackable = false;

			Movable = false;

			Weight = 10.0;
		}

		public StealableWarHammerArtifact( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			if ( Weight != 10 )
			{
				Weight = 10;
			}
		}
	}

	public abstract class StealableExecutionersAxeArtifact : ExecutionersAxe
	{
		public override bool ForceShowProperties { get { return true; } }

		public StealableExecutionersAxeArtifact() : base()
		{
			Stackable = false;

			Movable = false;

			Weight = 10.0;
		}

		public StealableExecutionersAxeArtifact( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			if ( Weight != 10 )
			{
				Weight = 10;
			}
		}
	}

	public abstract class StealableFoodArtifact : Food
	{
		public override bool ForceShowProperties { get { return true; } }

		public StealableFoodArtifact( int i, int id ) : base( i, id )
		{
			Stackable = false;

			Movable = false;

			Weight = 10.0;
		}

		public StealableFoodArtifact( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int ArtifactRarity { get { return 0; } }

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( ArtifactRarity > 0 )
			{
				list.Add( 1061078, ArtifactRarity.ToString() );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			if ( Weight != 10 )
			{
				Weight = 10;
			}
		}
	}
}