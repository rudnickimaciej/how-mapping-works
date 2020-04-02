# How ORM works

W jaki sposób rekord zapisany w bazie danych np. w tabeli Person przekształcić w obiekt C# klasy Person?

Dane z tabeli Person → new Pracownik().

Najprostszym sposobem  jest bezpośrednie wysłanie zapytania do bazy danych np. takiego:

Select * from Pracownicy where id=2;


Posłużymy się abstrakcją z przestrzenii System.Data.SqlClient.

```
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


```
