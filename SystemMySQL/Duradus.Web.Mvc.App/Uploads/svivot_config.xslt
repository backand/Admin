<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:XMLSchema.xsd="http://tempuri.org/XMLSchema.xsd">
  <xsl:template match="node()|@*">
    <xsl:copy>
      <xsl:apply-templates select="node()|@*"/>
    </xsl:copy>
  </xsl:template>
   
  <xsl:template  match="*[@elementName]">
    <xsl:element name="{@elementName}">
      <xsl:apply-templates select="node()|@*"/>
     </xsl:element>
  </xsl:template>
  
  <xsl:template  match="*[@ResultType]">
    <xsl:element  name="{@ResultType}">
      <xsl:apply-templates select="node()|@*"/>
    </xsl:element>
  </xsl:template>

  <xsl:template  match="*[@ResultType='JoinResults']">
    <xsl:element  name="{@ResultType}">
      <xsl:apply-templates select="node()|@*"/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="//Columns">
    <xsl:element name="Results">
      <xsl:apply-templates select="node()|@*"/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="@*[.='Yes']">
    <xsl:variable name="LocalName" select="local-name()"/>
    <xsl:attribute name="{$LocalName}">
      <xsl:value-of select="'yes'"/>
    </xsl:attribute>
  </xsl:template>
  <xsl:template match="@*[.='No']">
    <xsl:variable name="LocalName" select="local-name()"/>
    <xsl:attribute name="{$LocalName}">
      <xsl:value-of select="'no'"/>
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="@ResultType" ></xsl:template>
  <xsl:template match="@elementName" ></xsl:template>
  <xsl:template match="@showLink[.='No']"></xsl:template>
  <xsl:template match="@default[.='No']"></xsl:template>
  <xsl:template match="//id" ></xsl:template>
  <xsl:template match="@id" ></xsl:template>
  <xsl:template match="//NewDataSet" >
    <xsl:apply-templates select="node()|@*"/>
  </xsl:template>
</xsl:stylesheet>








<!--<xsl:stylesheet version="1.0"
 xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
 
 <xsl:template match="//NewDataSet/P8CM_Config/P8CM_Workspace/Action/Results/Result">
  <xsl:template match="*">  
  <column>
        <xsl:apply-templates/>
    
    </column>
  </xsl:template>


</xsl:stylesheet>-->