# How to Create and Build the project
## Step by Step instructions


First note that I used Visual Studio 2017 with the SQL Server data tools included. See the about window below.

![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/VS_help_about.jpg "Visual Studio About")

The steps are straightforward
1. Create a new project
2. Insert the source files
3. Include the references
4. *Optional* - Configure the post build event - *only needed for Sql Server 2008 R2*
5. Build

### Create the Project
Create a new data tools project (File -> New -> Project) and choose SQL Server Database Project.  Note, if you plan to run this on a sql server 2008R2, then you’ll need to choose .net 3.0 (in the screen shot).  On server 2016 it didn’t matter, that CLR could do .net 4.5  *Make the title SQLCLR_getSPList if you want things to match* and then click "Ok"
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/new_project.jpg "New Project")

### Add  the source files
Add the five source files to the project by selecting the SQLCLR_getSPList project in the Solution Explorer, and then from the menu Project -> Add Existing Item.  Add all 5 .cs files.
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/add_files_menu.jpg "Add Files Step")

### Include the References

Again, making sure the project SQLCLR_getSPList is selected in the Solution Explorer, navigate from the menu Project -> Add Reference
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/add_reference_menu.jpg "Add Reference Step")

From the Assemblies -> Framework section, select the four choices:
- System
- System.Data
- System.Web.Services
- System.Xml

And then click the “Ok” Button.  In the screen shot below you can't see all the references as the list is too long, but still you should find all four and click the checkboxes.
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/selecting_references.jpg "Selecting References")

The Solution Explorer should now look like this:
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/complete_solution_explorer.jpg "Solution Explorer")

You can now try a test compile from the menu Build -> Build Solution.  It should work:
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/success_build.jpg "First Build")

In the output directory you’ll see the generated files as well.
![](https://raw.githubusercontent.com/matt-jk/SQLCLR_getSPList/master/images/build_output_1.jpg "Output of First Build")

If you are deploying to SQL Server 2016, you can skip the next step.  This step is required for SQL Server 2008R2.  It may or may not be required on SQL 2012, I didn’t test it there.  This step has to do with the serialization / writing intermediate files to disk to handle the xml with the web service.

You need the program “sgen.exe” and on my system there were a handful of copies for different .net versions.  The one I found to work was in the folder below, yours may be in a different folder.
C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\x64\sgen.exe
Adding this will create the file SP2010WS.XmlSerializers.dll

Make sure the SQLCLR_getSPList project is highlighted in the solution explorer, and from the menu navigate to Project -> SQLCLR_getSPList Properties.  From the “Build Events” section, enter the location of your chosen sgen.exe, with the parameters /force “$(TargetPath)”.  In my example I did this:

"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\x64\sgen.exe" /force "$(TargetPath)"


Now you should be ready to compile the project.  From the menu click Build -> Rebuild Solution:

And these are the output files:

SQLCLR_getSPList.dacpac
SQLCLR_getSPList.pdb
these are used for a model of deployment, I didn’t use them.
SQLCLR_getSPList.dll  <-- this is the file you need
SQLCLR_getSPList.XmlSerializers.dll <-- this file only needed for SqlServer 2008R2


To deploy to the Sql Server 2008R2, copy the two dll files onto a folder on the database server

