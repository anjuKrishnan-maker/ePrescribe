<%--Revision History
HA/JJ 11-Oct-2k6 113 RSS 'Show' and 'Hide'--%>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Xml.Xsl" %>
<script language="VB" runat="server">
	Public Color As String = "black"
    'Public Text As String = "This is a user control... really!"
	Sub Page_Load(sender As Object, e As EventArgs)

		' Using a live RSS feed... could also use a cached XML file.
        Dim strXmlSrc As String = "http://www.fda.gov/oc/po/firmrecalls/rssRecalls.xml"
		'Dim strXmlSrc As String = Server.MapPath("megatokyo.xml")

		' Path to our XSL file.  Changing the XSL file changes the
		' look of the HTML output.  Try toggling the commenting on the
		' following two lines to give it a try.
        'Dim strXslFile As String = Server.MapPath("megatokyo.xsl")
        Dim strXslFile As String = Server.MapPath("XXSSLL.xsl")

		' Load our XML file into the XmlDocument object.
		Dim myXmlDoc As XmlDocument = New XmlDocument()
        myXmlDoc.Load(strXmlSrc)
        
        'Dim firstlist As XmlNodeList = myXmlDoc.DocumentElement.RemoveChild(my
        'myXmlDoc.DocumentElement.RemoveChild(myXmlDoc.DocumentElement.SelectSingleNode("rss/channel"))
        
        Dim mbbool As Boolean = myXmlDoc.SelectSingleNode("rss/channel").HasChildNodes
        Dim i As Integer
        Dim d As Integer
        Dim oNode As XmlNode
        
        If mbbool = True Then
            i = myXmlDoc.SelectSingleNode("rss/channel").ChildNodes.Count
            For d = 6 To i - 1
                'Node = myXmlDoc.SelectSingleNode("rss/channel").ChildNodes(d)
                'myXmlDoc.SelectSingleNode("rss/channel").RemoveChild(oNode)
                'myXmlDoc.SelectSingleNode("rss/channel").ChildNodes(d).RemoveAll()
                oNode = myXmlDoc.SelectSingleNode("rss/channel").ChildNodes(d)
                If ((oNode Is Nothing) = False) Then
                    oNode.ParentNode.RemoveChild(oNode)
                End If
                oNode = Nothing
            Next d
        End If
        
        'Dim addxmlnode As XmlNode
        'Dim myXmlDocSnip As XmlDocument = New XmlDocument()
        'Dim d As Integer
        'For d = 0 To 4
        '    addxmlnode = firstlist.ItemOf(d)
        '    myXmlDocSnip.DocumentElement.AppendChild(addxmlnode)
        'Next d       
        
        ' Load our XSL file into the XslTransform object.
        Dim myXslDoc As XslCompiledTransform = New XslCompiledTransform()
        myXslDoc.Load(strXslFile)

        ' Create a StringBuilder and then point a StringWriter at it.
        ' We'll use this to hold the HTML output by the Transform method.
        Dim myStringBuilder As StringBuilder = New StringBuilder()
        Dim myStringWriter As StringWriter = New StringWriter(myStringBuilder)

        ' Call the Transform method of the XslTransform object passing it
        ' our input via the XmlDocument and getting output via the StringWriter.
        '        myXslDoc.Transform(myXmlDocSnip, Nothing, myStringWriter)
        myXslDoc.Transform(myXmlDoc, Nothing, myStringWriter)

        ' Since I've got the page set to cache, I tag on a little
        ' footer indicating when the page was actually built.
        'myStringBuilder.Append(vbCrLf & "<p><em>Cached at: " & Now() & "</em></p>" & vbCrLf)

        ' Take our resulting HTML and display it via an ASP.NET
        ' literal control.
        RSSFeed.Text = myStringBuilder.ToString

    End Sub
    'HA/JJ 113
    Protected Sub lnkbtnShowHide_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        if lnkbtnShowHide.Text="Hide" then
             RSSPanel.Visible = False
             lnkbtnShowHide.Text="Show"
        else
            RSSPanel.Visible = True
            lnkbtnShowHide.Text="Hide"
        End if
    End Sub
   
</script>


<%--<p>--%>
    <asp:Panel ID="Panel1" runat="server"  Width="160px">            
    <div 
        style="width: 203px; height: 18px">
        <!--HA/JJ 113-->
        <!-- HA/Kumar.V   Css added for show button -->
        <asp:LinkButton ID="lnkbtnShowHide" CssClass="allergyhead" Text="Hide" runat="server" OnClick="lnkbtnShowHide_Click"></asp:LinkButton>
        </div>        
        <asp:Panel ID="RSSPanel" runat="server"  Width="207px" BackColor="White" BorderStyle="Groove" Font-Bold="True" Font-Size="Smaller" ToolTip="HealthCareRSS" Visible="true">
            <asp:Literal ID="RSSFeed" runat="server"></asp:Literal>
            <a href="rssmorelist.aspx" target="_blank">more....</a>
        </asp:Panel>        
     </asp:Panel>
<%--</p>--%>
 
