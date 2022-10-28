using System.IO;//use when accessing files on os
using System.Runtime.Serialization.Formatters.Binary; //Allows access and use of binary formatter
using UnityEngine;


//used for saving and loading data by first turning specific object into binary and then back
<<<<<<< HEAD
public static class FileReadWrite
{
  public static void WriteToBinaryFile<T>(string filePath, T objectToWrite) //method to write object of any type to chosen path
=======
public class FileReadWrite
{
  public static void WriteToBinaryFile<T>(string filePath, T objecToWrite) //method to write object of any type to chosen path
>>>>>>> 3a83a3c8e036bab92ad2197625257791d261df8f
  {
    using (Stream stream = File.Open(filePath, FileMode.Create)) //if file exists, overwrite, otherwise create new file
    {
      var binaryFormatter = new BinaryFormatter(); //new object of type binaryformatter
      binaryFormatter.Serialize(stream, objectToWrite); //serialize converts file to binary
      //stream.Close() is not needed to be called explicitly, it is done automatically when using is done
    }
  }
<<<<<<< HEAD
  public static T ReadFromBinaryFile<T>(string filePath)
  {
    using (Stream stream = File.Open(filePath, FileMode.Open))
    {
      var binaryFormatter = new BinaryFormatter();
      return (T)binaryFormatter.Deserialize(stream);
    }
  }
}
=======
  publc static T ReadFromBinaryFile<T>(string filePath)
  {
    using (Stream stream - File.Open(filePath, FileMode.Open))
    {
      var binaryFormatter - new BinaryFormatter();
      return (T)binaryFormatter.Deserialize(stream);
    }
  }
>>>>>>> 3a83a3c8e036bab92ad2197625257791d261df8f
