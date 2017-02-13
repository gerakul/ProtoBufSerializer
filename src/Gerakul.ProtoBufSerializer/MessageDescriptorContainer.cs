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

        public bool TryAdd(IUntypedMessageDescriptor item, string name = "")
        {
            lock (lockObject)
            {
                var subDict = GetSubDict(item, true);
                if (!subDict.ContainsKey(name))
                {
                    subDict.Add(name, item);
                    return true;
                }

                return false;
            }
        }

        public void AddRange(IEnumerable<MessageDescriptorEntry> entries)
        {
            lock (lockObject)
            {
                foreach (var item in entries)
                {
                    var subDict = GetSubDict(item.MessageDescriptor, true);
                    subDict.Add(item.Name, item.MessageDescriptor);
                }
            }
        }

        public int TryAddRange(IEnumerable<MessageDescriptorEntry> entries)
        {
            int num = 0;
            lock (lockObject)
            {
                foreach (var item in entries)
                {
                    var subDict = GetSubDict(item.MessageDescriptor, true);
                    if (!subDict.ContainsKey(item.Name))
                    {
                        subDict.Add(item.Name, item.MessageDescriptor);
                        num++;
                    }
                }
            }

            return num;
        }

        public void AddRange(IEnumerable<IUntypedMessageDescriptor> descriptors)
        {
            lock (lockObject)
            {
                foreach (var item in descriptors)
                {
                    var subDict = GetSubDict(item, true);
                    subDict.Add("", item);
                }
            }
        }

        public int TryAddRange(IEnumerable<IUntypedMessageDescriptor> descriptors)
        {
            int num = 0;
            lock (lockObject)
            {
                foreach (var item in descriptors)
                {
                    var subDict = GetSubDict(item, true);
                    if (!subDict.ContainsKey(""))
                    {
                        subDict.Add("", item);
                        num++;
                    }
                }
            }

            return num;
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

        public MessageDescriptorEntry(IUntypedMessageDescriptor messageDescriptor, string name)
        {
            this.MessageDescriptor = messageDescriptor;
            this.Name = name;
        }
    }
}
