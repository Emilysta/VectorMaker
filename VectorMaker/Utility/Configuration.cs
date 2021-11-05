using Newtonsoft.Json;
using System.Diagnostics;

namespace VectorMaker.Utility
{
    class Configuration
    {
        public string Name = "test";
        

        private static Configuration m_instance;
        public static Configuration Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Configuration();
                return m_instance;
            }
        }

        private Configuration() {
            SaveToFile();
        }

        private void SaveToFile()
        {
            string value = JsonConvert.SerializeObject(this,Formatting.Indented);
            Trace.WriteLine(value);

            JsonConvert.PopulateObject(value, this);
        }
    }
}
