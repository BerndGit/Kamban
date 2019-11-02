﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kamban.Views.WpfResources
{
    public abstract class BaseConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public abstract object Convert(object value, Type targetType, object parameter,
                                       CultureInfo culture);


        public virtual object ConvertBack(object value, Type targetType, object parameter,
                                          CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MoreThenOneToVisibility : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter,
                                       CultureInfo culture)
        {
            return (int?) value > 1 ? Visibility.Visible : Visibility.Hidden;
        } 
    } 

    public class BoolInverse : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter,
                                       CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    public class ThicknessToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness v)
                return $"{v.Left},{v.Top},{v.Right},{v.Bottom}";

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Thickness)new ThicknessConverter()
                .ConvertFrom(null, CultureInfo.CurrentCulture, value);
        }
    }

    public class DateTime_to_DateString : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ((DateTime)value).ToString("d");
            }
            catch
            {
                return "";
            }
            
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTime_to_TimeString : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ((DateTime)value).ToString("HH:mm:ss.f");
            }
            catch
            {
                return "";
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
