using System;
using Xamarin.Forms;
using System.Globalization;

namespace MeasureApp.View.Device
{
	public class GuidConverter : IValueConverter
	{
		public object Convert (
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
		{
			Guid guid = (Guid)value;
			return guid.ToString ();
		}

		public object ConvertBack (
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
		{
			throw new NotImplementedException ("GuidConverter is one-way");
		}
	}
}

