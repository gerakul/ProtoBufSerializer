using Gerakul.ProtoBufSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WriteSample();
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

            byte[] buff = roomDescriptor.Write(room);
        }

        public static void ReadSample()
        {
            byte[] buff = Convert.FromBase64String("CAoSCFJlc3Ryb29tGh8IHBCnARlmZmZmZmZQQCIESm9obioJMTIzLTQ1LTY3GhUILBCqARkAAAAAAEBVQCIFUGV0ZXIaHgghEKABGZqZmZmZmUpAIgNBbm4qCTExMS0yMi0zMw==");

            var roomDescriptor = Room.CreateDescriptor();

            Room room = roomDescriptor.Read(buff);
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
            return MessageDescriptorBuilder.New<Person>()
                .Int32(1, x => x.Age, (x, y) => x.Age = y)
                .Int32(2, x => x.Height, (x, y) => x.Height = y)
                .Double(3, x => x.Weight, (x, y) => x.Weight = y)
                .String(4, x => x.Name, (x, y) => x.Name = y)
                .String(5, x => x.Phone, (x, y) => x.Phone = y, x => x.Phone != null)
                .CreateDescriptor();
        }
    }

    public class Room
    {
        public int Capacity;
        public string Name;
        public List<Person> People = new List<Person>();

        public static MessageDescriptor<Room> CreateDescriptor()
        {
            return MessageDescriptorBuilder.New<Room>()
                .Int32(1, x => x.Capacity, (x, y) => x.Capacity = y)
                .String(2, x => x.Name, (x, y) => x.Name = y)
                .MessageArray(3, x => x.People, (x, y) => x.People.Add(y), Person.CreateDescriptor())
                .CreateDescriptor();
        }
    }

}
