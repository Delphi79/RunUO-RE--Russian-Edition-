using System;
using System.Collections;

namespace Server.Engines.MyRunUO
{
	public class LayerComparer : IComparer
	{
		private static Layer PlateArms = (Layer) 255;
		private static Layer ChainTunic = (Layer) 254;
		private static Layer LeatherShorts = (Layer) 253;

		private static Layer[] m_DesiredLayerOrder = new Layer[]
		{
			Layer.Cloak,
			Layer.Bracelet,
			Layer.Ring,
			Layer.Shirt,
			Layer.Pants,
			Layer.InnerLegs,
			Layer.Shoes,
			LeatherShorts,
			Layer.Arms,
			Layer.InnerTorso,
			LeatherShorts,
			PlateArms,
			Layer.MiddleTorso,
			Layer.OuterLegs,
			Layer.Neck,
			Layer.Waist,
			Layer.Gloves,
			Layer.OuterTorso,
			Layer.OneHanded,
			Layer.TwoHanded,
			Layer.FacialHair,
			Layer.Hair,
			Layer.Helm
		};

		private static int[] m_TranslationTable;

		public static int[] TranslationTable { get { return m_TranslationTable; } }

		static LayerComparer()
		{
			m_TranslationTable = new int[256];

			for ( int i = 0; i < m_DesiredLayerOrder.Length; ++i )
			{
				m_TranslationTable[ (int) m_DesiredLayerOrder[ i ] ] = m_DesiredLayerOrder.Length - i;
			}
		}

		public static bool IsValid( Item item )
		{
			return (m_TranslationTable[ (int) item.Layer ] > 0);
		}

		public static readonly IComparer Instance = new LayerComparer();

		public LayerComparer()
		{
		}

		public Layer Fix( int itemID, Layer oldLayer )
		{
			if ( itemID == 0x1410 || itemID == 0x1417 ) // platemail arms
			{
				return PlateArms;
			}

			if ( itemID == 0x13BF || itemID == 0x13C4 ) // chainmail tunic
			{
				return ChainTunic;
			}

			if ( itemID == 0x1C08 || itemID == 0x1C09 ) // leather skirt
			{
				return LeatherShorts;
			}

			if ( itemID == 0x1C00 || itemID == 0x1C01 ) // leather shorts
			{
				return LeatherShorts;
			}

			return oldLayer;
		}

		public int Compare( object x, object y )
		{
			Item a = (Item) x;
			Item b = (Item) y;

			Layer aLayer = a.Layer;
			Layer bLayer = b.Layer;

			aLayer = Fix( a.ItemID, aLayer );
			bLayer = Fix( b.ItemID, bLayer );

			return m_TranslationTable[ (int) bLayer ] - m_TranslationTable[ (int) aLayer ];
		}
	}
}