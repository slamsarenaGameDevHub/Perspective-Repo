using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;

public static class QuestionIdSaveSystem
{
   public static void SaveQuestion(List<int> questionId)
   {
   	BinaryFormatter formatter= new BinaryFormatter();
	   string SavePath=Application.persistentDataPath+"/QuestionId.bs";
	   FileStream stream = new FileStream(SavePath,FileMode.Create);
        formatter.Serialize(stream, questionId);
	   stream.Close();
   }
   public static void AddToList(int itemId)
   {
   	List<int> existingList=LoadList();
	   existingList.Add(itemId);
	   SaveQuestion(existingList);
   }
   
    public static List<int> LoadList()
    {
		string SavePath=Application.persistentDataPath+"/QuestionId.bs";
	
        if (File.Exists(SavePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(SavePath, FileMode.Open))
            {
                return (List<int>)formatter.Deserialize(stream);
            }
        }
        return new List<int>(); // Return an empty list if the file doesn't exist
    }
}
