using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ExceptionHandlingInjector.Infrastructure
{
    public class ApplyToConverter:ConverterExtension
    {
        public ApplyToConverter()
        {
            
        }

        private const string applyToText = "Apply injectors to";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format("{0} {1}", applyToText, value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().Replace(applyToText, "");
        }
    }
}
