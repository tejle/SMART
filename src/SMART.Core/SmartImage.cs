using System;

namespace SMART.Core
{
  [Serializable]
  public class SmartImage : object
  {
    private readonly byte[] image;



    public SmartImage(byte[] image)
    {
      this.image = image;
    }

    public SmartImage(string hexString)
    {
      if (hexString != null)
      {
          try
          {
              // pad if not even pairs 
              if (hexString.Length % 2 == 1) hexString = '0' + hexString;

              var data = new byte[hexString.Length / 2];
              for (var i = 0; i < data.Length; i++)
                  data[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
              image = data;
          }
          catch
          {              
              //
          }

      }
    }

    public byte[] Image
    {
      get { return image; }
    }

  }
}
