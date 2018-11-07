using Horsesoft.Music.Data.Model.Import;
using Horsesoft.Music.Data.Model.Tags;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Horsesoft.Music.Engine.Tagging
{
    public interface ISongTagger
    {
        SongTagFile PopulateSongTag(string fileName, TagOption options = TagOption.All);

        bool UpdateFileTag(string fileName, byte rating);
    }

    public abstract class SongTagger : ISongTagger
    {
        public virtual SongTagFile PopulateSongTag(string fileName, TagOption options = TagOption.All)
        {
            throw new NotImplementedException();
        }

        public virtual string GetFileHash(string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }          
        }

        public virtual bool UpdateFileTag(string fileName, byte rating)
        {
            throw new NotImplementedException();
        }
    }
}
