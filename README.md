H4H Disaster App
================

app => this folder is for the mobile app. Currently written in C# (using Xamarin).
Visual Studio will restore missing NuGet packages the first time you build the solution. Please be patient as it may take a while.
You need the following in order to build the projects successfuly:
* Xamarin.Android + Android SDK (for building the Kastil.Droid project)
* Xamarin.iOS + a Mac somewhere (for building the Kastil.iOS project)

web => please put the web app in this folder.

When you load the solution, there will be a missing file: Connection.Details.cs. This is intentional (it's added in .gitignore), to avoid having to store appId and secretKey here.
Please add the file yourself, with the following content:

```
namespace Kastil.Core
{
    public partial class Connection
    {
        public Connection()
        {
            AppId = "get the appId value from other contributor";
            SecretKey = "get the secretkey value from other contributor";
        }
    }
}
```
