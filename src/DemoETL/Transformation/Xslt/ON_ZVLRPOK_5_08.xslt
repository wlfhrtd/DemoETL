<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output
        method="xml"
        indent="yes"
        encoding="windows-1251"/>

	<!-- тянем из C# -->
	<xsl:param name="dateId"/>
	<xsl:param name="dateDoc"/>

	<xsl:template match="/Root">

		<Файл
            ВерсФорм="5.08"
            ТипИнф="ЭСНДСТСНП"
            ВерсПрог="DemoETL"
            ИдФайл="{concat('ON_ZVLRPOK_', Sender/TaxAuthority, '_', Sender/TaxAuthority, '_', Sender/INN, Sender/KPP, '_', $dateId, '_', IdFileSuffix)}"
            КодНО="{Sender/TaxAuthority}">

			<Документ
                КНД="1110017"
                ДатаДок="{$dateDoc}">

				<СвОтпр>
					<ОтпрЮЛ
                        НаимОрг="{Sender/Name}"
                        ИННЮЛ="{Sender/INN}"
                        КПП="{Sender/KPP}" />
				</СвОтпр>

				<Подписант ПрПодп="{Signer/Type}">
					<ФИО
						Фамилия="{Signer/LastName}"
						Имя="{Signer/FirstName}">

						<xsl:if test="string-length(Signer/MiddleName) &gt; 0">
							<xsl:attribute name="Отчество">
								<xsl:value-of select="Signer/MiddleName"/>
							</xsl:attribute>
						</xsl:if>

					</ФИО>
				</Подписант>

				<СвЗвл
                    НомерДокНП="1"
                    ДатаДокНП="{$dateDoc}"
					ПрЛизинг="{Declaration/LeasingFlag}"
					ПрДавСырья="{Declaration/TollingFlag}"
                    БазаНДС="{Declaration/VatBaseTotal}"
					ИтогоАкциз="{Declaration/ExciseTotal}"
                    ИтогоНДС="{Declaration/VatTotal}"
					ПВДок="{Declaration/StatementReason}">

					<СвКонтракт1
						ИдНомПродР1="{Declaration/ContractInfo/SellerId}"
						ПрПродФЛ="{Declaration/ContractInfo/SellerIsIndividual}"
						НаимПродР1="{Declaration/ContractInfo/SellerName}"
						КодСтранПродР1="{Declaration/ContractInfo/SellerCountryCode}"
						АдресПродР1="{Declaration/ContractInfo/SellerAddress}"
						ИдНомПокР1="{Declaration/ContractInfo/BuyerId}"
						НаимПокР1="{Declaration/ContractInfo/BuyerName}"
						КодСтранПокР1="{Declaration/ContractInfo/BuyerCountryCode}"
						АдресПокР1="{Declaration/ContractInfo/BuyerAddress}">

						<СвКонтр1
							НомКонтр="{Declaration/ContractInfo/ContractDocument/ContractDocumentNumber}"
							ДатаКонтр="{Declaration/ContractInfo/ContractDocument/ContractDocumentDate}">

								<СвСпециф
									НомПСпециф="{Declaration/ContractInfo/ContractDocument/SpecificationInfo/SpecificationApplicationNumber}"
									НомСпециф="{Declaration/ContractInfo/ContractDocument/SpecificationInfo/SpecificationNumber}"
									ДатаСпециф="{Declaration/ContractInfo/ContractDocument/SpecificationInfo/SpecificationDate}" />

						</СвКонтр1>

					</СвКонтракт1>

					<xsl:apply-templates select="Products/Product"/>

				</СвЗвл>

			</Документ>
		</Файл>

	</xsl:template>

	<xsl:template match="Product">

		<СвТовар
            НомТовПП="{position()}"
            НаимТов="{Name}"
			ТНВЭД="{TnVedCode}"
			ЕдИзмТов="{UnitCode}"
            КоличТов="{format-number(Quantity, '0.000000')}"
            СтоимТов="{format-number(Price, '0.00')}"
			ВалТов="{CurrencyCode}"
			КурсВал="{format-number(CurrencyRate, '0.0000')}"
			БазаВал="{format-number(CurrencyMultiplier, '0')}"
			НомСчФ="{InvoiceNumber}"
			ДатаСчФ="{InvoiceDate}"
			ДатаПрин="{AcceptanceDate}"
			НБАкциз="{format-number(ExciseBase, '0.000000')}"
			ЕдИзмТовНБАкц="{ExciseUnitCode}"
            НБНДС="{format-number(VatBase, '0.00')}"
			СтАкцизТверд="{format-number(ExciseRateFixed, '0.00')}"
			СтАкцизАдвал="{format-number(ExciseRateAdValorem, '0.00')}"
            СтНДС="{format-number(VatRate, '0.00')}"
			СумАкциз="{format-number(ExciseAmount, '0.00')}"
            СумНДС="{format-number(VatAmount, '0.00')}"
			ПрОсвАкциз="{IsExciseExempt}"
			ПрОсвНДС="{IsVatExempt}">

			<xsl:apply-templates select="TransportDocumentsInfo/TransportDocument"/>

		</СвТовар>

	</xsl:template>

	<xsl:template match="TransportDocument">

		<СвТСД
			СерНомТСД="{Number}"
			ДатаТСД="{Date}"/>
	
	</xsl:template>

</xsl:stylesheet>