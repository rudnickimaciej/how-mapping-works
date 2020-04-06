# Jak działa maper?

W jaki sposób rekord zapisany w bazie danych np. w tabeli Person przekształcić w obiekt C# klasy Person?

Dane z tabeli Person → new Pracownik().


Najprostszym sposobem  jest bezpośrednie wysłanie zapytania do bazy danych np. takiego:

Select * from Pracownicy where id=2;


Posłużymy się abstrakcją z przestrzenii System.Data.SqlClient.

```csharp
            using (SqlConnection con = new SqlConnection(_connString))
            {     
               con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                      "SELECT * FROM  Person where id='"+id +"'", con))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                return new Person
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Age = reader.GetInt32(2
                                    )
                                };
                            }
                            return null;
                        }
                        else
                        {
                            throw new Exception("Error: There is no such record in table.");
                        }

                    }
                }
                catch(Exception e)
                {
                    throw new Exception("Error: " + e.Message);
                }     
             }
```
![1](https://user-images.githubusercontent.com/42208564/78453794-6c27b380-7694-11ea-92a0-f2f546dd5b23.png)


Takie podejście wymaga od nas posiadania informacji o nazwach pól klasy Person i ich typach.

Nie chcemy używać nazw pól i ich typów kodzie ponieważ zmiana nazwy pól w klasie lub tabeli powoduje konieczność zmiany atrybutów w kodzie.
W programowaniu nie chcemy zbyt często czegoś zmieniać, ponieważ przy okazji możemy 
przez przypadek zmienić coś innego.


My chcemy dążyć do automatyzacji, która pozwoli nam "zapomnieć" o metadanych mapowanej klasy tj. nie używać ich w kodzie.

#Krok 1 w kierunku automatyzacji - refleksja


Refleksja pozwala nam automatycznie uzyskać m.in. nazwy pól i typy naszej klasy. 

```csharp
            Person p = new Person();
            Type t = p.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
                Console.WriteLine("Properties: {0}, {1}", prop.Name, prop.PropertyType );

            ///output

            Properties: Id, System.Int32
            Properties: Name, System.String
            Properties: Age, System.Int32
```

Posiadając mechanizm dynamicznego uzyskiwania pól klasy, możemy zmienić kod mapujący.


```csharp
                while (reader.Read())
                {
                   PropertyInfo[] properties = GetProperties<Person>();

                   object[] valuesForConstructor = new object[properties.Length];

                     for  (int i=0;i<valuesForConstructor.Length;i++)
                     {
                        valuesForConstructor[i] = reader.GetValue(i);
                     };
                     return (Person)Activator.CreateInstance(typeof(Person),valuesForConstructor);
                }
               
```               
Spójrz na powyższy kod. Klasa Activator pozwala nam na dynamiczne tworzenie obiektu danego typu. 
Zauważ, że przyjmuje on tablice parametrów typu object. Tablicą tą będą parametry naszego konstruktora. 
Nie wiemy jakiego typu one będą i  nas to nie interesuje bo każdy z nich pośrednio dziedziczy po klasie object,
a więc będzie mógł zostać umieszczony w tablicy. 

Tak więc 
    reader.GetValue (0) i reader. GetValue (2) zwrócą obiekt o typie int.
    reader.GetValue (1) zwróci obiekt o typie string. 
Wszystkie te obiekty umieszczamy w tablicy valuesForConstructor, której następnie używamy do stworzenia Klasy Person.

W ten sposób nadaliśmy naszego kodowi więcej dynamiczności. 

Problem z tą implementacją jest jednak taki, że kolejność argumentów w konstruktorze Person musi być identyczna jak kolejność czytania pól z tabeli. 
Obostrzenie to możemy rozwiązać na dwa sposoby:

1) Stworzenie dodatkowego mechanizmu, który podczas mapowania będzie inteligentnie przydzielał nazwy kolumny tabeli do odpowiednich pól obiektu
2) Stworzenie dodatkowego mechanizmu, który będzie automatycznie tworzył tabelę w taki sposób, żeby kolejność kolumn była taka sama jak kolejność pól w obiekcie.

Drugi scenariusz możemy zaimplementować poniższym kodem, znów używając refleksji:


```csharp
         public void CreateTable<T>()
        {

            Dictionary<Type, string> dict = new Dictionary<Type, string>
            {
                { typeof(Int32),"int" },
                { typeof(string),"varchar(20)"}

            };
        
           using (SqlConnection con = new SqlConnection(_connString))
            {
                con.Open();

                try
                {
                    PropertyInfo[] properties = GetProperties<Person>();
                    string query = "create table " + getTypeName(typeof(Person).ToString()) + "(";
                    for(int i = 0; i < properties.Length; i++)
                    {
                        query += properties[i].Name + " " + dict[properties[i].PropertyType];
                        if (properties[i].Name == "Id" || properties[i].Name== "id") query += " primary key ";
                        query += ", ";
                    }
                    query += ")";
                     
                    using (SqlCommand command = new SqlCommand(query,con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch(Exception e)
                {
                    throw new Exception("Error: " + e.Message);

                }
            }
        }
```

Automatyzacja tworzenia tabeli wymaga od nas informacji na jakie typy w bazie danych mają być mapowane typy c#. 
Informację tę umieściliśmy w słowniku.



