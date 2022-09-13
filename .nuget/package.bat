rem Package the library for Nuget
copy ..\MaxFactry.Module.Cms-NF-4.5.2\bin\Release\MaxFactry.Module.Cms*.dll lib\Cms\net452\

c:\install\nuget\nuget.exe pack MaxFactry.Module.Cms.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 

copy ..\MaxFactry.Module.Cms.Mvc4-NF-4.5.2\bin\MaxFactry.Module.Cms.Mvc4*.dll lib\Cms.Mvc4\net452\

c:\install\nuget\nuget.exe pack MaxFactry.Module.Cms.Mvc4.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 
