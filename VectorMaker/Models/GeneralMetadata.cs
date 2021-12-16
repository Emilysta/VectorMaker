using System.Xml.Linq;

namespace VectorMaker.Utility
{
    /// <summary>
    /// This class holds general metadata for whole app.
    /// </summary>
    internal class GeneralMetadata
    {
        #region Fields
        private XElement m_author = new XElement("Author");
        private XElement m_rights = new XElement("Rights");
        private XElement m_publisher = new XElement("Publisher");
        private XElement m_language = new XElement("Language");
        private XElement m_identifier = new XElement("Identifier");
        private XElement m_date = new XElement("Date");
        private XElement m_metadata = new XElement("Metadata");
        #endregion

        #region Properties
        /// <summary>
        /// Author name as XElement
        /// </summary>
        public XElement Author => m_author;
        /// <summary>
        /// Rights as XElement
        /// </summary>
        public XElement Rights => m_rights;
        /// <summary>
        /// Publisher as XElement
        /// </summary>
        public XElement Publisher => m_publisher;
        /// <summary>
        /// Language as XElement
        /// </summary>
        public XElement Language => m_language;
        /// <summary>
        /// Identifier XElement
        /// </summary>
        public XElement Identifier => m_identifier;
        /// <summary>
        /// Date as XElement
        /// </summary>
        public XElement Date => m_date;
        /// <summary>
        /// Metadata object as XElement that holds all data as children nodes
        /// </summary>
        public XElement MetaData => m_metadata;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public GeneralMetadata()
        {
           MetaData.Add(Author);
           MetaData.Add(Rights);
           MetaData.Add(Publisher);
           MetaData.Add(Language);
           MetaData.Add(Identifier);
           MetaData.Add(Date);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method that saves <see cref="Date"/> node or remode <see cref="Date"/> node from <see cref="MetaData"/>
        /// </summary>
        /// <param name="isToSave"></param>
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
        #endregion
    }
}
