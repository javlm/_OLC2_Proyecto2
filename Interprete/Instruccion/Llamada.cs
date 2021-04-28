﻿using Proyecto1.Codigo3D;
using Proyecto1.Interprete.Expresion;
using Proyecto1.TS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto1.Interprete.Instruccion
{
    class Llamada : Instruccion
    {
        public String id;
        private List<Expresion.Expresion> exp_list;
        public object retorno;
        public Llamada(String id, List<Expresion.Expresion> exp_list) 
        {
            this.id = id.ToLower();
            this.exp_list = exp_list;
            this.Semanticos = new List<Analisis.Error>();
        }
        public override object Ejecutar(TabladeSimbolos ts)
        {
            Simbolo_Funcion funcion = ts.getFuncion(this.id);
            if (funcion == null)
            {
                this.Semanticos.Add(new Analisis.Error("Semantico", "La funcion" + this.id + "no existe.", 0, 0));
                return "";
            }
            TabladeSimbolos ts_funcion = new TabladeSimbolos(ts, ts.alias+funcion.Id);
            if (this.exp_list != null) 
            {
                if (funcion.Params.Count == this.exp_list.Count)
                {
                    int i = 0;
                    foreach(var par in funcion.Params)
                    {
                        par.Value.ValorPar = this.exp_list[i].Evaluar(ts).Value;
                        i++;
                    }
                }
                foreach (var par in funcion.Params)
                {
                    Simbolo tmp_param = new Simbolo(par.Value.Id, par.Value.Tipo, 0, 0, par.Value.Referencia);
                    tmp_param.Value = par.Value.ValorPar;
                    ts_funcion.declararVariable(par.Value.Id, tmp_param);
                }
            } 
            
            foreach (var inst in funcion.listaInst) 
            {
                if (inst != null) 
                {
                    inst.Ejecutar(ts_funcion);
                }
            }
            foreach (var sent in funcion.listaSent) 
            {
                if (sent != null)
                {
                    if (sent is Asignacion)
                    {
                        Asignacion posible_return = (Asignacion)sent;
                        retorno = sent.Ejecutar(ts_funcion);
                        if (posible_return.id == this.id) 
                        {
                            retorno = sent.Ejecutar(ts_funcion);
                            return retorno;
                        }
                            
                    }
                    else 
                    {
                        object output = sent.Ejecutar(ts_funcion);
                        if (output is Break)
                        {
                            return "";
                        }
                        else if (output is Continue)
                        {
                            break;
                        }
                        else if (output is Exit)
                        {
                            Exit returnf = (Exit)output;
                            Simbolo nuevo = (Simbolo)returnf.valor_exit;
                            ts.setFuncionValor(this.id, nuevo.Value);
                            return ts.getFuncion(this.id);
                        }
                        else
                        {
                            this.retorno = output;
                        }
                    }
                }
            }
            if (funcion.Tipo != null) {
                return this.retorno;
            }
            return "";
        }

        public override string generar3D(TabladeSimbolos ts, Intermedio inter)
        {
            string void_name = inter.getVoid(this.id);
            string code = "//---> Inicio de Llamada de funcion" + this.id + "\n";
            if (this.exp_list != null)
            {
                foreach (var exp in this.exp_list)
                {
                    if (!(exp is Primitivo))
                    {
                        code += exp.generar3D(ts, inter);
                        string tmp_exp = inter.tmp.getLastTemporal();
                        code += inter.tmp.generarTemporal() + " = " + tmp_exp + ";\n";
                        inter.param_tmp.Enqueue(inter.tmp.getLastTemporal());
                    }
                    else
                    {
                        Simbolo tmp = exp.Evaluar(ts);
                        switch (tmp.Tipo.tipoAuxiliar) 
                        {
                            case "string":
                                code += exp.generar3D(ts, inter);
                                string tmp_cadena = inter.tmp.getLastTemporal();
                                code += inter.tmp.generarTemporal() + " = " + tmp_cadena + ";\n";
                                break;
                            default:
                                code += inter.tmp.generarTemporal() + " = " + exp.generar3D(ts, inter) + ";\n";
                                break;
                        }
                        inter.param_tmp.Enqueue(inter.tmp.getLastTemporal());
                    }
                }
            }
            else 
            {
                code += "//no hay parametros en la llamada \n"; 
            }
            
            code += "SP = SP + 1;         //cambio de ambito simulado\n";
            code += "//-----> se guardan temporales\n";
            string tmp_guardado = inter.tmp.generarTemporal();
            for (int i = 0; i < inter.param_tmp.Count; i++) 
            {
                code += tmp_guardado + " = SP + " + i + ";\n" ;
                code += "Stack[(int)" + tmp_guardado + "] = " + inter.param_tmp.ElementAt(i)+";\n";
            }
            code += "//----------------------------- \n";
            code += "SP = SP + " + inter.getVoidSize(this.id) + ";\n";
            code += "//-------Paso de Parametros \n";
            for (int j = 0; j < inter.param_tmp.Count; j++)
            {
                code += inter.tmp.generarTemporal() + " = SP + " + (j+1) + ";\n";
                code += "Stack[(int)" + inter.tmp.getLastTemporal() + "] = " + inter.param_tmp.ElementAt(j) + ";\n";

            }

            code += void_name+"();\n";

            return code;
        }
    }
}
