using System.Collections.Generic;
using Siemens.Engineering;
using Siemens.Engineering.Compiler;
using Siemens.Engineering.HW;

namespace Suplanus.Stapi
{
  public class CompileHelper
  {
    public static void Compile(List<Software> softwareList)
    {
      foreach (var software in softwareList)
      {
        Compile(software);
      }
    }

    public static void Compile(Software software)
    {
      if (software is IEngineeringServiceProvider engineeringServiceProvider)
      {
        ICompilable compilable = engineeringServiceProvider.GetService<ICompilable>();
        compilable?.Compile();
      }
    }
  }
}
