using System.Collections.Generic;
using System.Linq;
using Siemens.Engineering;
using Siemens.Engineering.HW;

namespace Suplanus.Stapi
{
  public class NetworkHelper
  {
    public static void ConnectNodesToNetwork(Project project, string typeIdentifier, string name, List<Node> nodes)
    {
      Subnet subnet = project.Subnets.FirstOrDefault(obj => obj.Name.Equals(name) && obj.TypeIdentifier.Equals(typeIdentifier)) ??
                      project.Subnets.Create(typeIdentifier, name);

      foreach (var node in nodes)
      {
        node.ConnectToSubnet(subnet);
      }
    }
  }
}
