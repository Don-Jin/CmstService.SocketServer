using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;

namespace CmstService.SocketServer.ConfigurationHelper
{
    public class PropertyGridFileEditor : UITypeEditor
    {
        // 自定义属性编辑样式
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        // 值编辑
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editor = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            
            if (editor != null)
            {
                // 打开对话框
                OpenFileDialog open = new OpenFileDialog();

                // 隐藏扩展名
                open.AddExtension = false;
                
                // 文件名筛选
                open.Filter = "动态链接库|*.dll";

                // 文件选中
                if (open.ShowDialog().Equals(DialogResult.OK))
                {
                    try
                    {
                        Type type = null;
                        
                        // 1.根据 InterfaceAssemblyAttribute 特性反射接口，如为 null 执行2
                        InterfaceAssemblyAttribute attr = context.PropertyDescriptor.Attributes[typeof(InterfaceAssemblyAttribute)] as InterfaceAssemblyAttribute;
                        if (attr != null)
                        {
                            type = attr.Assembly;
                        }
                        
                        // 2.通过泛型类型获取实例实现的接口类型，如为 null 执行3
                        if (type == null)
                        {
                            PropertyInfo prop = context.Instance.GetType().GetProperty("Assembly");
                            if (prop != null)
                            {
                                type = prop.GetValue(context.Instance, null) as Type;
                            }
                        }
                        
                        // 3.未获取实例，抛出异常
                        if (type == null)
                        {
                            throw new NullReferenceException();
                        }

                        // 获取文件名，通过反射获取匹配上述接口的类
                        foreach (Type tp in Assembly.LoadFrom(open.FileName).GetExportedTypes())
                        {
                            if (tp.GetInterface(type.ToString()) != null)
                            {
                                return tp.ToString() + ", " + tp.Namespace;
                            }
                        }
                    }
                    catch { }
                }
            }
            return value;
        }
    }
}
