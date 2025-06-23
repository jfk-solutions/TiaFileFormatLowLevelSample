using TiaFileFormat;
using TiaFileFormat.Database;

var tfp = TiaFileProvider.CreateFromSingleFile("V19.plf"); //Could be a Tia-Project File, a archived Project or a PLF
var database = TiaDatabaseFile.Load(tfp);

var root = database.RootObject;