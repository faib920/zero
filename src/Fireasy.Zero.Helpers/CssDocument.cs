using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Fireasy.Zero.Helpers
{
    public class CssParser
    {
        private int m_idx;

        public static bool IsWhiteSpace(char ch)
        {
            return ("\t\n\r ".IndexOf(ch) != -1);
        }

        public void EatWhiteSpace()
        {
            while (!Eof())
            {
                if (!IsWhiteSpace(GetCurrentChar()))
                {
                    return;
                }

                m_idx++;
            }
        }

        public bool Eof()
        {
            return (m_idx >= Source.Length);
        }

        public string ParseElementName()
        {
            var element = new StringBuilder();
            EatWhiteSpace();
            while (!Eof())
            {
                if (GetCurrentChar() == '{')
                {
                    m_idx++;
                    break;
                }

                element.Append(GetCurrentChar());
                m_idx++;
            }

            EatWhiteSpace();
            return element.ToString().Trim();
        }

        public string ParseAttributeName()
        {
            var attribute = new StringBuilder();
            EatWhiteSpace();

            while (!Eof())
            {
                if (GetCurrentChar() == ':')
                {
                    m_idx++;
                    break;
                }

                attribute.Append(GetCurrentChar());
                m_idx++;
            }

            EatWhiteSpace();
            return attribute.ToString().Trim();
        }

        public string ParseAttributeValue()
        {
            var attribute = new StringBuilder();
            EatWhiteSpace();
            while (!Eof())
            {
                if (GetCurrentChar() == ';')
                {
                    m_idx++;
                    break;
                }

                attribute.Append(GetCurrentChar());
                m_idx++;
            }

            EatWhiteSpace();
            return attribute.ToString().Trim();
        }

        public char GetCurrentChar()
        {
            return GetCurrentChar(0);
        }

        public char GetCurrentChar(int peek)
        {
            if ((m_idx + peek) < Source.Length)
                return Source[m_idx + peek];
            else
                return (char)0;
        }

        public char AdvanceCurrentChar()
        {
            return Source[m_idx++];
        }

        public void Advance()
        {
            m_idx++;
        }

        public string Source { get; set; }

        public List<CssElement> Parse()
        {
            var elements = new List<CssElement>();

            while (!Eof())
            {
                string elementName = ParseElementName();

                if (string.IsNullOrEmpty(elementName))
                {
                    break;
                }

                var element = new CssElement(elementName);

                var name = ParseAttributeName();
                var value = ParseAttributeValue();

                while (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    element.Add(name, value);

                    EatWhiteSpace();

                    if (GetCurrentChar() == '}')
                    {
                        m_idx++;
                        break;
                    }

                    name = ParseAttributeName();
                    value = ParseAttributeValue();
                }

                elements.Add(element);
            }

            return elements;
        }
    }

    public class CssDocument
    {
        public string Text { get; set; }

        public List<CssElement> Elements { get; set; }

        public CssElement this[string name]
        {
            get
            {
                for (int i = 0; i < Elements.Count; i++)
                {
                    if (Elements[i].Name.Equals(name))
                        return Elements[i];
                }

                return null;
            }
        }

        public string File { get; set; }

        public void Load(string file)
        {
            using (var sr = new StreamReader(file))
            {
                Text = sr.ReadToEnd();
                sr.Close();
            }

            var parse = new CssParser();
            parse.Source = Regex.Replace(Text, @"/\*.*?\*/", "", RegexOptions.Compiled);
            Elements = parse.Parse();
        }

        public void Add(CssElement element)
        {
            Elements.Add(element);
        }

        public void Save()
        {
            Save(File);
        }

        public void Save(string file)
        {
            using (var sw = new StreamWriter(file, false))
            {
                foreach (var element in Elements)
                {
                    sw.WriteLine(element.Name + " {");
                    foreach (string name in element.Attributes.AllKeys)
                    {
                        sw.WriteLine("\t{0}:{1};", name, element.Attributes[name]);
                    }
                    sw.WriteLine("}");
                }
                sw.Flush();
                sw.Close();
            }
        }
    }

    public class CssElement
    {
        public string Name { get; set; }

        public NameValueCollection Attributes { get; set; }

        public CssElement(string name)
        {
            this.Name = name;
            Attributes = new NameValueCollection();
        }

        public void Add(string attribute, string value)
        {
            Attributes[attribute] = value;
        }
    }
}
