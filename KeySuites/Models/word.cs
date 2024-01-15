using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Word;
//using Microsoft.Office.Interop.Word.Application;
//using Range = Microsoft.Office.Interop.Word.Range;
//using Microsoft.Office.Interop.Word.Document;

namespace Vidly.Models
{
    public class word
    {
        public Application app = new Application();
        public Range range;
        public Document document = new Document();
        public Bookmark bookmark;
        public Bookmarks bookmarks;
        public WdSaveFormat wdSave = WdSaveFormat.wdFormatPDF;
        public WdSaveFormat wdSaveHTML = WdSaveFormat.wdFormatHTML;
        public WdSaveFormat wdSaveOpenDocText = WdSaveFormat.wdFormatOpenDocumentText;
        public WdSaveFormat wdSaveRTF = WdSaveFormat.wdFormatRTF;
        public WdSaveFormat wdSaveOpenXMLDoc = WdSaveFormat.wdFormatStrictOpenXMLDocument;
        public WdSaveFormat wdSaveText = WdSaveFormat.wdFormatText;
        public WdSaveFormat wdSaveWebArchive = WdSaveFormat.wdFormatWebArchive;
        public WdSaveFormat wdSaveXML = WdSaveFormat.wdFormatXML;
        public WdSaveFormat wdSaveXMLDoc = WdSaveFormat.wdFormatXMLDocument;
        public WdSaveFormat OnlywdSave = WdSaveFormat.wdFormatDocument;
        public WdExportFormat wd = Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF;
        public ContentControls ccl;
        public ContentControl cc;
    }
}