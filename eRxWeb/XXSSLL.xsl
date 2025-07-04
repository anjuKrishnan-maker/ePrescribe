<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" /> 

	<xsl:template match="/">

		<xsl:for-each select="rss/channel">
			<a class ="RSSHeading" Target="_blank" href="{link}"><xsl:value-of select="title" /></a>
		</xsl:for-each>

		<ul>
			<xsl:for-each select="rss/channel/item">
				<li><a class ="RSSDetail" Target="_blank" href="{link}"><strong><xsl:value-of select="title" /></strong></a></li>
			</xsl:for-each>
		</ul>
	</xsl:template>

</xsl:stylesheet>
