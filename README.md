#Jak dziala maper (część 3)

W tym wpisie spróbujemy iść o krok dalej i zaimplementować mapowanie również obiektów niepłaskich.
 
 
 ```csharp
  public class Person
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Address Address { get; set; }

    }

      public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int HomeNo { get; set; }
    }
```

Klasę Person możemy zmapować na dwa sposoby:

1) Sposób piewszy
Stworzymy dwie tablice, jedna będzie przechowywać informacje o Adresie, druga o Person. Tablica Person będzie miała dodatkowo klucz obcy odwołujący
się do tablicy Address

2) Sposób drugi
Stworzymy jedną tablicę Person, ale będzie miała ona dodatkowego kolumny z klasy Address (Street varchar() , City varchar(), City int).
Jest to tak zwane spłaszczanie. Wrzucamy wszystko do jednej tablicy.

My zajmiemy się sposobem pierwszym. 

# Tworzenie struktury tabel

Jak będą wyglądać kwarendy?

```sqlserver
    CREATE TABLE Address(
        Id INT IDENTITY PRIMARY KEY NOT NULL,
        City VARCHAR(20) NOT NULL,
        Street VARCHAR(20) NOT NULL
    )

    CREATE TABLE Person(
        Id INT IDENTITY PRIMARY KEY NOT NULL,
        Name VARCHAR(20),
        Age INT,
        Address_id INT,
        FOREIGN KEY (Address_id) REFERENCES Address(Id)
    )
```

Zauważmy, że kolejność wykonywania tych kwarend musi być dokładnie taka jak powyżej. Jeśli spróbujemy najepierw stworzyć tablice Person, a później
Address, wtedy wyskoczy nam błąd:

```sqlserver 
    Foreign key 'FK__Person__Address___5CD6CB2B' references invalid table 'Address'.
```

Jeśli typ Person miałby więcej typów złożonych, to każda tablica odpowiadająca temu typowi musiała by być tworzona przed tablicą Person.

Co jeśli typ Address skolei miałby w sobie pola złożone? Te pola również musielibyśmy zmapować do odpowiednich tabel.

```csharp

    public class City
    {
        public string Name { get; set; }
        public string PostalCode { get; set; }

    }

     public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public int HomeNo { get; set; }
        public City City { get; set; }
    }

```

W tej sytuacji kwarendy miały by taką postać.

```sqlserver

CREATE TABLE City(
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Name VARCHAR(20) NOT NULL,
    PostalCode VARCHAR(20)
)

CREATE TABLE Address(
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    City_id INT NOT NULL,
    Street VARCHAR(20) NOT NULL,
    FOREIGN KEY (City_id) REFERENCES City(Id)

)

CREATE TABLE Person(
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Name VARCHAR(20),
    Age INT,
    Address_id INT,
    FOREIGN KEY (Address_id) REFERENCES Address(Id)
)

```
Widzimy, że nasze klasy tworzą drzewo zależnosci. Korzeniem drzewa jest typ Person. Address jest węzłem, a City liściem.
Z naszych kwarend wynika, że mapowanie tego drzewa musimy zaczynać od liści. Jest to ważne spostrzeżenie ponieważ definiuje ono flow naszego kodu.

Przed kompilacją nie wiemy jak bardzo złożony będzie drzewo oraz ile będzie mieć wierzchołków. Chcemy móc obsługiwać zarówno mapowanie obiektów
płaskich czyli drzew jednowierzchołkowych jak i bardziej skomplikowanych drzew.
Nasze obostrzenie jest jedank takie, że musi to być drzewo. Nie będziemy więc (przynajmniej narazie) obsługiwać sytuacji w których wierzchołki z
różnych gałęzi mają do siebie referencję.