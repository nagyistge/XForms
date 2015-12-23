using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  internal class Deserializer : IDeserializer
  {
    private const string propertyStoreFile = "PropertyStore.forms";

    public Task<IDictionary<string, object>> DeserializePropertiesAsync()
    {
      return Task.Run<IDictionary<string, object>>((Func<IDictionary<string, object>>) (() =>
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile("PropertyStore.forms", System.IO.FileMode.OpenOrCreate))
          {
            using (var binaryReader = XmlDictionaryReader.CreateBinaryReader((Stream) storageFileStream, XmlDictionaryReaderQuotas.Max))
            {
              if (storageFileStream.Length == 0L)
                return (IDictionary<string, object>) null;
              try
              {
                return (IDictionary<string, object>) new DataContractSerializer(typeof (Dictionary<string, object>)).ReadObject(binaryReader);
              }
              catch (Exception ex)
              {
              }
            }
          }
        }
        return (IDictionary<string, object>) null;
      }));
    }

    public Task SerializePropertiesAsync(IDictionary<string, object> properties)
    {
      properties = (IDictionary<string, object>) new Dictionary<string, object>(properties);
      return Task.Run((Action) (() =>
      {
        bool flag = false;
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile("PropertyStore.forms.tmp", System.IO.FileMode.OpenOrCreate))
          {
            using (XmlDictionaryWriter binaryWriter = XmlDictionaryWriter.CreateBinaryWriter((Stream) storageFileStream))
            {
              try
              {
                new DataContractSerializer(typeof (Dictionary<string, object>)).WriteObject(binaryWriter,  properties);
                binaryWriter.Flush();
                flag = true;
              }
              catch (Exception ex)
              {
              }
            }
          }
        }
        if (!flag)
          return;
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          try
          {
            if (storeForApplication.FileExists("PropertyStore.forms"))
              storeForApplication.DeleteFile("PropertyStore.forms");
            storeForApplication.MoveFile("PropertyStore.forms.tmp", "PropertyStore.forms");
          }
          catch (Exception ex)
          {
          }
        }
      }));
    }
  }
}
