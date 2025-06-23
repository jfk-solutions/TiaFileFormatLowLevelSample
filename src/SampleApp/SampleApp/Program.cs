using TiaFileFormat;
using TiaFileFormat.Database;
using TiaFileFormat.ExtensionMethods;

var networkInformationConverter = new TiaFileFormat.Wrappers.Controller.Network.NetworkInformationConverter();

var tfp = TiaFileProvider.CreateFromSingleFile("V19.plf"); //Could be a Tia-Project File, a archived Project or a PLF
var database = TiaDatabaseFile.Load(tfp);

var cpus = database.ProjectRoot.Traverse(x => x.ProjectTreeChildren).Where(x => x.StorageBusinessObjectType == TiaFileFormat.Database.StorageTypes.StorageBusinessObjectType.Plc);
foreach (var cpu in cpus)
{
    var networkInfos = cpu.Traverse(x => x.ProjectTreeChildren)
        .Where(x => networkInformationConverter.GetHighLevelObjectType(x) == TiaFileFormat.Wrappers.HighLevelObjectType.NetworkInformation)
        .Select(x => networkInformationConverter.Convert(x, null))
        .OfType<TiaFileFormat.Wrappers.Controller.Network.NetworkInformation>()
        .ToList();

    Console.WriteLine("CPU: " + cpu.Name + " (" + cpu.Path + ")");
    foreach(var nw in networkInfos)
    {
        Console.WriteLine("  " + nw.Name + ": " + nw.IpAddress);
    }
}
Console.ReadLine();