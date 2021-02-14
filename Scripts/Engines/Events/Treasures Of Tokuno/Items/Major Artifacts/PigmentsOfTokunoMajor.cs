using System;
using Server;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public enum PigmentsType
	{
		ParagonGold,
		VioletCouragePurple,
		InvulnerabilityBlue,
		LunaWhite,
		DryadGreen,
		ShadowDancerBlack,
		BerserkerRed,
		NoxGreen,
		RumRed,
		FireOrange
	}

	public class PigmentsOfTokunoMajor : Item, IUsesRemaining
	{
		public override int LabelNumber { get { return 1070933; } } // Pigments of Tokuno

		private PigmentsType m_Type;

		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public PigmentsType Type { get { return m_Type; } set { m_Type = value; } }

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

		public int GetHue( PigmentsType type )
		{
			int hue = 0;

			switch ( type )
			{
				case PigmentsType.ParagonGold:
					hue = 0x501;
					break;
				case PigmentsType.VioletCouragePurple:
					hue = 0x486;
					break;
				case PigmentsType.InvulnerabilityBlue:
					hue = 0x4F2;
					break;
				case PigmentsType.LunaWhite:
					hue = 0x47E;
					break;
				case PigmentsType.DryadGreen:
					hue = 0x48F;
					break;
				case PigmentsType.ShadowDancerBlack:
					hue = 0x455;
					break;
				case PigmentsType.BerserkerRed:
					hue = 0x21;
					break;
				case PigmentsType.NoxGreen:
					hue = 0x58C;
					break;
				case PigmentsType.RumRed:
					hue = 0x66C;
					break;
				case PigmentsType.FireOrange:
					hue = 0x54F;
					break;
			}

			return hue;
		}

		[Constructable]
		public PigmentsOfTokunoMajor( PigmentsType type ) : base( 0xEFF )
		{
			m_Type = type;

			Hue = GetHue( type );

			m_UsesRemaining = 50;
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendLocalizedMessage( 1070929 ); // Select the artifact or enhanced magic item to dye.

			from.Target = new DyeTarget( this );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1070987 + (int) m_Type );

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public PigmentsOfTokunoMajor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

			writer.Write( (int) m_Type );

			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Type = (PigmentsType) reader.ReadInt();

			m_UsesRemaining = reader.ReadInt();
		}

		private class DyeTarget : Target
		{
			private PigmentsOfTokunoMajor dye;

			public DyeTarget( PigmentsOfTokunoMajor m_dye ) : base( 8, false, TargetFlags.None )
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

				if ( item is StealableArtifact || item is StealableContainerArtifact || item is StealableLightArtifact || item is StealableLongswordArtifact || item is StealablePlateGlovesArtifact || item is StealableWarHammerArtifact || item is StealableExecutionersAxeArtifact || item is StealableFoodArtifact )
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
					item.Hue = dye.GetHue( dye.Type );

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