using Gerakul.ProtoBufSerializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //WriteSample();
            ReadSample();
        }

        public static void WriteSample()
        {
            Room room = new Room()
            {
                Capacity = 10,
                Name = "Restroom"
            };

            room.People.Add(new Person() { Age = 28, Height = 167, Weight = 65.6, Name = "John", Phone = "123-45-67" });
            room.People.Add(new Person() { Age = 44, Height = 170, Weight = 85, Name = "Peter" });
            room.People.Add(new Person() { Age = 33, Height = 160, Weight = 53.2, Name = "Ann", Phone = "111-22-33" });

            var roomDescriptor = Room.CreateDescriptor();

            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                using (var writer = roomDescriptor.CreateWriter(ms))
                {
                    writer.Write(room);
                }

                buff = ms.ToArray();
            }

            var s = Convert.ToBase64String(buff);
        }

        public static void ReadSample()
        {
            byte[] buff = Convert.FromBase64String("CAoSCFJlc3Ryb29tGh8IHBCnARlmZmZmZmZQQCIESm9obioJMTIzLTQ1LTY3GhUILBCqARkAAAAAAEBVQCIFUGV0ZXIaHgghEKABGZqZmZmZmUpAIgNBbm4qCTExMS0yMi0zMw==");

            var roomDescriptor = Room.CreateDescriptor();

            Room room;
            using (var reader = roomDescriptor.CreateReader(new MemoryStream(buff), true))
            {
                room = reader.Read();
            }
        }
    }

    public class Person
    {
        public int Age;
        public int Height;
        public double Weight;
        public string Name;
        public string Phone;

        public static MessageDescriptor<Person> CreateDescriptor()
        {
            FieldSetting<Person>[] settings = new FieldSetting<Person>[]
            {
                FieldSetting<Person>.CreateInt32(1, x => x.Age, (x, y) => x.Age = y),
                FieldSetting<Person>.CreateInt32(2, x => x.Height, (x, y) => x.Height = y),
                FieldSetting<Person>.CreateDouble(3, x => x.Weight, (x, y) => x.Weight = y),
                FieldSetting<Person>.CreateString(4, x => x.Name, (x, y) => x.Name = y),
                FieldSetting<Person>.CreateString(5, x => x.Phone, (x, y) => x.Phone = y, x => x.Phone != null)
            };

            return MessageDescriptor<Person>.Create(settings);

        }
    }

    public class Room
    {
        public int Capacity;
        public string Name;
        public List<Person> People = new List<Person>();

        public static MessageDescriptor<Room> CreateDescriptor()
        {
            FieldSetting<Room>[] settings = new FieldSetting<Room>[]
            {
                FieldSetting<Room>.CreateInt32(1, x => x.Capacity, (x, y) => x.Capacity = y),
                FieldSetting<Room>.CreateString(2, x => x.Name, (x, y) => x.Name = y),
                FieldSetting<Room>.CreateMessageArray(3, x => x.People, (x, y) => x.People.Add(y), Person.CreateDescriptor())
            };

            return MessageDescriptor<Room>.Create(settings);
        }
    }
}
