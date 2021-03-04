# xsltrans - An XML Transform Utility
A simple utility for applying an [XSLT](https://www.w3.org/TR/xslt/) stylesheet to one or more XML files conceptually similar to [xsltproc](https://linux.die.net/man/1/xsltproc).  The utility can be run on a single XML file or by specifying a folder containing multiple XML files with a search pattern.

## Command Line Args
| Short | Long | Description |
| ------|------|-------------|
|\-x| \--xslt| XSLT File Path|
|\-f| \--file|XML File to transform (cannot specify both file and folder)|
|\-F| \--folder|Folder path containing XML Files to transform (cannot specify both file and folder)|
|\-i| \--include|When transforming XML files by folder specify file extensions to include (ex: -i *.xml or --include *.xml)|
|\-o| \--out|Output file path|
|\-h| \--help|Displays the help|

## Examples

### Batch transform

```bash
$ xsltrans -x ~/xml2csv.xslt -F ~/my_xml_files -o ~/output.csv
```

### Batch transform with search filter

```bash
$ xsltrans -x ~/xml2txt.xslt -F ~/my_xml_files -i *02_2020.xml -o ~/output.txt
```

### Single File

```bash
$ xsltrans -x ~/xml2html.xslt -f ~/my_doc.xml -o ~/output.html
```
