using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;

namespace Suplanus.Stapi.Example
{
  class Program
  {
    private static string DataPath => GetDataPath();

    private static string GetDataPath()
    {
      return Path.Combine(
        Directory.GetParent(
                   Directory.GetParent(
                     Directory.GetCurrentDirectory()).Parent?.FullName)
                 .Parent?.FullName ??
        throw new InvalidOperationException(), "data");
    }

    static void Main()
    {
      Console.WriteLine("Set firewall...");
      Assembly assembly = Assembly.GetExecutingAssembly();
      Starter.SetTiaPortalFirewall(assembly);
      TiaPortal tiaPortal = null;
      try
      {
        Console.WriteLine("Get instance...");
        tiaPortal = Starter.GetInstance(TiaPortalMode.WithUserInterface);

        Console.WriteLine("Create project...");
        string projectFullPath = Path.Combine(DataPath, "debug", "MyProject", "MyProject.ap15_1");
        Project project = ProjectHelper.CreateProjectIfNotExists(tiaPortal, projectFullPath);

        Console.WriteLine("Insert devices...");
        List<Device> devices = InsertDevices(project);

        Console.WriteLine("Get network interfaces...");
        List<Node> nodes = GetNetworkInterfaces(devices);

        Console.WriteLine("Create Subnet...");
        NetworkHelper.ConnectNodesToNetwork(project, "System:Subnet.Ethernet", "MySubnet", nodes);

        Console.WriteLine("Get software...");
        List<Software> softwareList = GetSoftwareList(devices);

        Console.WriteLine("Insert blocks from SCL source...");
        List<PlcSoftware> plcSoftwareList = softwareList.OfType<PlcSoftware>().ToList();
        InsertBlocksFromSclSource(plcSoftwareList);

        Console.WriteLine("Compile...");
        CompileHelper.Compile(softwareList);

        Console.WriteLine("Save project...");
        project.Save();

        Console.WriteLine("Show block in editor...");
        var block = plcSoftwareList.FirstOrDefault()?.BlockGroup.Blocks
                                   .OfType<OB>()
                                   .FirstOrDefault(obj => obj.Number.Equals(1));
        block?.ShowInEditor();
      }
      finally
      {
        tiaPortal?.Dispose();
      }
    }

    private static List<Device> InsertDevices(Project project)
    {
      // Clear
      foreach (var projectDevice in project.Devices.ToList())
      {
        projectDevice.Delete();
      }

      // Name not allowed for example HMI
      Device plc = project.Devices.CreateWithItem("OrderNumber:6ES7 211-1AD30-0XB0/V2.2", "PLC_1", null);
      Device hmi = project.Devices.CreateWithItem("OrderNumber:6AV2 123-2GB03-0AX0/15.1.0.0", "HMI_1", null);
      return new List<Device> { plc, hmi };
    }

    private static List<Node> GetNetworkInterfaces(List<Device> devices)
    {
      List<Node> nodes = new List<Node>();
      foreach (var device in devices)
      {
        foreach (var deviceItem in device.DeviceItems)
        {
          var nodesComposition = DeviceHelper.GetNetworkNodes(deviceItem);
          if (nodesComposition != null)
          {
            nodes.AddRange(nodesComposition);
          }
        }
      }
      return nodes;
    }

    private static List<Software> GetSoftwareList(List<Device> devices)
    {
      List<Software> softwareList = new List<Software>();
      foreach (var device in devices)
      {
        foreach (var deviceItem in device.DeviceItems)
        {
          Software software = DeviceHelper.GetSoftware(deviceItem);
          if (software != null)
          {
            softwareList.Add(software);
          }
        }
      }
      return softwareList;
    }

    private static void InsertBlocksFromSclSource(List<PlcSoftware> plcSoftwareList)
    {
      foreach (var plcSoftware in plcSoftwareList)
      {
        string sourceName = "SclSource";
        string sourcePath = Path.Combine(DataPath, sourceName + ".scl");
        BlockHelper.InsertBlocksFromSclSource(plcSoftware, sourceName, sourcePath);
      }
    }
  }
}