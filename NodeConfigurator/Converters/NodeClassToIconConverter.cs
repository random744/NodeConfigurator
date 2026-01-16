using System;
using System.Globalization;
using System.Windows.Data;
using Opc.Ua;

namespace NodeConfigurator.Converters
{
    public class NodeClassToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeClass nodeClass)
            {
                return nodeClass switch
                {
                    NodeClass.Object => "📁",
                    NodeClass.Variable => "📊",
                    NodeClass.Method => "⚙️",
                    NodeClass.ObjectType => "📂",
                    NodeClass.VariableType => "📈",
                    NodeClass.ReferenceType => "🔗",
                    NodeClass.DataType => "🔢",
                    NodeClass.View => "👁️",
                    _ => "❓"
                };
            }
            return "❓";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
