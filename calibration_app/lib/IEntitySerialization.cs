using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace calibration_app.lib
{
    /// <summary>  
    /// 定义元属性实体类与Xml之间的转换接口  
    /// </summary>  
    /// <typeparam name="T">元属性实体类</typeparam>  
    public interface IEntitySerialization<T>
    {
        /// <summary>  
        /// 将实体类<c>T</c>使用<see cref="XmlWriter"/>转为XML 数据写到流、文件、文本  
        /// 读取器或字符串。  
        /// </summary>  
        /// <param name="writer"><see cref="XmlWriter"/>实例</param>  
        void WriteXml(XmlWriter writer);

        /// <summary>  
        /// 将<see cref="XmlNode"/>节点的信息转为实体类<c>T</c>  
        /// </summary>  
        /// <param name="node"><see cref="XmlNode"/>实例，该实例包含了<c>T</c>  
        /// 实体类所需要的信息</param>  
        /// <returns>实体类<c>T</c></returns>  
        T GetObject(XmlNode node);

        /// <summary>  
        /// 根节点元素标签名称  
        /// </summary>  
        string RootElementName { get; }
    }
}
