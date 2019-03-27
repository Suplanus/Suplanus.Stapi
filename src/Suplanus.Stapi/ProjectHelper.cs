using System;
using System.IO;
using System.Linq;
using Siemens.Engineering;

namespace Suplanus.Stapi
{
  public class ProjectHelper
  {
    public static Project CreateProjectIfNotExists(TiaPortal tiaPortal, string projectFullPath)
    {
      Project project;

      // Open
      if (File.Exists(projectFullPath))
      {
        FileInfo fileInfo = new FileInfo(projectFullPath);
        project = tiaPortal.Projects.FirstOrDefault(obj => obj.Path.FullName.Equals(projectFullPath));
        if (project == null)
        {
          project = tiaPortal.Projects.Open(fileInfo);
        }
      }

      // Create
      else
      {
        string projectName = Path.GetFileNameWithoutExtension(projectFullPath);
        var directory = Path.GetDirectoryName(projectFullPath);
        DirectoryInfo directoryInfo = new DirectoryInfo(directory ?? throw new InvalidOperationException());
        project = tiaPortal.Projects.Create(directoryInfo, projectName);
      }
      return project;
    }
  }
}
