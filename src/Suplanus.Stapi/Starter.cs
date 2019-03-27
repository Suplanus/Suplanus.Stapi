using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using Microsoft.Win32;
using Siemens.Engineering;

namespace Suplanus.Stapi
{
  public class Starter
  {
    public static TiaPortal GetInstance(TiaPortalMode mode)
    {
      var tiaPortalProcesses = TiaPortal.GetProcesses()
                                        .Where(obj => obj.Mode.Equals(mode))
                                        .ToList();
      if (tiaPortalProcesses.Any())
      {
        return tiaPortalProcesses.First().Attach();
      }
      return new TiaPortal(mode);
    }

    public static void SetTiaPortalFirewall(Assembly assembly)
    {
      // Check if admin
      WindowsIdentity identity = WindowsIdentity.GetCurrent();
      WindowsPrincipal principal = new WindowsPrincipal(identity);
      bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
      if (!isAdmin)
      {
        Debug.WriteLine("Suplanus.Stapi: Firewall could not be set, you are no admin");
        return;
      }

      string exePath = assembly.Location;

      // Get hash
      HashAlgorithm hashAlgorithm = SHA256.Create();
      FileStream stream = File.OpenRead(exePath);
      byte[] hash = hashAlgorithm.ComputeHash(stream);
      string convertedHash = Convert.ToBase64String(hash);

      // Get date
      FileInfo fileInfo = new FileInfo(exePath);
      DateTime lastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
      string lastWriteTimeUtcFormatted = lastWriteTimeUtc.ToString("yyyy'/'MM'/'dd HH:mm:ss.fff");

      // Get execution version
      AssemblyName siemensAssembly = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                                             .First(obj => obj.Name.Equals("Siemens.Engineering"));
      string version = siemensAssembly.Version.ToString(2);

      // Set key and values
      string keyFullName = $@"SOFTWARE\Siemens\Automation\Openness\{version}\Whitelist\{fileInfo.Name}\Entry";
      RegistryKey key = Registry.LocalMachine.CreateSubKey(keyFullName);
      if (key == null)
      {
        throw new Exception("Key note found: " + keyFullName);
      }
      key.SetValue("Path", exePath);
      key.SetValue("DateModified", lastWriteTimeUtcFormatted);
      key.SetValue("FileHash", convertedHash);
    }
  }
}
