using System.Xml.Linq;

namespace VectorMaker.Utility
{
    internal class GeneralMetadata
    {
        private XElement m_author = new XElement("Author");
        private XElement m_rights = new XElement("Rights");
        private XElement m_publisher = new XElement("Publisher");
        private XElement m_language = new XElement("Language");
        private XElement m_identifier = new XElement("Identifier");
        private XElement m_date = new XElement("Date");
        private XElement m_metadata = new XElement("Metadata");

        public XElement Author => m_author;
        public XElement Rights => m_rights;
        public XElement Publisher => m_publisher;
        public XElement Language => m_language;
        public XElement Identifier => m_identifier;
        public XElement Date => m_date;
        public XElement MetaData => m_metadata;

        public GeneralMetadata()
        {
           MetaData.Add(Author);
           MetaData.Add(Rights);
           MetaData.Add(Publisher);
           MetaData.Add(Language);
           MetaData.Add(Identifier);
           MetaData.Add(Date);
        }

        public void SetDateInMetadata(bool isToSave)
        {
            if (isToSave)
                MetaData.Add(Date);
            else
            {
                if(Date.Parent!=null)
                    Date.Remove();
            } 
        }
    }
}
