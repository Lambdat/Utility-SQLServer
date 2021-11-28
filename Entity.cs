using System.Reflection;

namespace Utility_SQLServer
{
    /*
     * 
     *
     * La Classe Entity può essere vista come una Classe Padre che può essere ereditata da tutte le altre classi tipo.
     * All' Interno di questa classe sono presenti solo due metodi helper
     * che possono ritornare utili:
     * 
     * 1) Metodo ToString() sovrascritto dalla classe Object per restituire
     *  il contenuto dell'oggetto sotto forma di stringa
     *  
     * 2) Metodo FromDictionary() per caricare le proprietà dell'oggetto con il 
     *  Dictionary<string,string> raccolto dalla lettura da DataBase
     *  
     *  Entrambi questi metodi si avvalgono del procedimento di Reflection,
     *  ossia la capacità di un oggetto di ispezionarsi per restituire o fare qualcosa
     *  
     */



    public abstract class Entity
    {

        public override string ToString()
        {
            string ris = base.ToString();

            //PropertyInfo è un oggetto che conterrà tutte le informazioni di una
            //determinata proprietà


            //this.GetType().GetProperties():

            //this -> indica la classe Entity

            //GetType() -> è un metodo che si trova nella classe Object che ci restituisce
            //             un oggetto di tipo Type

            //GetProperties() -> è un metodo che si trova nella classe Type e ci restituisce
            //                   un vettore di oggetti PropertyInfo


            //Cicliamo una ad una tutte le proprietà dell'oggetto 
            foreach (PropertyInfo proprieta in this.GetType().GetProperties())
            {
                ris += proprieta.Name + " : " + proprieta.GetValue(this) + "\n";
            }


            return ris;
        }







        // Questo metodo riceve come parametro di ingresso un dizionario,
        // nel nostro immaginario riceverà la riga della tabella del database
        // e valorizzerà le proprietà PRIMITIVE dell'oggetto ( NON ALTRE PROPRIETÀ DI CLASSI TIPO)
        // pescandole dal Dictionary

        public void FromDictionary(Dictionary<string, string> riga)
        {

            foreach(PropertyInfo proprieta in this.GetType().GetProperties())
            {
                // Se il Dictionary contiene come chiave il nome della Proprietà dell'oggetto
                if (riga.ContainsKey(proprieta.Name.ToLower()))
                {
                    //in un oggetto temporaneo andiamo a mettere il valore di quella chiave
                    object valore = riga[proprieta.Name.ToLower()];


                    // Ora con un costrutto di controllo switch, vado a capire il nome del TIPO
                    // dell oggetto valore e opportunamente vado a parsare il valore

                    switch (proprieta.PropertyType.Name.ToLower())
                    {
                        case "int32":
                            valore = int.Parse(riga[proprieta.Name.ToLower()]);
                            break;

                        case "double":
                            valore = double.Parse(riga[proprieta.Name.ToLower()]);
                            break;

                        case "float":
                            valore = float.Parse(riga[proprieta.Name.ToLower()]);
                            break;

                        case "char":
                            valore = char.Parse(riga[proprieta.Name.ToLower()]);
                            break;

                        case "boolean":
                            valore = bool.Parse(riga[proprieta.Name.ToLower()]);
                            break;

                        case "datetime":
                            valore = DateTime.Parse(riga[proprieta.Name.ToLower()]);
                            break;

                        default:
                            Console.WriteLine("Tipo da convertire non riconosciuto");
                            break;
                    }



                    proprieta.SetValue(this, valore);


                }


            }



        }











    }



}
