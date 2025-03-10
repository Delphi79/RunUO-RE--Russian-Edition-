using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Engines.Craft;

namespace Server.Engines.BulkOrders
{
	public class SmallTailorBOD : SmallBOD
	{
		public static double[] m_TailoringMaterialChances = new double[] {0.857421875, // None
			0.125000000, // Spined
			0.015625000, // Horned
			0.001953125 // Barbed
		};

		public override int ComputeFame()
		{
			return TailorRewardCalculator.Instance.ComputeFame( this );
		}

		public override int ComputeGold()
		{
			return TailorRewardCalculator.Instance.ComputeGold( this );
		}

		public override ArrayList ComputeRewards( bool full )
		{
			ArrayList list = new ArrayList();

			RewardGroup rewardGroup = TailorRewardCalculator.Instance.LookupRewards( TailorRewardCalculator.Instance.ComputePoints( this ) );

			if ( rewardGroup != null )
			{
				if ( full )
				{
					for ( int i = 0; i < rewardGroup.Items.Length; ++i )
					{
						Item item = rewardGroup.Items[ i ].Construct();

						if ( item != null )
						{
							list.Add( item );
						}
					}
				}
				else
				{
					RewardItem rewardItem = rewardGroup.AquireItem();

					if ( rewardItem != null )
					{
						Item item = rewardItem.Construct();

						if ( item != null )
						{
							list.Add( item );
						}
					}
				}
			}

			return list;
		}

		public static SmallTailorBOD CreateRandomFor( Mobile m )
		{
			SmallBulkEntry[] entries;
			bool useMaterials;

			if ( useMaterials = Utility.RandomBool() )
			{
				entries = SmallBulkEntry.TailorLeather;
			}
			else
			{
				entries = SmallBulkEntry.TailorCloth;
			}

			if ( entries.Length > 0 )
			{
				double theirSkill = m.Skills[ SkillName.Tailoring ].Base;
				int amountMax;

				if ( theirSkill >= 70.1 )
				{
					amountMax = Utility.RandomList( 10, 15, 20, 20 );
				}
				else if ( theirSkill >= 50.1 )
				{
					amountMax = Utility.RandomList( 10, 15, 15, 20 );
				}
				else
				{
					amountMax = Utility.RandomList( 10, 10, 15, 20 );
				}

				BulkMaterialType material = BulkMaterialType.None;

				if ( useMaterials && theirSkill >= 70.1 )
				{
					for ( int i = 0; i < 20; ++i )
					{
						BulkMaterialType check = GetRandomMaterial( BulkMaterialType.Spined, m_TailoringMaterialChances );
						double skillReq = 0.0;

						switch ( check )
						{
							case BulkMaterialType.DullCopper:
								skillReq = 65.0;
								break;
							case BulkMaterialType.Bronze:
								skillReq = 80.0;
								break;
							case BulkMaterialType.Gold:
								skillReq = 85.0;
								break;
							case BulkMaterialType.Agapite:
								skillReq = 90.0;
								break;
							case BulkMaterialType.Verite:
								skillReq = 95.0;
								break;
							case BulkMaterialType.Valorite:
								skillReq = 100.0;
								break;
							case BulkMaterialType.Spined:
								skillReq = 65.0;
								break;
							case BulkMaterialType.Horned:
								skillReq = 80.0;
								break;
							case BulkMaterialType.Barbed:
								skillReq = 99.0;
								break;
						}

						if ( theirSkill >= skillReq )
						{
							material = check;
							break;
						}
					}
				}

				double excChance = 0.0;

				if ( theirSkill >= 70.1 )
				{
					excChance = (theirSkill + 80.0)/200.0;
				}

				bool reqExceptional = (excChance > Utility.RandomDouble());

				SmallBulkEntry entry = null;

				CraftSystem system = DefTailoring.CraftSystem;

				for ( int i = 0; i < 150; ++i )
				{
					SmallBulkEntry check = entries[ Utility.Random( entries.Length ) ];

					CraftItem item = system.CraftItems.SearchFor( check.Type );

					if ( item != null )
					{
						bool allRequiredSkills = true;
						double chance = item.GetSuccessChance( m, null, system, false, ref allRequiredSkills );

						if ( allRequiredSkills && chance >= 0.0 )
						{
							if ( reqExceptional )
							{
								chance = item.GetExceptionalChance( system, chance, m );
							}

							if ( chance > 0.0 )
							{
								entry = check;
								break;
							}
						}
					}
				}

				if ( entry != null )
				{
					return new SmallTailorBOD( entry, material, amountMax, reqExceptional );
				}
			}

			return null;
		}

		private SmallTailorBOD( SmallBulkEntry entry, BulkMaterialType material, int amountMax, bool reqExceptional )
		{
			this.Hue = 0x483;
			this.AmountMax = amountMax;
			this.Type = entry.Type;
			this.Number = entry.Number;
			this.Graphic = entry.Graphic;
			this.RequireExceptional = reqExceptional;
			this.Material = material;
		}

		[Constructable]
		public SmallTailorBOD()
		{
			SmallBulkEntry[] entries;
			bool useMaterials;

			if ( useMaterials = Utility.RandomBool() )
			{
				entries = SmallBulkEntry.TailorLeather;
			}
			else
			{
				entries = SmallBulkEntry.TailorCloth;
			}

			if ( entries.Length > 0 )
			{
				int hue = 0x483;
				int amountMax = Utility.RandomList( 10, 15, 20 );

				BulkMaterialType material;

				if ( useMaterials )
				{
					material = GetRandomMaterial( BulkMaterialType.Spined, m_TailoringMaterialChances );
				}
				else
				{
					material = BulkMaterialType.None;
				}

				bool reqExceptional = Utility.RandomBool() || (material == BulkMaterialType.None);

				SmallBulkEntry entry = entries[ Utility.Random( entries.Length ) ];

				this.Hue = hue;
				this.AmountMax = amountMax;
				this.Type = entry.Type;
				this.Number = entry.Number;
				this.Graphic = entry.Graphic;
				this.RequireExceptional = reqExceptional;
				this.Material = material;
			}
		}

		public SmallTailorBOD( int amountCur, int amountMax, Type type, int number, int graphic, bool reqExceptional, BulkMaterialType mat )
		{
			this.Hue = 0x483;
			this.AmountMax = amountMax;
			this.AmountCur = amountCur;
			this.Type = type;
			this.Number = number;
			this.Graphic = graphic;
			this.RequireExceptional = reqExceptional;
			this.Material = mat;
		}

		public SmallTailorBOD( Serial serial ) : base( serial )
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