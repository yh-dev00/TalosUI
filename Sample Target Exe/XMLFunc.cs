using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;

namespace ViCAT_MASTER_LT
{
    class XMLFunc // A class of XML related function
    {
        public string sValidationMessage = " ";

        private const string sXMLValid = "XML File Valid";

        private string sSchemaValidationMessage;

        public bool CheckXMLExtension(String sXMLFilename)
        {
            String sExt = Path.GetExtension(sXMLFilename);

            if (sExt == ".xml")
                return true;
            else
                return false;
        }       
          
        //validate xml schema
        public bool ValidateXMLSchema(String sXMLFilename, String sXMLSchemaResource)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;

            XmlSchemaSet schemas = new XmlSchemaSet();
            settings.Schemas = schemas;

            schemas.Add(null, XmlReader.Create(new StringReader(sXMLSchemaResource)));
            settings.ValidationEventHandler += ValidationEventHandle;

            XmlReader validator = XmlReader.Create(sXMLFilename, settings);
            try
            {       
                while (validator.Read()) { }
                sValidationMessage = sXMLValid;
                return true;   
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                sValidationMessage = sSchemaValidationMessage;
                return false;
            }
            finally
            {
                validator.Close();
            }
        }
        
        // XML Validation callback handler
        private void ValidationEventHandle(object sender, ValidationEventArgs args)
        {
            sSchemaValidationMessage = "XML " + args.Severity.ToString() + ": " + args.Message.ToString() + " (Line " + args.Exception.LineNumber.ToString() + ")";
            Console.WriteLine(sSchemaValidationMessage);
            throw new Exception();
        }
        
        // Retrieve MAC Address from ENI file
        public bool GetMACAddrFromXML(String sXMLFilename, ref String sMACAddr)
        {
           sMACAddr = " ";
           
           XDocument document = XDocument.Load(sXMLFilename);
           XElement MasterElement = document.Root.Elements("Config").Elements("Master").FirstOrDefault();
          
           if (MasterElement != null)
           {
               XElement InfoElement = MasterElement.Element("Info");
               if(InfoElement != null)
               {
                   sMACAddr = (String)InfoElement.Element("Source");
                   return true;
               }
               else
               {
                   return false;
               }
           }
           else
           {
               return false;
           }
        }

        //public String ShortenXMLFilePath(String sFilePath)
        //{
        //    String sPattern = @"(""[^\\]+\\)(?:[^\\""]+\\)*([^""]+"")";
        //    String sInput = sFilePath;
        //    String sReplacement = "$1...\\$2";
        //    Regex rgx = new Regex(sPattern);
        //    String sResult = rgx.Replace(sInput, sReplacement);

        //    return sResult;
        //}

        
        //private bool VerifySchemaFilePath(String sXMLFilename, String sSchemaFilePath, String sXMLNamespace)
        //{
        //    try
        //    {       
        //        XmlDocument xmld = new XmlDocument();
        //        xmld.Load(sXMLFilename);

        //        if(xmld.DocumentElement.NamespaceURI == sXMLNamespace)
        //        {
        //            xmld.Schemas.Add(sXMLNamespace, sSchemaFilePath);
        //            xmld.Validate(ValidationEventHandle);

        //            sValidationMessage = sXMLValid;
        //            return true;
        //        }
        //        else
        //        {
        //            sValidationMessage = sXMLInvalid;
        //            return false;
        //        }
        //    }
        //    catch
        //    {
        //        sValidationMessage = sXMLInvalid;
        //        return false;
        //    }
        //}
    }
}
