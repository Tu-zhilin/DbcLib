using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Builder
{
    internal class VersionBuilder : IBuilder
    {
        public void Build(Dbc dbc)
        {
            dbc.Version = "";
        }
    }
}