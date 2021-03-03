using System;
using System.IO;
using Utility.CommandLine;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Linq;

namespace xsltrans {
    public static class Program {
        [Argument('x', "xslt", "XSLT File Path")]
        private static string xsltPath { get; set; }
        [Argument('f', "file", "XML File to transform (cannot specify both file and folder)")]
        private static string xmlfile { get; set; }
        [Argument('F', "folder", "Folder path containing XML Files to transform (cannot specify both file and folder)" )]
        private static string xmlFolder { get; set; }
        [Argument ( 'i', "include", "When transforming XML files by folder specify file extensions to include (ex: -i *.xml or --include *.xml)" )]
        private static string xmlFileSearchPattern { get; set; }
        [Argument('o', "out", "Output file path")]
        private static string outputFile { get; set; }
        [Argument('h', "help", "displays the help")]
        private static bool help { get; set; }
        [Operands]
        private static string[] operands { get; set; }
        static void Main ( string[] args ) {
            Arguments.Populate ( );
            if(help) {
                ShowHelp ( );
                return;
            }

            if(!File.Exists(xsltPath)) {
                Console.WriteLine ( "XLST path does not exist" );
                return;
            }
            if(!string.IsNullOrEmpty(xmlfile) && !string.IsNullOrEmpty(xmlFolder)) {
                Console.WriteLine ( "You must only specify the XML File (-f --file) or the XML Folder (-F --folder), but not both" );
                return;
            }
            var isSingleFile = !string.IsNullOrWhiteSpace(xmlfile) && string.IsNullOrWhiteSpace(xmlFolder);
            if(!string.IsNullOrWhiteSpace(xmlfile) && !File.Exists(xmlfile) && string.IsNullOrWhiteSpace(xmlFolder)) {
                Console.WriteLine ( "XML file does not exist" );
                return;
            }
            if(!string.IsNullOrWhiteSpace(xmlFolder) && Directory.GetFiles(xmlFolder).Length == 0) {
                Console.WriteLine ( "XML folder path does not contain any files" );
                return;
            }
            var transform = new XslCompiledTransform ( );
            using ( var xsltReader = new XmlTextReader ( File.OpenRead ( xsltPath ) ) ) {
                var settings = new XsltSettings ( true, true );
                var resolver = new XmlUrlResolver ( );
                transform.Load ( xsltReader, settings, resolver );
            }
            using ( var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite) ) {
                if ( isSingleFile ) {
                    using ( var inputStream = new StreamReader ( xmlfile, true ) ) {
                        if( !TransformXml ( inputStream, transform, outputStream ) ) {
                            Console.WriteLine ( $"Failed to transform {xmlfile}" );
                            return;
                        }
                    }
                } else {
                    var files = Directory.GetFiles ( xmlFolder, xmlFileSearchPattern ?? "*.*" );
                    foreach(var filePath in files) {
                        using ( var inputStream = new StreamReader ( filePath, true ) ) {
                            if ( !TransformXml ( inputStream, transform, outputStream ) ) {
                                Console.WriteLine ( $"Failed to transform {xmlfile}" );
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static bool TransformXml ( StreamReader inputXmlStream, XslCompiledTransform transform, Stream outputStream ) {
            var success = false;
            // validate incoming xml stream
            if ( inputXmlStream != null && inputXmlStream.BaseStream.Length > 0 ) {
                // ensure output stream is valid (or use memory stream)
                if ( outputStream == null ) {
                    Console.Error.WriteLine ( "outputStream was null, using an in memory stream for transform output" );
                }
                // validate output stream can handle our output (require file buffer if xml is large enough)
                //if ( outputStream is MemoryStream && inputXmlStream.BaseStream.Length > ushort.MaxValue ) {
                //    var exMsg = string.Format ( "Cannot transform large XML files using a Memory Stream (outputStream must be a file stream)\r\nXML Size (bytes) {0}", inputXmlStream.Length );
                //    Console.Error.WriteLine ( exMsg );
                //    throw new InvalidOperationException ( exMsg );
                //}
                Console.Error.WriteLine (  "Starting transform" );
                // load the document and perform the transform
                var xmlDoc = new XPathDocument ( inputXmlStream );
                var argList = new XsltArgumentList ( );
                try {
                    transform.Transform ( xmlDoc, argList, new XmlTextWriter ( outputStream, new UTF8Encoding ( false ) ) );
                    success = true;
                    //Console.Error.WriteLine (  "Transformation complete" );
                } catch ( Exception ex ) {
                    success = false;
                    Console.Error.WriteLine ( "Error transforming XML.  Details: {0}", ex );
                }
            }
            return success;
        }

        private static void ShowHelp ( ) {
            var helpAttributes = Arguments.GetArgumentInfo ( typeof ( Program ) );

            var maxLen = helpAttributes.Select ( a => a.Property.PropertyType.ToColloquialString ( ) ).OrderByDescending ( s => s.Length ).FirstOrDefault ( ).Length;

            Console.WriteLine ( $"Short\tLong\t{"Type".PadRight ( maxLen )}\tFunction" );
            Console.WriteLine ( $"-----\t----\t{"----".PadRight ( maxLen )}\t--------" );

            foreach ( var item in helpAttributes ) {
                var result = item.ShortName + "\t" + item.LongName + "\t" + item.Property.PropertyType.ToColloquialString ( ).PadRight ( maxLen ) + "\t" + item.HelpText;
                Console.WriteLine ( result );
            }
        }

        public static string ToColloquialString ( this Type type ) {
            return ( !type.IsGenericType ? type.Name : type.Name.Split ( '`' )[0] + "<" + String.Join ( ", ", type.GetGenericArguments ( ).Select ( a => a.ToColloquialString ( ) ) ) + ">" );
        }
    }
}
