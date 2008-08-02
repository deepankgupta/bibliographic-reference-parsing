using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    class Reference
    {
        private string referenceText;
        private string authors;
        public int year;
        private string title;
        private string publication;
        public int seperatorBeforePublication;
        public int seperatorAfterPublication;
        public Reference(string reference)
        {
            this.referenceText = reference;
            this.publication = "";
            this.seperatorAfterPublication = -1;
            this.seperatorBeforePublication = -1;
            this.title = "";
            this.year = -1;
            this.authors = "";
        }

        public string ReferenceText
        {
            get
            {
                return referenceText;
            }
            set
            {
                referenceText = value;
                referenceText = Strip(referenceText);
            }
        }

        public string Authors
        {
            get
            {
                return authors;
            }
            set
            {
                authors = value;
                authors = Strip(authors);
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                title = Strip(title);
            }
        }

        public string Publication
        {
            get
            {
                return publication;
            }
            set
            {
                publication = value;
                publication = Strip(publication);
            }
        }

        

        private string Strip(string input)
        {
            int i;
            for (i = 0; i < input.Length; i++)
            {
                bool flag = false;
                foreach (char ch in Common.seperators)
                {
                    if (input[i] == ch)
                        flag = true;
                }
                if (!flag)
                    break;
            }
            string output = input.Remove(0, i);
            return output;
        }

        public void Display()
        {
            Console.WriteLine("REFERENCE : " + referenceText);
            Console.WriteLine("AUTHORS : " + authors);
            Console.WriteLine("YEAR : " + year);
            Console.WriteLine("TITLE : " + title);
            if (this.seperatorBeforePublication != -1)
            {
                Console.WriteLine("SEPERATOR BEFORE PUBLICATION : "
                    + referenceText[seperatorBeforePublication]);
            }
            Console.WriteLine("PUBLICATION : " + publication);
            if (this.seperatorAfterPublication != -1)
            {
                Console.WriteLine("SEPERATOR AFTER PUBLICATION : " +
                    referenceText[seperatorAfterPublication]);
            }
            Console.WriteLine("\n\n");
            //Console.ReadKey();
        }



        public bool IsValid()
        {
            if (authors == String.Empty || publication == string.Empty || title == string.Empty)
                return false;
            else
                return true;
        }
    }
}
