
# xsd2


Improved version of xsd.exe.

This version enables:

* List based collections in generated types
* Auto-capitalization of properties
* Nullable attribute types
* Removal of DebuggerStepThrough attribute

## Usage:
### Command line:
xsd2.exe &lt;schema file&gt; [/o:&lt;output-directory&gt;] [/ns:&lt;namespace&gt;] /all

### Example running for embedding in your CSPROJ (C# project):

	<ItemGroup>
		<XSDFile Include="**/*.xsd" />  
	</ItemGroup>  
	<Target Name=“GenerateSerializationClasses” BeforeTargets=“BeforeBuild” Inputs="%(XSDFile.Identity)" Outputs="%(XSDFile.Identity).cs">  
		<Message Importance=“High” Text=“Generating xsd code…%(XSDFile.Identity)” />  
		<Exec Command=“xsd2.exe %(XSDFile.Identity) /o:$(ProjectDir) /ns:My.Namespace.Example” />  
	</Target>

## Notes:

* [PetaTest](http://www.toptensoftware.com/petatest/) framework is used for testing.
* Original idea http://mikehadlow.blogspot.com/2007/01/writing-your-own-xsdexe.html.
