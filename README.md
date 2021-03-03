# xmltrans - An XML Transform Utility
A simple xml transform command line utility.  Can be run on a single xml file or a folder containing multiple files.

## Command Line Args
| Short | Long | Description |
| ------|------|-------------|
|x| xslt| XSLT File Path|
|f|file|XML File to transform (cannot specify both file and folder)|
|F|folder|Folder path containing XML Files to transform (cannot specify both file and folder)|
|i|include|When transforming XML files by folder specify file extensions to include (ex: -i *.xml or --include *.xml)|
|o|out|Output file path|
|h|help|Displays the help|

## Example

```bash
$ xmltrans -x ~/xml2csv.xslt -F ~/my_xml_files -o ~/output.csv
```

