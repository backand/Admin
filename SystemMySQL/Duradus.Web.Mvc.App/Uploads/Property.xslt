<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" >
  <xsl:output method="xml" indent="yes" doctype-system=" http://java.sun.com/dtd/properties.dtd" />

  <!--<xsl:output method="xml"   doctype-system=" http://java.sun.com/dtd/properties.dtd" doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"/>-->
  <xsl:template match="node()|@*">
    <xsl:copy>
      <xsl:apply-templates select="node()|@*"/>
    </xsl:copy>
  </xsl:template>
  <xsl:template  match="*[@entryType]">
    <xsl:element  name="{@entryType}">
      <xsl:apply-templates select="node()|@*"/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="//entry">
    <xsl:element name="{@entryType}">
      <xsl:apply-templates select="@key"/>
      <xsl:value-of select="@text" />
    </xsl:element>
  </xsl:template>
  <xsl:template match="//P8CM_Config" >
    <xsl:apply-templates select="node()|@*"/>
  </xsl:template>
  <!--<xsl:template match="/">
    --><!--
since doctype-public="" doctype-system="" in xsl:output
doesn't uniformly produce an empty DOCTYPE across all
XSLT processors, we'll use d-o-e
--><!--
    <xsl:value-of
    select="'&lt;!DOCTYPE rootelement&gt;'"/>
  </xsl:template>-->
  <!---->
</xsl:stylesheet>
 