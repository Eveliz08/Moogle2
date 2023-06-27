namespace MoogleEngine;
using System.Text.Json;
using System;

public static class Moogle
{
    public static  SearchResult Query(string query) 
    {
      //Estas líneas de código se ejecutarán una sola vez: en la primera ejecución del programa
      //cuando la variable executionsProgram es 0.
      if(executionsProgram == 0)
      {
        OpenDataBase();                             //Descarga la información que se dispone.
        StaticMatrix.ToDoMatrix(content);           //Crea una matriz con esa información.
      }

      executionsProgram++; 


      //Se crea con el query dos vectores con el método QueryVector. Uno textual a las palabras del usuario y otro que incluye sinónimos.
      //QueryVector también devuelve las palabras del query separadas en un arreglo sin signos de puntuación.
      var vectors = QueryVectors.QueryVector(query);

      //Se realiza la búsqueda con la multiplicación de los dos vectores por la matriz que contiene toda la información que se dispone.
      //El resultado será dos vectores con el score de la búsqueda en cada documento.
      float[] scoreQuery = OperationsVectors.Multiplication(vectors.Item1); 
      float[] scoreSuggestion = OperationsVectors.Multiplication(vectors.Item2);
        
      //Ahora se busca el documento que más alto score tiene por cada vector.
        float score1 = 0.0f;       //Va a guardar el documento con el score más alto.
        int position = 0;       //Va a guardar la posición del documento con el score más alto.
        for(int k = 0; k <scoreQuery.Length; k++)
        {
          if(score1 < scoreQuery[k])
          {
          score1 = scoreQuery[k];
          position = k;
          }
        }

        float score2 = 0.0f;      //Va a guardar el documento con el score más alto del vector sugerencia.
        int position2 = 0;     //Va a guardar la posición del documento con el score más alto que responde al vector sugerencia.
         for(int k = 0; k < scoreSuggestion.Length && k!= position; k++)
        {
          if(score2 < scoreSuggestion[k])
          {
          score2 = scoreSuggestion[k];
          position2 = k;
          }
        }

        if (score1 == 0.0f)
        {
          SearchItem[] items1 = new SearchItem[1] {
          new SearchItem("Lo sentimos, no se han encontrado resultados para su búsqueda", " ", score1)  };

         return new SearchResult(items1, query);
        }


       SearchItem[] items = new SearchItem[2] {
           new SearchItem(files[position], Snippet(position, vectors.Item3), score1),
           new SearchItem(files[position2], Snippet(position2, vectors.Item3),score2),

        };

        return new SearchResult(items, query);
    }




  public static void OpenDataBase()
  {
    files = Directory.GetFiles(Path.Combine("..", "Content"));
    totalDoc =  files.Length; 

    content = new string[totalDoc];
    lengthDoc = new int[totalDoc];
     
    for (int i = 0; i < totalDoc; i++)
   {
      content[i] = File.ReadAllText(files[i]);
      lengthDoc[i] = content[i].Split(' ').Length;
      lengthDataBase += lengthDoc[i]; //Se introduce la cantidad de palabras de cada documento, el mayor será la dimensión de la matriz y el vector query
    }  

    textSplitter = new float[totalDoc, lengthDataBase];
    
   Synonyms = JsonSerializer.Deserialize<Dictionary<string, string[]>>(File.ReadAllText("../MoogleEngine/Synonymous.json"))!; 
  
  }




  private static string Snippet(int position, string[] queryPecked )
  {
    //Se abre el documento que guarda la variable position. Este documento se va a guarar linea a linea en el string snippet.
    //Se va a comprobar si la linea tiene alguna palabra del query.
    StreamReader doc = new StreamReader(files[position]);
    string snippet = doc.ReadLine()!;

    while(snippet != null)
    {
      for(int i = 0; i < queryPecked.Length; i++)
      {
        if(StaticMatrix.Cleaner(snippet).Contains(queryPecked[i]) && queryPecked[i] != " ")
        {
          goto getout; //Cuando se encuentre la primera línea que contenga cualquier palabra del query se detendrá ambos bucles y se devolverá esta   
        }
      }
      snippet = doc.ReadLine()!;
    }
    getout:
   return snippet!;
  }


 
    private static string[] files;
    public static string[] content;
    
    public static int lengthDataBase;
    public static int[] lengthDoc; 
    
    public static int totalDoc;
    public static float[,] textSplitter; 
   public static Dictionary<string, string[]> Synonyms;
   private static int executionsProgram = 0;

}  
