using TiaFileFormat;
using TiaFileFormat.Database;
using TiaFileFormat.Wrappers.UserManagement;

var tfp = TiaFileProvider.CreateFromSingleFile("X:\\aa\\project.zap19"); //adjust to a project
var database = TiaDatabaseFile.Load(tfp);

var userConverter = new UserConverter();
var users = userConverter.GetProjectUsers(database);
Console.WriteLine();
Console.WriteLine("Username:");
var username = Console.ReadLine();
Console.WriteLine();
Console.WriteLine("Passwort:");
var password = Console.ReadLine();
var myUser = users.FirstOrDefault(x => x.Name == username);

Console.WriteLine();
if (myUser != null)
{
    var result = myUser.CheckPassword(password!);
    Console.WriteLine(result ? "Password was correct" : "Password was wrong");
}
else
{
    Console.WriteLine("User not found");
}
Console.WriteLine();
Console.WriteLine("Press Key to Continue");
Console.ReadLine();