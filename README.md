# ProtoBufSerializer
This library is using concept called MessageDescriptor instead of proto file. MessageDescriptor let you describe object for serialization more flexibly than proto file. It let you use specific features of C#, for example values of type ```Nullable<T>``` which may not be put in buffer if they are NULL. 

##Sample

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
FieldSetting<Person>[] personSettings = new FieldSetting<Person>[]
{
    FieldSetting<Person>.CreateInt32(1, x => x.Age, (x, y) => x.Age = y),
    FieldSetting<Person>.CreateInt32(2, x => x.Height, (x, y) => x.Height = y),
    FieldSetting<Person>.CreateDouble(3, x => x.Weight, (x, y) => x.Weight = y),
    FieldSetting<Person>.CreateString(4, x => x.Name, (x, y) => x.Name = y),
    FieldSetting<Person>.CreateString(5, x => x.Phone, (x, y) => x.Phone = y, x => x.Phone != null)
};

var personDescr = MessageDescriptor<Person>.Create(personSettings);

// Room descriptor
FieldSetting<Room>[] roomSettings = new FieldSetting<Room>[]
{
    FieldSetting<Room>.CreateInt32(1, x => x.Capacity, (x, y) => x.Capacity = y),
    FieldSetting<Room>.CreateString(2, x => x.Name, (x, y) => x.Name = y),
    FieldSetting<Room>.CreateMessageArray(3, x => x.People, (x, y) => x.People.Add(y), personDescr)
};

var roomDescr = MessageDescriptor<Room>.Create(roomSettings);
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
