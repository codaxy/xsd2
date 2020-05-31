
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

```xml
  <PropertyGroup>
    <!-- XsdFilesPath points here to current folder. It can poing for example to external folder, like ..\..\sdk\schema\ -->
    <XsdFilesPath>.\</XsdFilesPath>
  </PropertyGroup>

  <ItemGroup>
    <None Include="$(XsdFilesPath)**\*.xsd" />
  </ItemGroup>
  <ItemGroup>
    <XSDFile Include="$(XsdFilesPath)**\*.xsd" />
  </ItemGroup>
  <ItemGroup>
    <!-- Monitor whether files are changed so the Inputs/Outputs comparision below will re-generate -->
    <UpToDateCheckInput Include="$(XsdFilesPath)**\*.xsd" />
  </ItemGroup>

  <ItemGroup>
    <Compile Update="*.cs">
      <!-- This will make Visual Studio fold each generated file under its xsd source -->
      <DependentUpon>$(XsdFilesPath)%(Filename).xsd</DependentUpon>
    </Compile>
  </ItemGroup>

  <Target Name="GenerateSerializationClasses" BeforeTargets="BeforeBuild" Inputs="@(XSDFile)" Outputs="@(XSDFile -> '%(Filename).cs')">
    <Message Importance="High" Text="Generating Schema classes: %(XSDFile.Identity)" />
    <Exec Command="$(Xsd2Exe) %(XSDFile.Identity) /o:$(ProjectDir) /ns:SomeNamespaces.Interfaces.Schema" />
  </Target>

  <Target Name="CleanGeneratedFiles" BeforeTargets="Clean">
    <ItemGroup>
      <_FilesToDelete Include="*.cs"/>
    </ItemGroup>
    <Delete Files="@(_FilesToDelete)"/>
  </Target>
```

## Notes:

* [PetaTest](http://www.toptensoftware.com/petatest/) framework is used for testing.
* Original idea http://mikehadlow.blogspot.com/2007/01/writing-your-own-xsdexe.html.
