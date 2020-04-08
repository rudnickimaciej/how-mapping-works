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


Foreign key 'FK__Person__Address___5CD6CB2B' references invalid table 'Address'.