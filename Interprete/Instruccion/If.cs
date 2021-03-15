﻿using Proyecto1.TS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto1.Interprete.Instruccion
{
    class If : Instruccion
    {
        private Expresion.Expresion valor;
        private LinkedList<Instruccion> instrucciones;
        private Instruccion _else;
        object salida;
        public If(Expresion.Expresion valor, LinkedList<Instruccion> instrucciones, Instruccion _else)
        {
            this.valor = valor;
            this.instrucciones = instrucciones;
            this._else = _else;
            this.Semanticos = new List<Analisis.Error>();
        }
        public override object Ejecutar(TabladeSimbolos ts)
        {
            Simbolo valor = this.valor.Evaluar(ts);
            if (valor == null || valor.Value==null) 
            {
                this.listaErrores.Add(new Analisis.Error("Semantico","La condicion es incorrecta en sentencia IF",0,0));
                return this.salida = "";
            }
            if (valor.Tipo.tipo != Tipos.BOOLEAN)
            {
                this.listaErrores.Add(new Analisis.Error("Semantico", "La condicion no es BOOLEANA en sentencia IF", 0, 0));
                return this.salida;
            }

            if (bool.Parse(valor.Value.ToString()))
            {
                try
                {
                    foreach (var instruccion in instrucciones)
                    {
                        if (instruccion != null)
                        {
                            Object output = instruccion.Ejecutar(ts);
                            if (output is Break)
                            {
                                return output;
                            }
                            else if (output is Continue)
                            {
                                break;
                            }
                            else if (output is Exit)
                            {
                                return output;
                            }
                            else
                            {
                                this.salida = output;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                if (_else != null) 
                {
                    this.salida = _else.Ejecutar(ts);
                }
            }
            return this.salida;
        }
    }
}
