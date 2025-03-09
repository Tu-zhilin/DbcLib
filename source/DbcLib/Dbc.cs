using DbcLib.Definitions.MessageDefinitions;
using DbcLib.Definitions.NodeDefinitions;
using DbcLib.Definitions.UserDefinitions;
using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace DbcLib
{
    public class Dbc : DbcObject
    {
        public override string Name => (string)AttributeValues.Where(o => o.Name == "DBName").First().Value;
        public string Version
        {
            get; set;
        }
        public List<string> Symbol { get; set; }
        public List<string> BaudRates { get; set; }
        private List<Node> nodes;
        public IReadOnlyList<Node> Nodes => new ReadOnlyCollection<Node>(nodes);
        private List<Message> messages;
        public IReadOnlyList<Message> Messages => new ReadOnlyCollection<Message>(messages.Where(o => o.Name != "VECTOR__INDEPENDENT_SIG_MSG").ToList());
        private List<Signal> signals;
        public IReadOnlyList<Signal> Signals => new ReadOnlyCollection<Signal>(signals);
        private List<AttributeDefine> attributeDefines;
        public IReadOnlyList<AttributeDefine> AttributeDefines => new ReadOnlyCollection<AttributeDefine>(attributeDefines);
        public Dbc()
        {
            Symbol = new List<string>();
            BaudRates = new List<string>();
            nodes = new List<Node>();
            messages = new List<Message>();
            signals = new List<Signal>();
            attributeDefines = new List<AttributeDefine>();
        }
        public Node CreateNode(string name)
        {
            if (nodes.Where(o => o.Name == name).Count() != 0)
            {
#if DEBUG
                throw new Exception($"this dbc contains {name} Node");
#else
                return null;
#endif
            }

            Node node = new Node(this, name);

            nodes.Add(node);

            return node;
        }
        public Message CreateMessage(string name)
        {
            if (messages.Where(o => o.Name == name).Count() != 0)
            {
#if DEBUG
                throw new Exception($"this dbc contains {name} Message");
#else
                return null;
#endif
            }
            Message message = new Message(this, name);

            AttributeDefines.
                Where(o => o.ObjectType == ObjectType.MESSAGE).
                Select(o =>
                {
                    message.AddAttributeValue(o.CreateAttributeValue());
                    return 0;
                }).ToList();
            messages.Add(message);

            return message;
        }
        public Signal CreateSignal(string name)
        {
            //            if (signals.Where(o => o.Name == name).Count() != 0)
            //            {
            //#if DEBUG
            //                throw new Exception($"this dbc contains {name} Signal");
            //#else
            //                return null;
            //#endif
            //            }

            Signal signal = new Signal(name);

            AttributeDefines.
                Where(o => o.ObjectType == ObjectType.SIGNAL).
                Select(o =>
                {
                    signal.AddAttributeValue(o.CreateAttributeValue());
                    return 0;
                }).ToList();

            signals.Add(signal);

            //  TODO: 开始要归属到默认Message下

            return signal;
        }
        public AttributeDefine CreateAttributeDefine(string name, ObjectType objectType, AttributeValueType valueType)
        {
            if (attributeDefines.Where(o => o.Name == name).Count() != 0)
            {
#if DEBUG
                throw new Exception($"this dbc contains {name} AttributeDefine");
#else
                return null;
#endif
            }

            var attributeDefine = AttributeDefine.CreateAttributeDefine(this, name, objectType, valueType);

            switch (objectType)
            {
                case ObjectType.NETWORK:
                    this.AddAttributeValue(attributeDefine.CreateAttributeValue());
                    break;
                case ObjectType.NODE:
                    foreach (var node in nodes)
                    {
                        node.AddAttributeValue(attributeDefine.CreateAttributeValue());
                    }
                    break;
                case ObjectType.MESSAGE:
                    foreach (var message in messages)
                    {
                        message.AddAttributeValue(attributeDefine.CreateAttributeValue());
                    }
                    break;
                case ObjectType.SIGNAL:
                    foreach (var signal in signals)
                    {
                        signal.AddAttributeValue(attributeDefine.CreateAttributeValue());
                    }
                    break;
                case ObjectType.ENVIRONMENT:
#if DEBUG
                    throw new Exception($"object type ({objectType}) not supported yet");
#else
                    return null;
#endif
                default:
#if DEBUG
                    throw new Exception($"unexpected Object Type ({objectType}) have occurred");
#else
                    return null;
#endif
            }

            this.attributeDefines.Add(attributeDefine);

            return attributeDefine;
        }
        public bool DeleteNode(Node node)
        {
            if (!nodes.Contains(node))
            {
#if DEBUG
                throw new Exception($"this dbc contains {node.Name} Node");
#else
                return false;
#endif
            }
            foreach (var message in messages)
            {
                if (message.Transmitters.Contains(node.Name))
                {
                    message.RemoveTransmitter(node.Name);
                }
            }
            foreach (var signal in signals)
            {
                if (signal.Receivers.Contains(node.Name))
                {
                    signal.RemoveReceiver(node.Name);
                }
            }

            nodes.Remove(node);

            return true;
        }
        public bool DeleteMessage(Message message)
        {
            if (!messages.Contains(message))
            {
#if DEBUG
                throw new Exception($"this dbc contains {message.Name} Message");
#else
                return false;
#endif
            }

            messages.Remove(message);

            return true;
        }

        public bool DeleteSignal(Signal signal)
        {
            if (!signals.Contains(signal))
            {
#if DEBUG
                throw new Exception($"this dbc do not contains {signal.Name} Signal");
#else
                return false;
#endif
            }

            foreach (var message in messages)
            {
                if (message.Signals.Contains(signal))
                {
                    message.RemoveSignal(signal);
                }
            }

            signals.Remove(signal);

            return true;
        }
        public bool DeleteAttributeDefine(AttributeDefine attributeDefine)
        {
            if (!attributeDefines.Contains(attributeDefine))
            {
#if DEBUG
                throw new Exception($"this dbc do not contains {attributeDefine.Name} AttributeDefine");
#else
                return false;
#endif
            }
            switch (attributeDefine.ObjectType)
            {
                case ObjectType.NETWORK:
                    this.RemoveAttributeValue(attributeDefine.Name);
                    break;
                case ObjectType.NODE:
                    foreach (var node in nodes)
                    {
                        node.RemoveAttributeValue(attributeDefine.Name);
                    }
                    break;
                case ObjectType.MESSAGE:
                    foreach (var message in messages)
                    {
                        message.RemoveAttributeValue(attributeDefine.Name);
                    }
                    break;
                case ObjectType.SIGNAL:
                    foreach (var signal in signals)
                    {
                        signal.RemoveAttributeValue(attributeDefine.Name);
                    }
                    break;
                case ObjectType.ENVIRONMENT:
#if DEBUG
                    throw new Exception($"object type ({attributeDefine.ObjectType}) not supported yet");
#else
                    return false;
#endif
                default:
#if DEBUG
                    throw new Exception($"unexpected Object Type ({attributeDefine.ObjectType}) have occurred");
#else
                    return false;
#endif
            }

            attributeDefines.Remove(attributeDefine);

            return true;
        }
        public override void Rename(string name)
        {
            DbcHelper.SetAttributeValue(this, "DBName", name);
        }
    }
}