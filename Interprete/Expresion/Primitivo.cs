﻿using Proyecto1.Codigo3D;
using Proyecto1.Interprete.Instruccion;
using Proyecto1.TS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Interprete.Expresion
{
    class Primitivo : Expresion
    {

        public char tipo;
        public object valor;

        public Primitivo(char tipo, Object valor)
        {
            this.tipo = tipo;
            this.valor = valor;
        }

        public override Simbolo Evaluar(TabladeSimbolos ts)
        {
            Simbolo primitivo = null; 
            switch(this.tipo)
            {
                case 'A':
                    AccesoArray accesito = (AccesoArray)this.valor;
                    object valor_accesitoo = accesito.Ejecutar(ts);
                    Arreglo tmp_arr = ts.getArray(accesito.id);
                    primitivo = new Simbolo("primitivo", tmp_arr.Tipo,0,0,false);
                    primitivo.Value = valor_accesitoo;
                    break;
                case 'N':
                    primitivo = new Simbolo("primitivo", new Tipo(Tipos.INT, "integer"), 0, 0, false);
                    primitivo.Value = this.valor;
                    break;
                case 'S':
                    primitivo = new Simbolo("primitivo", new Tipo(Tipos.STRING, "string"), 0, 0, false);
                    string val = this.valor.ToString();
                    val = val.Replace("'","");
                    primitivo.Value = val;
                    break;
                case 'B':
                    primitivo = new Simbolo("primitivo", new Tipo(Tipos.BOOLEAN, "boolean"), 0, 0, false);
                    primitivo.Value = this.valor;
                    break;
                case 'R':
                    primitivo = new Simbolo("primitivo", new Tipo(Tipos.REAL, "real"), 0, 0, false);
                    primitivo.Value = this.valor;
                    break;
                case 'I':
                    primitivo = ts.getVariableValor(this.valor.ToString());
                    break;
                case 'O':
                    if (this.valor is AccesoObjeto) 
                    {
                        AccesoObjeto tmp = (AccesoObjeto)this.valor;
                        object new_valor = tmp.Ejecutar(ts);
                        primitivo = (Simbolo)new_valor;
                    }
                    break;
                case 'L':
                    Simbolo_Funcion funct = null;
                    List<object> salida = new List<object>(); 
                    if (valor is Llamada) 
                    {
                        object output;
                        Llamada llamadita = (Llamada)valor;
                        output = llamadita.Ejecutar(ts);
                        funct = ts.getFuncion(llamadita.id);
                    }
                    return funct;
            }
            return primitivo;
        }

        public override string generar3D(TabladeSimbolos ts, Intermedio c3d)
        {
            string code = "";
            switch (this.tipo)
            {
                case 'A':
                    break;
                case 'R':
                case 'N':
                    code += this.valor;
                    break;
                case 'S':
                    code += c3d.tmp.generarTemporal() + " = HP; //guardo la referencia de inicio de la cadena\n";
                    string cadena = this.valor.ToString();
                    cadena = cadena.Replace("'", "");
                    foreach (var car in cadena)
                    {
                        code += "Heap[(int)HP] = " + (int)car + ";   //" + car + "\n";
                        code += "HP = HP + 1; \n";
                    }
                    code += "Heap[(int)HP] = -1;\n";
                    code += "HP = HP + 1; \n";
                    break;
                case 'B':
                    if (this.valor.ToString().ToLower().Equals("true")) code += "1";
                    if (this.valor.ToString().ToLower().Equals("false")) code += "0";
                    break;
                case 'I':
                    string varval = ts.getVariablePos(this.valor.ToString());
                    string[] search;
                    if (varval != null)
                    {
                        search = varval.Split(':');
                    }
                    else
                    {
                        search = new string[3];
                    }
                    if (search[1].Equals("global"))
                    {
                        code += c3d.tmp.generarTemporal() + " = Heap[(int)" + search[0] + "];";
                    }
                    else if (search[1].Equals("param"))
                    {
                        code += c3d.tmp.generarTemporal() + " = SP + " + search[0] + "; //posicion de parametro " + search[2] + "\n";
                        string tmp_param = c3d.tmp.getLastTemporal();
                        code += c3d.tmp.generarTemporal() + " = Stack[(int)" + tmp_param + "];\n";
                    }
                    else
                    {
                        code += c3d.tmp.generarTemporal() + " = Stack[(int)" + search[0] + "];";
                    }
                    break;
                case 'O':
                    break;
                case 'L':
                    if (valor is Llamada)
                    {
                        Llamada llamadita = (Llamada)valor;
                        code += llamadita.generar3D(ts, c3d);
                    }
                    break;
            }
            return code;
        }

        Simbolo_Funcion encontrarRetorno(List<object> salida)
        {
            foreach (var nodo in salida) {
                if (nodo is Simbolo_Funcion) 
                    return (Simbolo_Funcion)nodo;
            }
            return null;
        }
    }
}
