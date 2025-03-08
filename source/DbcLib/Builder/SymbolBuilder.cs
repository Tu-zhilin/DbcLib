﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Builder
{
    internal class SymbolBuilder : IBuilder
    {
        private List<string> symbols = new List<string>()
        {
            "NS_DESC_",
            "CM_",
            "BA_DEF_",
            "BA_",
            "VAL_",
            "CAT_DEF_",
            "CAT_",
            "FILTER",
            "BA_DEF_DEF_",
            "EV_DATA_",
            "ENVVAR_DATA_",
            "SGTYPE_",
            "SGTYPE_VAL_",
            "BA_DEF_SGTYPE_",
            "BA_SGTYPE_",
            "SIG_TYPE_REF_",
            "VAL_TABLE_",
            "SIG_GROUP_",
            "SIG_VALTYPE_",
            "SIGTYPE_VALTYPE_",
            "BO_TX_BU_",
            "BA_DEF_REL_",
            "BA_REL_",
            "BA_DEF_DEF_REL_",
            "BU_SG_REL_",
            "BU_EV_REL_",
            "BU_BO_REL_",
            "SG_MUL_VAL_",
        };
        public void Build(Dbc dbc)
        {
            dbc.Symbol.AddRange(symbols);
        }
    }
}