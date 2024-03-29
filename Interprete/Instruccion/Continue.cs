﻿using Proyecto1.Codigo3D;
using Proyecto1.TS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Interprete.Instruccion
{
    class Continue : Instruccion
    {
        public override object Ejecutar(TabladeSimbolos ts)
        {
            return this;
        }

        public override string generar3D(TabladeSimbolos ts, Intermedio inter)
        {
            inter.lcontinue = inter.lrecursives.Peek();
            return "goto " + inter.lcontinue + "; //etieuqeta CONTINUE\n";
        }
    }
}
