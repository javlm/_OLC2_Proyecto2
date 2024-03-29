﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2.Optimización.Reglas
{
    class Etiqueta : Instruccion3D
    {
        string label;
        int fila;

        public Etiqueta(string l, int f) 
        {
            this.fila = f;
            this.label = l;
        }
        public override string optimizar3d(Dictionary<string, int> Etiquetas, Dictionary<string, int> Saltos)
        {
            if(!Etiquetas.ContainsKey(this.label)) 
                Etiquetas.Add(this.label, this.fila);
            return "";
        }
    }
}
