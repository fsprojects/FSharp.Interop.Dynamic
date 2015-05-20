try{
  
  $solname = "FSharp.Dynamic" 
  $testname = "Tests"
  $projectname = $solname
  $projecttype ="fsproj"

  #Build PCL and .NET version from one project using msbuild script
  &("C:\Program Files (x86)\MSBuild\12.0\Bin\msbuild.exe") ..\$solname.sln /p:Configuration=Release
  
  if (!$?){
     throw $error[0].Exception
  }
  
  #Download and configure Nunit test runner
  ..\.nuget\nuget.exe install NUnit.Runners -Version 2.6.4 -o ..\packages
  Copy-Item nunit-console.exe.config ..\packages\NUnit.Runners.2.6.4\tools\
  
  if (!$?){
     throw $error[0].Exception
  }
  
  #Test portable
  Echo "Testing Portable"
  ..\packages\NUnit.Runners.2.6.4\tools\nunit-console.exe /framework:net-4.5 /noxml /nodots /labels /stoponerror /exclude=Performance ..\$testname\bin\Release\$testname.dll
 
  if (!$?){
     throw $error[0].Exception
  }
  
 
}catch{
  Echo "Build Failed"
  exit
}

#if successful create nuget package
..\.nuget\nuget.exe pack ..\$projectname\$projectname.$projecttype -Properties Configuration=Release -Symbols
Echo "Nuget Success"