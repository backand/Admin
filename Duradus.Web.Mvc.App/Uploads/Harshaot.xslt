<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:XMLSchema.xsd="http://tempuri.org/XMLSchema.xsd">
  <xsl:template match="node()|@*">
    <xsl:copy>
      <xsl:apply-templates select="node()|@*"/>
    </xsl:copy>
  </xsl:template>
  <xsl:template match="@show[.='No']"></xsl:template>
  <xsl:template match="//Harshaa">
    <xsl:element name="Harshaa">
      <xsl:value-of select="@Text" />
    </xsl:element>
  </xsl:template>
  <xsl:template match="//P8CM_Config" >
    <xsl:apply-templates select="node()|@*"/>
  </xsl:template>
</xsl:stylesheet>
 