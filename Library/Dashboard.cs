using System;
using System.Collections.Generic;
using System.Text;

namespace com.MovieAssistant.core
{
    public enum SaveType { XML, JSON, Binary }
    public static class Dashboard
    {

        public static void Save(string path, ModleTree modleTree, SaveType saveType)
        {
            throw new NotImplementedException();
        }
        public static ModleTree Load(string path)
        {
            throw new NotImplementedException();
        }
        public static Tree Download(ModleTree modleTree, string name)
        {
            throw new NotImplementedException();
        }
    }
}
