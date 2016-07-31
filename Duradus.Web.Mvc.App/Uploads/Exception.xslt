<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:XMLSchema.xsd="http://tempuri.org/XMLSchema.xsd">
  <xsl:template match="node()|@*">
    <xsl:copy>
      <xsl:apply-templates select="node()|@*"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="@Ordinal" ></xsl:template>
  <xsl:template match="//P8CM_Config" >
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