using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;

class QueryVectors
{
    //Este método va a devolver 3 vectores: uno con el IDF de cada palabra del query y otro que además de estas palabras 
    //contiene otras como sugerencia. El tercer vector es el query recortado en palabras, el cual se usará en el snippet.
    public static (float[], float[], string[]) QueryVector(string query) 
    {

        float[] vector = new float[MoogleEngine.Moogle.lengthDataBase];   //La dimensión es el total de palabras del vocabulario
        float[] vectorSugerencias = new float[MoogleEngine.Moogle.lengthDataBase];

        double valor_palabra = 1; //Esta variable cambia de valor si el usuario intruduce alguna simbología especial como: ^ * !

        query = StaticMatrix.Cleaner(query);
        string[] queryPecked = query.Split();


        for (int i = 0; i < queryPecked.Length; i++)
        {
            if (StaticMatrix.Vocabulary.ContainsKey(queryPecked[i]))  
            {
               valor_palabra = SearchRequests(queryPecked[i]);
               queryPecked[i] = Regex.Replace(queryPecked[i], "\\p{P}", string.Empty); 
            
               vector[StaticMatrix.Key_Vocabulary(queryPecked[i])] = (float)(valor_palabra * Math.Log10((MoogleEngine.Moogle.totalDoc + 1) / TdocQuery(queryPecked[i])));
               vectorSugerencias[StaticMatrix.Key_Vocabulary(queryPecked[i])] = vector[StaticMatrix.Key_Vocabulary(queryPecked[i])] ;

               vectorSugerencias = AddSynonyms(queryPecked[i], vectorSugerencias);
            }
            else
            {
                
                queryPecked[i] = queryPecked[i].Replace(queryPecked[i], " ");
            }
        }
        return(vector, vectorSugerencias, queryPecked);
    } 

    private static double SearchRequests(string query)
    {
        if (query.IndexOf("!") == 0) { return 0; }
         if (query.IndexOf("^") == 0) { return 100; }
        int i = 0;
        while (query.IndexOf("*") == i) { i++; }
        return Math.Pow(2, i);
    }

    private static int TdocQuery (string a)
    {
        int D = 0;
       for(int k = 0; k < MoogleEngine.Moogle.totalDoc; k++)
       {
          if(MoogleEngine.Moogle.content[k].Contains(a))
          D++;
       } 
       return D; 
    }

    public static float[] AddSynonyms(string aPecked, float[] vectorSugerencias) 
    { 
        if(MoogleEngine.Moogle.Synonyms.ContainsKey(aPecked))
        {
            var sinonimos = MoogleEngine.Moogle.Synonyms[aPecked];
            for(int i = 0; i < sinonimos.Length; i++)
            {
                if(StaticMatrix.Vocabulary.ContainsKey(sinonimos[i]))
                   vectorSugerencias[StaticMatrix.Key_Vocabulary(sinonimos[i])] = 1/2 * (float)Math.Log10((double)(MoogleEngine.Moogle.totalDoc + 1)/TdocQuery(sinonimos[i]));
            }
        } 
        return vectorSugerencias;
    }
}