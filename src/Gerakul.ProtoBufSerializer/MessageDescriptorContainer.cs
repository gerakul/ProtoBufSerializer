using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    public class MessageDescriptorContainer
    {
        private Dictionary<Type, Dictionary<string, IUntypedMessageDescriptor>> descriptors = new Dictionary<Type, Dictionary<string, IUntypedMessageDescriptor>>();
        private object lockObject = new object();

        private Dictionary<string, IUntypedMessageDescriptor> GetSubDict(Type argumentType, bool addIfNotExists)
        {
            Dictionary<string, IUntypedMessageDescriptor> subDict = null;
            if (!descriptors.TryGetValue(argumentType, out subDict))
            {
                if (addIfNotExists)
                {
                    subDict = new Dictionary<string, IUntypedMessageDescriptor>();
                    descriptors.Add(argumentType, subDict);
                }
            }

            return subDict;
        }

        private Dictionary<string, IUntypedMessageDescriptor> GetSubDict(IUntypedMessageDescriptor item, bool addIfNotExists)
        {
            return GetSubDict(item.GetArgumentType(), addIfNotExists);
        }

        public void Add(IUntypedMessageDescriptor item, string name = "")
        {
            lock (lockObject)
            {
                var subDict = GetSubDict(item, true);
                subDict.Add(name, item);
            }
        }

        public bool Contains(Type argumentType, string name = "")
        {
            lock (lockObject)
            {
                var subDict = GetSubDict(argumentType, false);
                return subDict?.ContainsKey(name) ?? false;
            }
        }

        public bool Contains<T>(string name = "")
        {
            return Contains(typeof(T), name);
        }

        public bool Remove(Type argumentType, string name = "")
        {
            lock (lockObject)
            {
                var subDict = GetSubDict(argumentType, false);
                return subDict?.Remove(name) ?? false;
            }
        }

        public bool Remove<T>(string name = "")
        {
            return Remove(typeof(T), name);
        }

        public IUntypedMessageDescriptor GetUntyped(Type argumentType, string name = "")
        {
            lock (lockObject)
            {
                var subDict = GetSubDict(argumentType, false);
                if (subDict != null)
                {
                    IUntypedMessageDescriptor d;
                    if (subDict.TryGetValue(name, out d))
                    {
                        return d;
                    }
                }
            }

            return null;
        }

        public IUntypedMessageDescriptor GetUntyped<T>(string name = "")
        {
            return GetUntyped(typeof(T), name);
        }

        public MessageDescriptor<T> Get<T>(string name = "") where T : new()
        {
            return (MessageDescriptor<T>)GetUntyped(typeof(T), name);
        }

        public IUntypedMessageDescriptor[] GetAll()
        {
            lock (lockObject)
            {
                return descriptors.Values.SelectMany(x => x.Values).ToArray();
            }
        }

        public MessageDescriptorEntry[] GetAllEntries()
        {
            lock (lockObject)
            {
                return descriptors.SelectMany(x => x.Value).Select(x => new MessageDescriptorEntry(x.Value, x.Key)).ToArray();
            }
        }
    }

    public class MessageDescriptorEntry
    {
        public IUntypedMessageDescriptor MessageDescriptor { get; private set; }
        public string Name { get; private set; }

        internal MessageDescriptorEntry(IUntypedMessageDescriptor messageDescriptor, string name)
        {
            this.MessageDescriptor = messageDescriptor;
            this.Name = name;
        }
    }
}
