#Jak dziala maper (część 2)

Dlaczego aktualna implementacja metody Save jest słaba?

```csharp
        public void Save<T>(T obj)
        {
            using(SqlConnection con = new SqlConnection(_connString))
            {
             
                con.Open();
                PropertyInfo[] properties = GetProperties<T>();
                StringBuilder  queryBuilder = new StringBuilder("insert into  " + getTypeName(typeof(T).ToString()) + " VALUES(");
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name == "Id") continue;

                    string value = properties[i].GetValue(obj).ToString();

                    if (properties[i].PropertyType.IsEquivalentTo(typeof(System.String))){
                        value = "'" + value + "'";
                    }
                    queryBuilder.Append(value+",");
                    
                }
                queryBuilder.Remove(queryBuilder.Length - 1, 1);
                queryBuilder.Append(")");

                Console.WriteLine(queryBuilder.ToString());

                try
                {
                    using (SqlCommand command = new SqlCommand(queryBuilder.ToString(),con))
                    {
                        int reader = command.ExecuteNonQuery();
                    }
                }

                catch (Exception e)
                {
                    throw new Exception("Error: " + e.Message);
                }

            }
        }

```csharp

Dotychczas mapować płaską klasę Person. Posiadała ona dwa rodzaje typów: int i string. 
Spróbujmy zmapować inną klasę (również płaską), która oprócz powyższych typów będzie zawierać typ DateTime.



```csharp
public class OrderItem
    {
        public int Id { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }
```

Na ten moment, tylko metoda Get obsługuje typ DateTime. Przeanalizujmy co musimy dodać do metod CreateTable i Save.

# Refoktoring metody CreateTable

 W tej metodzie musimy tylko dodać informacje dotyczącą mapowania .netowskiego typu DateTime na sqlserverowy odpowiednik.
Aktualna wersja kodu pozwala nam to definiować w słowniku.

```csharp
            Dictionary<Type, SqlDbType> dict = new Dictionary<Type, SqlDbType>
            {
                { typeof(Int32), SqlDbType.Int},
                { typeof(string), SqlDbType.Text},
                {typeof(DateTime), SqlDbType.DateTime }

            };
```

#Refaktorin metody Save


Skupimy się tylko na tym co znajduje się wewnątrz pętli for ponieważ tylko tutaj będziemy coś zmieniać. Dla przypomnienia.

 ```csharp
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name == "Id") continue;
                                 
                     string value = properties[i].GetValue(obj).ToString(); //Trywialna konwersja

                    if (properties[i].PropertyType.IsEquivalentTo(typeof(System.String)))
                    {
                         value = "'" + value + "'"; //Mniej trywialna konwersja
                    }

                    queryBuilder.Append(value+",");
                    
                }
```

Dla tego typu kwarenda INSERT będzie wyglądać następująco:

```sql
INSERT INTO OrderItem VALUES(200,'zakończony','2020-20-15')

```

Wartość typu int np. 200 jest w sposób trywialny mapowana do zwykłego stringa "5". String ten następnia trafia do kwarendy jest interpretowany jako liczba.

Wartość typu string np. "zakończony", żeby być traktowany jako varchar po stronie sql musi być otoczony przez dodatkowe apostrofy. 

Konwersja DateTime jest już bardziejs skomplikowana. Oprócz tego, że musimy zrzutować DateTime na String to jeszcze musimy otoczyć go apostrofami.
Dodajmy więc taką logikę to pętli for.



 ```csharp
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name == "Id") continue;


                    string value  = properties[i].GetValue(obj).ToString(); //Trywialna konwersja				
                    
                    if (properties[i].PropertyType.IsEquivalentTo(typeof(System.String))) //Mniej trywialna konwersja
                    {
                       value = "'" + value + "'"; 
                    }

                    if (properties[i].PropertyType.IsEquivalentTo(typeof(System.DateTime)))  //Bardziej złożona konwersja
                    {
                        DateTime val = (DateTime)properties[i].GetValue(obj); 
                        value =val.ToString("yyyy MM dd");
                        value = "'" + value + "'";
                    }
                    
                    queryBuilder.Append(value+",");
                    
                }
```
Wydawać by się mogło, że to koniec. Dodaliśmy kolejny kawałek kodu, który obsługuje pole typu DateTime i zamienia go na stringa w formacie 'yyyy MM dd';
Jeśli jednak spróbujemy wykonać nasz program, a kod natrafi na pole DateTime klasy OrderItem, wtedy wyskoczy nam następujący błąd.

```csharp 
        System.Exception: 'Error: Conversion failed when converting date and/or time from character string.'
```csharp 

Kod natrafiając na pole z typem DateTime, próbuje je konwertować na string za pomocą zwykłej metody ToString(). W przypadku typu int oraz string metoda ta sprawdziła się,
ale w przypadku DateTime już nie. Dzieje się to dlatego, że przy konwertowaniu DateTime na string musimy jako argument ToString() podać format np. "yyyy MM dd".
Chcemy taką konwersję przeprowadzać tylko dla DateTime. Można to zrobić, dodając kilka nowych ifów i elsów.

 ```csharp
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name == "Id") continue;

                     string value =0;


                    if (properties[i].PropertyType.IsEquivalentTo(typeof(System.DateTime)))
                    {
                        DateTime val = (DateTime)properties[i].GetValue(obj);
                        value =val.ToString("yyyy-MM-dd");
                        value = "'" + value + "'";
                    }

                    else
                    {
                        value  = properties[i].GetValue(obj).ToString(); //Trywialna konwersja				                              

                        if (properties[i].PropertyType.IsEquivalentTo(typeof(System.String)))
                        {
                             value = "'" + value + "'"; //Mniej trywialna konwersja
                        }
                    }

                    queryBuilder.Append(value+",");
                    
                }
```

Do czego zmierzam? 
Próba dodania możliwości mapowania dodatkowego typu skończyła się okropnym, proceduralnym kodem, pełnym ifów. Takie podejście jest 
nieutrzymywalne, szczególnie gdy będziemy chcieli dodać obsługę większej ilości typów.

Kasujemy więc całę wnętrze pętli for i próbujemy zaimplementować ją w lepszy, bardziej obiektowy sposób.

#Refaktoring metody Save ciąg dalszy

