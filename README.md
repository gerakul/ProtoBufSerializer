# ProtoBufSerializer
This library is using concept called MessageDescriptor instead of proto file. MessageDescriptor let you describe object for serialization more flexibly than proto file. It let you use specific features of C#, for example values of type ```Nullable<T>``` which may not be put in buffer if they are NULL. 

## Sample

Declaration of classes:

```csharp
public class Person
{
    public int Age;
    public int Height;
    public double Weight;
    public string Name;
    public string Phone;
}

public class Room
{
    public int Capacity;
    public string Name;
    public List<Person> People = new List<Person>();
}
``` 

Creating message descriptors:

```csharp
// Person descriptor
var personDescr = MessageDescriptorBuilder.New<Person>()
    .Int32(1, x => x.Age, (x, y) => x.Age = y)
    .Int32(2, x => x.Height, (x, y) => x.Height = y)
    .Double(3, x => x.Weight, (x, y) => x.Weight = y)
    .String(4, x => x.Name, (x, y) => x.Name = y)
    .String(5, x => x.Phone, (x, y) => x.Phone = y, x => x.Phone != null)
    .CreateDescriptor();

// Room descriptor
var roomDescr = MessageDescriptorBuilder.New<Room>()
    .Int32(1, x => x.Capacity, (x, y) => x.Capacity = y)
    .String(2, x => x.Name, (x, y) => x.Name = y)
    .MessageArray(3, x => x.People, (x, y) => x.People.Add(y), Person.CreateDescriptor())
    .CreateDescriptor();
```

Serialization and deserialization:

```csharp
Room room = new Room()
{
    Capacity = 10,
    Name = "Restroom"
};

room.People.Add(new Person() { Age = 28, Height = 167, Weight = 65.6, Name = "John", Phone = "123-45-67" });
room.People.Add(new Person() { Age = 44, Height = 170, Weight = 85, Name = "Peter" });
room.People.Add(new Person() { Age = 33, Height = 160, Weight = 53.2, Name = "Ann", Phone = "111-22-33" });

// serialization
byte[] buff = roomDescr.Write(room);

// deserialization
Room r = roomDescr.Read(buff);
```
