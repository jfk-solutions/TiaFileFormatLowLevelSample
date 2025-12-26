using Siemens.Simatic.HwConfiguration.Model;
using TiaFileFormat;
using TiaFileFormat.Database;
using TiaFileFormat.Database.StorageTypes;
using TiaFileFormat.ExtensionMethods;
using TiaFileFormat.Wrappers.Controller.Network;

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
    foreach(var nw in networkInfos.OfType<EthernetNetworkInformation>())
    {
        Console.WriteLine("  " + nw.Name + ": " + nw.IpAddress);
    }
}
Console.WriteLine();
Console.WriteLine("Press Key to Continue");
Console.ReadLine();







//*********************************************
//  And now without high level functions...
//*********************************************

foreach (var cpu in cpus)
{
    var nwNodes = cpu.ProjectTreeChildren
                        .Where(x => x.TiaTypeName == "Siemens.Simatic.HwConfiguration.Model.DeviceItemData")
                        .Where(x => x.CoreAttributes?.Subtype == "S71500.CPU.Interface.IE")
                        .ToList();

    Console.WriteLine("CPU: " + cpu.Name + " (" + cpu.Path + ")");
    foreach (var nwNode in nwNodes)
    {
        var deviceItemData = nwNode.GetChild<DeviceItemData>();
        var persistentExpandoData = nwNode.GetChild<PersistentExpandoData>();

        var nwInfoNode = nwNode.GetRelationsWithNameResolved("Siemens.Simatic.HwConfiguration.Model.NodeData.DeviceItemNodes").FirstOrDefault();
        var persistentExpandoDatanwInfoNode = nwInfoNode.GetChild<PersistentExpandoData>();
        //   ^-- Contains the Network Information
        if (persistentExpandoDatanwInfoNode.TryGetValue("NodeIPAddress", out var ipObj))
        {
            var ip = (long)ipObj;
            var ipAddress = string.Join('.', BitConverter.GetBytes(ip).Take(4).Reverse().Select(x => x.ToString()));
            Console.WriteLine("  " + nwNode.Name + " / " + nwInfoNode.Name + ": " + ipAddress);
        }







        //**************************************************************
        //Only for Info about Relations and StorageBusinessObjects
        //**************************************************************

        //Every StorrageBusinessObject has multiple Children, wich you can see in the "Children" object Collection, and get them via "GetChild" and the Type a generic Parameter
        //and also in the CHildren Collection there are often multiple Relation Lists. (relations are links to other StorageBusinessObjects)
        //You can get a list of all Relations Names of an object like this:
        var relationNames = nwNode.GetAllRelations().Select(x => x.Name);
        //You can resolve a relation like this:
        var rel = nwNode.GetAllRelations().First();
        var targetObj = rel.StoreObjectId.StorageObject as StorageBusinessObject; // most of the time the relation links to a StorageBusinessObject
        //You can also get all the Names of the Target Types the Relation Links to:
        var relationResolvedTiaTypeNames = nwNode.GetAllRelationsResolved().Select(x => x.TiaTypeName).ToList();
        //So the RelationName and the TiaTypeName of the relation target are different.
        //foreach (var e in relationNames.Zip(relationResolvedTiaTypeNames))
        //{
        //    Console.WriteLine("Relation: " + e.First + " -> " + e.Second);
        //}
    }
}
Console.WriteLine();
Console.WriteLine("Press Key to Continue");
Console.ReadLine();