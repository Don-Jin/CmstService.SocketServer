using System;
using System.ComponentModel;

namespace CmstService.SocketServer.ConfigurationHelper
{
    // 通用字符串类型转换器
    public class PropertyGridStringConverter : StringConverter
    {
        public PropertyGridStringConverter()
            : base()
        {
        
        }

        public PropertyGridStringConverter(string[] array)
        {
            this.StandardValue = array;
        }

        private string[] StandardValue;

        // 覆盖 GetStandardValuesSupported 方法并返回 true ，表示此对象支持可以从列表中选取的一组标准值。 
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        // 覆盖 GetStandardValues 方法并返回填充了标准值的 StandardValuesCollection。
        // 创建 StandardValuesCollection 的方法之一是在构造函数中提供一个值数组。
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this.StandardValue == null)
            {
                return base.GetStandardValues(context);
            }
            return new StandardValuesCollection(this.StandardValue);
        }

        // （可选）如果希望用户能够键入下拉列表中没有包含的值，请覆盖 GetStandardValuesExclusive 方法并返回 false。
        // 这从根本上将下拉列表样式变成了组合框样式。
        //public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        //{
        //    return false;
        //}
    }
}
