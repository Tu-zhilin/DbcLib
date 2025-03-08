using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Builder
{
    internal interface IBuilder
    {
        void Build(Dbc dbc);
    }
}