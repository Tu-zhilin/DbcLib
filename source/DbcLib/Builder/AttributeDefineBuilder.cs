using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Builder
{
    internal class AttributeDefineBuilder : IBuilder
    {
        public void Build(Dbc dbc)
        {
            // 构建默认属性
            var attributeDefine = dbc.CreateAttributeDefine("BusType", ObjectType.NETWORK, Definitions.UserDefinitions.AttributeValueType.STRING);
            attributeDefine.SetDefaultValue("CAN");

            DbcHelper.UpdateAttributeValueDefault(dbc, attributeDefine);
        }
    }
}