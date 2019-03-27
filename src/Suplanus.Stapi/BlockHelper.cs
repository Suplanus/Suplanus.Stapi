using System;
using System.Linq;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.ExternalSources;

namespace Suplanus.Stapi
{
  public class BlockHelper
  {
    public static void InsertBlocksFromSclSource(PlcSoftware plcSoftware, string sourceName, string sourcePath)
    {
      // Create source
      var source = plcSoftware.ExternalSourceGroup.ExternalSources.FirstOrDefault(obj => obj.Name.Equals(sourceName));
      if (source == null)
      {
        Console.WriteLine("Load SCL sources...");
        source = plcSoftware.ExternalSourceGroup.ExternalSources.CreateFromFile(sourceName, sourcePath);
      }

      // Create block from source
      PlcBlock block = plcSoftware.BlockGroup.Blocks.FirstOrDefault(obj => obj.Name.Equals(sourceName));
      if (block == null)
      {
        Console.WriteLine("Generate block...");
        GenerateBlockOption options = GenerateBlockOption.KeepOnError;
        source.GenerateBlocksFromSource(options);
      }
    }
  }
}
