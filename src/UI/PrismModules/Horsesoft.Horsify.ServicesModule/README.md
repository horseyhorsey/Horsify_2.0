# GOOD CONFIG

	<?xml version="1.0" encoding="utf-8" ?>
	<configuration>
	  <system.serviceModel>
		<bindings>
		  <basicHttpBinding>
			<binding name="BasicHttpBinding_IHorsifySongService" maxBufferPoolSize="2147483647"
						maxBufferSize="2147483647" maxReceivedMessageSize="2147483647">
			  <readerQuotas maxDepth="2147483647" maxStringContentLength="2147483647"
				  maxArrayLength="2147483647" maxBytesPerRead="2147483647" maxNameTableCharCount="2147483647" />
			</binding>
		  </basicHttpBinding>
		</bindings>
		<client>
		  <endpoint address="http://localhost:8080/HorsifySongService"
			  binding="basicHttpBinding" bindingConfiguration="BasicHttpBinding_IHorsifySongService"
			  contract="HorsifyService.IHorsifySongService" name="BasicHttpBinding_IHorsifySongService" />
		</client>
	  </system.serviceModel>
	</configuration>