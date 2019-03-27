using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;

namespace Suplanus.Stapi
{
  public class DeviceHelper
  {
    public static Software GetSoftware(DeviceItem deviceItem)
    {
      SoftwareContainer softwareContainer = deviceItem.GetService<SoftwareContainer>();
      return softwareContainer?.Software;
    }

    public static NodeComposition GetNetworkNodes(DeviceItem deviceItem)
    {
      var networkInterface = deviceItem.GetService<NetworkInterface>();
      return networkInterface?.Nodes;
    }
  }
}
