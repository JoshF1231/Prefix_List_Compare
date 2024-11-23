using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Collections;

namespace Prefix_List_Compare.Converters
{
    public class DictionaryAccessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AvaloniaDictionary<string, bool> dictionary && parameter is string key)
            {
                return dictionary.ContainsKey(key)&& dictionary[key];
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}