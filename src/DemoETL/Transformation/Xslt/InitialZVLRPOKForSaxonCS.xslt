<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="3.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" indent="yes" encoding="windows-1251"/>

	<xsl:variable name="dateDoc" select="format-date(current-date(), '[D01].[M01].[Y0001]')"/>
	<xsl:variable name="dateId" select="format-date(current-date(), '[Y0001][M01][D01]')"/>

	<xsl:template match="/Root">

		<Файл
            ВерсФорм="5.08"
            ТипИнф="ЭСНДСТСНП"
            ВерсПрог="DemoETL"
            ИдФайл="{concat('ON_ZVLRPOK_', Sender/INN, '_', Sender/KPP, '_', $dateId)}">

			<Документ
                КНД="1110017"
                ДатаДок="{$dateDoc}">

				<СвОтпр>
					<ОтпрЮЛ
                        НаимОрг="{Sender/Name}"
                        ИННЮЛ="{Sender/INN}"
                        КПП="{Sender/KPP}" />
				</СвОтпр>

				<СвЗвл
                    НомерДокНП="1"
                    ДатаДокНП="{$dateDoc}"
                    БазаНДС="{format-number(sum(Products/Product/Price), '0.00')}"
                    ИтогоНДС="{format-number(sum(Products/Product/Price) * 0.2, '0.00')}">

					<xsl:apply-templates select="Products/Product"/>

				</СвЗвл>

			</Документ>
		</Файл>

	</xsl:template>

	<xsl:template match="Product">
		<СвТовар
            НомТовПП="{position()}"
            НаимТов="{Name}"
            КоличТов="{format-number(Quantity, '0.000000')}"
            СтоимТов="{format-number(Price, '0.00')}"
            НБНДС="{format-number(Price, '0.00')}"
            СтНДС="20.00"
            СумНДС="{format-number(Price * 0.2, '0.00')}" />
	</xsl:template>

</xsl:stylesheet>