namespace Infra;

using LinqToDB;
using LinqToDB.Data;
using Infra.Entities;

public class AppDataConnection(DataOptions options) : DataConnection(options)
{
    public ITable<User> Users => this.GetTable<User>();
}
