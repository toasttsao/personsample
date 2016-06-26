using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigXmlInsertTableSample
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class bookstory
    {

        private List<book> bookField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("books")]
        public List<book> books
        {
            get
            {
                return this.bookField;
            }
            set
            {
                this.bookField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class book
    {


        private string createDateFieldField;

        private decimal amountField;

        private string BookNameField;


        private string bookidField;
        /// <remarks/>
        public string CreateDate
        {
            get
            {
                return this.createDateFieldField;
            }
            set
            {
                this.createDateFieldField = value;
            }
        }

        /// <remarks/>
        public decimal Amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
        /// <remarks/>
        public string BookName
        {
            get
            {
                return this.BookNameField;
            }
            set
            {
                this.BookNameField = value;
            }
        }
        /// <remarks/>
        public string BookId
        {
            get
            {
                return this.bookidField;
            }
            set
            {
                this.bookidField = value;
            }
        }
    }


}
