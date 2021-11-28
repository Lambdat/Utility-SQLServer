using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Utility_SQLServer
{
    //Questa classe si occuperà connettersi al Database
    //e permettere di eseguire query

    // Questa classe seguirà quindi il Patteren Facade dato
    // dato che sarà la sola classe a fare tutte le varie operazioni
    // sulle connessioni 


    public class Database
    {

        private SqlConnection _con;
        

        // All'inizializzazione dell'oggetto di tipo Database va passato al metodo costruttore la connection string
        // presa dal file di configurazione del progetto (es. da ASP.NET 6 in Program.cs)

        // il parametro string name è opzionale, può essere cambiato a seconda delle connection strings presenti nel
        // file appsettings.json sempre di configurazione

        public Database(string connectionStringInCode, IConfiguration config, string connectionStringNameInConfigFile = "Default")
        {


            string parametriEsternalizzati = config.GetConnectionString(connectionStringNameInConfigFile);



            if (!string.IsNullOrWhiteSpace(parametriEsternalizzati))
            {

                try
                {
                    _con = new SqlConnection(parametriEsternalizzati);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                                          "Errore nella ConnectionString presente nel file di configurazione,\n" +
                                                $"ECCEZIONE: {ex.Message} \n" +
                                          "Tentativo con la ConnectionString interna al codice (inserita in fase di inizializzazione al metodo costruttore) ... \n"
                                      );
                    try
                    {
                        _con = new SqlConnection(connectionStringInCode);
                    }
                    catch (Exception ConnectionNotFound)
                    {
                        Console.WriteLine(
                                        "Errore nello stabilire connessione con il Database SQL Server \n" +
                                                $"ECCEZIONE: {ConnectionNotFound.Message} \n"
                                     );
                    }
                }
            }
            else    //Nel caso il file di configurazione non contenesse alcuna stringa di connessione
            {

                try
                {
                    _con = new SqlConnection(connectionStringInCode);

                }
                catch (Exception ConnectionNotFound)
                {
                    Console.WriteLine(
                                        "Errore nello stabilire connessione con il Database SQL Server \n" +
                                                $"ECCEZIONE: {ConnectionNotFound.Message} \n"
                                     );
                }
            }

        }

        public List<Dictionary<string,string>> Read(string query)
        {

            List<Dictionary<string, string>> ris = new List<Dictionary<string, string>>();

            //Aprire la connessione grazie all'oggetto SqlConnection con
            _con.Open();


            //SQLCommand per l'esecuzione della query arrivata come parametro
            SqlCommand cmd = new SqlCommand(query, _con);

            //DataReader che conterrà il risultato dell'esecuzione del comando
            SqlDataReader dr = cmd.ExecuteReader();

            //Cicleremo il nostro DR e ciascuna riga la trasformeremo in un Dictionary
            while (dr.Read()) // UNA RIGA PER VOLTA (LETTURA VERTICALE)
            {

                //Come chiavi ci inseriremo i nomi delle colonne
                //come valori ciò che è scritto nella tabella
                Dictionary<string, string> riga = new Dictionary<string, string>();

                for(int i = 0; i < dr.FieldCount; i++)
                {
                                        // es.  Dictionary<id,1>
                    riga.Add(dr.GetName(i).ToLower(), dr.GetValue(i).ToString());

                }

                //Aggiungiamo il singolo Dictionary<string,string> all'apposita lista per il ritorno
                ris.Add(riga);




            }

            //Chidiamo DataReader e Connessione
            dr.Close();
            _con.Close();

            return ris;

        }

        //Questo metodo è invece indicato nella proiezione singola, come:
        /*
         * 1) La Selezione di un singolo elemento attraverso la clausola WHERE
         * 
         * 2) La Proiezione di UNA Funzione di Aggregazione (AVG,SUM,COUNT)
        */
        public Dictionary<string,string> ReadOne(string query)
        {

            try
            {
                return Read(query)[0]; //Restituiamo la prima riga della Lista di Dictionary<string,string>
            }
            catch(Exception e) // Entriamo nel costrutto catch qualora non avessimo ottenuto risultati
            {                   // dalla query

                Console.WriteLine($"\n {e.Message} \n");


                return null;

            }

        }

        //Questo metodo è rivolto ai comandi della sintassi Data Maniupulation Language (DML), come:
        /*
         * 1) UPDATE <table> SET <colonna=nuovo valore> WHERE <condizione>
         * 
         * 2) INSERT INTO <table>("nomi colonne specifiche") VALUES (...)
         * 
         * 3) DELETE FROM <table> WHERE <condizione>
         */

        public void Update(string query)
        {

            try
            {
                _con.Open();

                SqlCommand cmd = new SqlCommand(query, _con);

                cmd.ExecuteNonQuery();

            }
            catch(Exception e)
            {
                Console.WriteLine($"\n {e.Message} \n");

                Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n" +
                                    "La seguente query ha generato un errore: \n" +
                                    $"{query} + \n" +
                                  "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n");

            }
            finally  // In ogni caso (che sia try o catch) viene chiusa la connessione
            {
                _con.Close();
            }

        }


    }
}
