using System;
using System.Drawing;
using System.Collections;

namespace Server.Engines.Reports
{
	//*********************************************************************
	//
	// Chart Class
	//
	// Base class implementation for BarChart and PieChart
	//
	//*********************************************************************

	public abstract class ChartRenderer
	{
		private const int _colorLimit = 9;

		private Color[] _color = {Color.Firebrick, Color.SkyBlue, Color.MediumSeaGreen, Color.MediumOrchid, Color.Chocolate, Color.SlateBlue, Color.LightPink, Color.LightGreen, Color.Khaki};

		// Represent collection of all data points for the chart
		private ChartItemsCollection _dataPoints = new ChartItemsCollection();

		// The implementation of this method is provided by derived classes
		public abstract Bitmap Draw();

		public ChartItemsCollection DataPoints { get { return _dataPoints; } set { _dataPoints = value; } }

		public void SetColor( int index, Color NewColor )
		{
			if ( index < _colorLimit )
			{
				_color[ index ] = NewColor;
			}
			else
			{
				throw new Exception( "Color Limit is " + _colorLimit );
			}
		}

		public Color GetColor( int index )
		{
			//return _color[index%_colorLimit];

			if ( index < _colorLimit )
			{
				return _color[ index ];
			}
			else
			{
				return _color[ (index + 2)%_colorLimit ];
				//throw new Exception("Color Limit is " + _colorLimit);
			}
		}
	}
}