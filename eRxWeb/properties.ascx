<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Xml.Xsl" %>
<script language="VB" runat="server">
	Public Color As String = "black"
    'Public Text As String = "This is a user control... really!"
	Sub Page_Load(sender As Object, e As EventArgs)

		' Using a live RSS feed... could also use a cached XML file.
        Dim strXmlSrc As String = "http://www.nih.gov/news/feed.xml"
		'Dim strXmlSrc As String = Server.MapPath("megatokyo.xml")

		' Path to our XSL file.  Changing the XSL file changes the
		' look of the HTML output.  Try toggling the commenting on the
		' following two lines to give it a try.
        'Dim strXslFile As String = Server.MapPath("megatokyo.xsl")
        Dim strXslFile As String = Server.MapPath("XXSSLL.xsl")

		' Load our XML file into the XmlDocument object.
		Dim myXmlDoc As XmlDocument = New XmlDocument()
		myXmlDoc.Load(strXmlSrc)

		' Load our XSL file into the XslTransform object.
        Dim myXslDoc As XslCompiledTransform = New XslCompiledTransform()
		myXslDoc.Load(strXslFile)

		' Create a StringBuilder and then point a StringWriter at it.
		' We'll use this to hold the HTML output by the Transform method.
		Dim myStringBuilder As StringBuilder = New StringBuilder()
		Dim myStringWriter  As StringWriter  = New StringWriter(myStringBuilder)

		' Call the Transform method of the XslTransform object passing it
		' our input via the XmlDocument and getting output via the StringWriter.
		myXslDoc.Transform(myXmlDoc, Nothing, myStringWriter)

		' Since I've got the page set to cache, I tag on a little
		' footer indicating when the page was actually built.
		myStringBuilder.Append(vbCrLf & "<p><em>Cached at: " & Now() & "</em></p>" & vbCrLf)

		' Take our resulting HTML and display it via an ASP.NET
		' literal control.
        RSSFeed.Text = myStringBuilder.ToString

	End Sub

</script>


<p>
<font color="<%= Color %>">
    <asp:Panel ID="RSSPanel" runat="server" Height="380px" Width="180px" BackColor="White" Font-Bold="True" Font-Size="Smaller" ToolTip="HealthCareRSS" BorderStyle="Groove" ScrollBars="Vertical">
    <asp:Literal ID = "RSSFeed" runat = "server"></asp:Literal>
    </asp:Panel>

</font>
</p>
 
