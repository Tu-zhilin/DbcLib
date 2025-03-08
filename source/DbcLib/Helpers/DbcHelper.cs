using DbcLib.Definitions.MessageDefinitions;
using DbcLib.Definitions.UserDefinitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DbcLib.Helper
{
    public class DbcHelper
    {
        /// <summary>
        /// 用于解析DBC文本格式的报文ID
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static UInt32 GetMessageIDFromFile(string content)
        {
            UInt32 id = UInt32.Parse(content);
            if (id == 0xC0000000)
            {
                return id;
            }
            return (id <= 0x7FF) ? id : (id & 0x1FFFFFFF);
        }
        /// <summary>
        /// 用于还原DBC文本格式的报文ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static UInt32 GetMessageIDForFile(UInt32 id)
        {
            if (id == 0xC0000000)
            {
                return id;
            }
            return id <= 0x7FF || id >= 0x1FFFFFFF ? id : id |= 0x80000000;
        }
        /// <summary>
        /// 起始位转换
        /// Motolora 转 Intel
        /// </summary>
        /// <param name="startBit"></param>
        /// <param name="size"></param>
        /// <returns>Intel格式的StartBit</returns>
        public static int MotoloraToIntel(int startBit, int size)
        {
            int x = 8 - (startBit % 8) - 1;
            int startByte = startBit / 8;
            if ((x + 1) >= size)
            {
                return (startByte + 1) * 8 - (startBit % 8 + size);
            }
            int headBit = 8 - (startBit % 8);
            int remainBit = size - headBit;
            int remainByte = remainBit / 8 + ((remainBit % 8) == 0 ? 0 : 1);
            return (startByte - remainByte) * 8 + (7 - (remainBit - 1) % 8);
        }
        /// <summary>
        /// 起始位转换
        /// Intel转Motolora
        /// </summary>
        /// <param name="startBit"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int IntelToMotolora(int startBit, int size)
        {
            // TODO: intel to motolora
            return 0;
        }
        /// <summary>
        /// Motolora（DBC文本格式）转 Motolora（DBC显示格式）
        /// 针对motolora格式，DBC中保存的startBit与我们实际看到的不一致，所以我们这里需要进行转换
        /// </summary>
        /// <param name="startBit"></param>
        /// <param name="length"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        internal static int GetSignalStartBit(int startBit, int length, ByteOrder order)
        {
            if (order == ByteOrder.LSB)
            {
                return startBit;
            }
            int x = startBit % 8 + 1;
            if (x >= length)
            {
                return startBit - (length - 1);
            }
            int remainLength = length - x;
            int newStartByte = (startBit / 8) + (remainLength % 8 == 0 ? remainLength / 8 : remainLength / 8 + 1);
            return newStartByte * 8 + 7 - (remainLength - 1) % 8;
        }
        /// <summary>
        /// 根据属性名获取Dbc对象的属性
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetAttributeValue(DbcObject instance, string name)
        {
            IEnumerable<AttributeValue> _attributes = instance.AttributeValues.Where(o => o.Name == name);
            if (_attributes.Count() == 0)
            {
#if DEBUG
                throw new Exception($"{instance.Name}'s do not contains {name} AttributeValue");
#else
                return null;
#endif
            }
            return _attributes.First()?.Value;
        }
        internal static string ObjectTypeMapping(ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.NETWORK:
                    return $"";
                case ObjectType.MESSAGE:
                    return $"BO_";
                case ObjectType.NODE:
                    return $"BU_";
                case ObjectType.SIGNAL:
                    return $"SG_";
                case ObjectType.ENVIRONMENT:
#if DEBUG
                    throw new Exception($"unrecognize type {objectType}\r\n");
#else
                    objectType = ObjectType.ENVIRONMENT;
                    return;
#endif
                default:
#if DEBUG
                    throw new Exception($"unrecognize type {objectType}\r\n");
#else
                    return;
#endif
            }
        }
        internal static ObjectType ObjectTypeMapping(string content)
        {
            switch (content)
            {
                case "":
                    return ObjectType.NETWORK;
                case "BO_":
                    return ObjectType.MESSAGE;
                case "BU_":
                    return ObjectType.NODE;
                case "SG_":
                    return ObjectType.SIGNAL;
                default:
                    return ObjectType.NETWORK;
            }
        }
        public static void SetAttributeValue(DbcObject instance, string attributeName, object value)
        {
            instance.AttributeValues.
                Where(o => o.Name == attributeName).
                Select(o =>
                {
                    o.SetValue(value);
                    return true;
                }).ToList();
        }
        internal static void UpdateAttributeValueDefault(Dbc dbc, AttributeDefine attributeDefine)
        {
            switch (attributeDefine.ObjectType)
            {
                case ObjectType.NETWORK:
                    SetAttributeValue(dbc, attributeDefine.Name, attributeDefine.DefaultValue);
                    break;
                case ObjectType.NODE:
                    dbc.Nodes.Select(o =>
                    {
                        SetAttributeValue(o, attributeDefine.Name, attributeDefine.DefaultValue);
                        return false;
                    }).ToList();
                    break;
                case ObjectType.MESSAGE:
                    dbc.Messages.Select(o =>
                    {
                        SetAttributeValue(o, attributeDefine.Name, attributeDefine.DefaultValue);
                        return false;
                    }).ToList();
                    break;
                case ObjectType.SIGNAL:
                    dbc.Signals.Select(o =>
                    {
                        SetAttributeValue(o, attributeDefine.Name, attributeDefine.DefaultValue);
                        return false;
                    }).ToList();
                    break;
#if DEBUG
                    throw new Exception($"unrecognized object type {attributeDefine.ObjectType}");
#else
                        return;
#endif
            }
        }
    }
}