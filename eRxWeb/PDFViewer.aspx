<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PDFViewer.aspx.cs" Inherits="eRxWeb.PDFViewer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Scripts/pdfjs-dist/web/viewer.css" rel="stylesheet" />
   
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
    <div class="container">
         <div id="progress" class="loader"></div>
    </div>
   
    <div id="viewerContainer">
        <div id="viewer" class="pdfViewer">.</div>

    </div>

    <script src="Scripts/pdfjs-dist/build/pdf.js"></script>
    <script src="Scripts/pdfjs-dist/web/pdf_viewer.js"></script>
    <script src="Scripts/pdfjs-dist/simpleviewer.js"></script>
    <script type="text/javascript">
        PDFSet('<%=PdfUrl%>', 'viewerContainer');
    </script>
</body>
</html>
