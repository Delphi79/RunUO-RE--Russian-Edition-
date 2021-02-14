using System;
using Server;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public class PigmentsOfTokuno : Item, IUsesRemaining
	{
		public override int LabelNumber { get { return 1070933; } } // Pigments of Tokuno

		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set
			{
				m_UsesRemaining = value;
				InvalidateProperties();
			}
		}

		public bool ShowUsesRemaining
		{
			get { return true; }
			set
			{
			}
		}

		[Constructable]
		public PigmentsOfTokuno() : base( 0xEFF )
		{
			m_UsesRemaining = 10;
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendLocalizedMessage( 1070929 ); // Select the artifact or enhanced magic item to dye.

			from.Target = new DyeTarget( this );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public PigmentsOfTokuno( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_UsesRemaining = reader.ReadInt();
		}

		private class DyeTarget : Target
		{
			private PigmentsOfTokuno dye;

			public DyeTarget( PigmentsOfTokuno m_dye ) : base( 8, false, TargetFlags.None )
			{
				dye = m_dye;
			}

			public bool CheckWarn( Item item )
			{
				if ( item is BaseWeapon && ((BaseWeapon) item).Hits < ((BaseWeapon) item).MaxHits )
				{
					return false;
				}

				if ( item is BaseArmor && ((BaseArmor) item).HitPoints < ((BaseArmor) item).MaxHitPoints )
				{
					return false;
				}

				if ( item is BaseClothing && ((BaseClothing) item).HitPoints < ((BaseClothing) item).MaxHitPoints )
				{
					return false;
				}

				return true;
			}

			public bool CanHue( Item item )
			{
				if ( item is BaseWeapon && (((BaseWeapon) item).ArtifactRarity > 0 || !CraftResources.IsStandard( ((BaseWeapon) item).Resource )) )
				{
					return true;
				}

				if ( item is BaseArmor && (((BaseArmor) item).ArtifactRarity > 0 || !CraftResources.IsStandard( ((BaseArmor) item).Resource )) )
				{
					return true;
				}

				if ( item is BaseClothing && ((BaseClothing) item).ArtifactRarity > 0 )
				{
					return true;
				}

				if ( item is BaseJewel && ((BaseJewel) item).ArtifactRarity > 0 )
				{
					return true;
				}

				for ( int i = 0; i < Paragon.Artifacts.Length; i++ )
				{
					Type type = Paragon.Artifacts[ i ];

					if ( type == item.GetType() )
					{
						return true;
					}
				}

				for ( int i = 0; i < Leviathan.Artifacts.Length; i++ )
				{
					Type type = Leviathan.Artifacts[ i ];

					if ( type == item.GetType() )
					{
						return true;
					}
				}

				for ( int i = 0; i < TreasureMapChest.m_Artifacts.Length; i++ )
				{
					Type type = TreasureMapChest.m_Artifacts[ i ];

					if ( type == item.GetType() )
					{
						return true;
					}
				}

				for ( int i = 0; i < TokunoTreasures.MinorArtifacts.Length; i++ )
				{
					Type type = TokunoTreasures.MinorArtifacts[ i ];

					if ( type == item.GetType() )
					{
						return true;
					}
				}

				for ( int i = 0; i < TokunoTreasures.MajorArtifacts.Length; i++ )
				{
					Type type = TokunoTreasures.MajorArtifacts[ i ];

					if ( type == item.GetType() )
					{
						return true;
					}
				}

				return false;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				Item item = targeted as Item;

				if ( item == null )
				{
					return;
				}

				if ( !item.IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1062334 ); // This item must be in your backpack to be used.
				}
				else if ( item is PigmentsOfTokuno || item is PigmentsOfTokunoMajor )
				{
					from.SendLocalizedMessage( 1042083 ); // You cannot dye that.
				}
				else if ( item.IsLockedDown )
				{
					from.SendLocalizedMessage( 1070932 ); // You may not dye artifacts and enhanced magic items which are locked down.
				}
				else if ( !CheckWarn( item ) )
				{
					from.SendLocalizedMessage( 1070930 ); // Can't dye artifacts or enhanced magic items that are being worn.
				}
				else if ( CanHue( item ) )
				{
					item.Hue = 0;

					dye.UsesRemaining--;

					if ( dye.UsesRemaining <= 0 )
					{
						dye.Delete();
					}
				}
				else
				{
					from.SendLocalizedMessage( 1070931 ); // You can only dye artifacts and enhanced magic items with this tub.
				}
			}
		}
	}
}