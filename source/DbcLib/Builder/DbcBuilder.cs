using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Builder
{
    public static class DbcBuilder
    {
        private static List<IBuilder> builders = new List<IBuilder>()
        {
            new VersionBuilder(),
            new SymbolBuilder(),
            new AttributeDefineBuilder(),
            new MessageBuilder(),
        };
        public static Dbc CreateDbc()
        {
            Dbc dbc = new Dbc();
            foreach (var builder in builders)
            {
                builder.Build(dbc);
            }
            return dbc;
        }
    }
}