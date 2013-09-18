using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace SsisUnit
{
    class CSharpEval
    {
        public static object Eval(string CSCode)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");

            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;
            StringBuilder sb = new StringBuilder("");

            sb.Append("using System;\n");

            sb.Append("namespace CSCodeEvaler{ \n");
            sb.Append("public class CSCodeEvaler{ \n");
            sb.Append("public object EvalCode(){\n");
            sb.Append("return " + CSCode + "; \n");
            sb.Append("} \n");
            sb.Append("} \n");
            sb.Append("}\n");

            CompilerResults cr = c.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
            {
                throw new ArgumentException(cr.Errors[0].ErrorText, "CSCode");
            }

            System.Reflection.Assembly a = cr.CompiledAssembly;
            object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");
            Type t = o.GetType();
            MethodInfo mi = t.GetMethod("EvalCode");
            object s = mi.Invoke(o, null);
            return s;
        }

        public static object EvalWithParam(string sCSCode, object oParam)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");

            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;
            StringBuilder sb = new StringBuilder("");

            sb.Append("using System;\n");
            
            sb.Append("namespace CSCodeEvaler{ \n");
            sb.Append("public class CSCodeEvaler{ \n");
            sb.Append("public object EvalCode(object result){\n");
            //sb.Append("MessageBox.Show(oParam.ToString()); \n");
            sb.Append("return " + sCSCode + "; \n");
            sb.Append("} \n");
            sb.Append("} \n");
            sb.Append("}\n");
            
            CompilerResults cr = c.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
            {
                throw new ArgumentException(cr.Errors[0].ErrorText, "CSCode");
            }

            System.Reflection.Assembly a = cr.CompiledAssembly;
            object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");
            Type t = o.GetType();
            MethodInfo mi = t.GetMethod("EvalCode");
            object[] oParams = new object[1];
            oParams[0] = oParam;
            object s = mi.Invoke(o, oParams);
            return s;
        }

        public static object EvalWithRef(string sCSCode)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("EvalCSCode.exe");

            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;
            StringBuilder sb = new StringBuilder("");

            sb.Append("using System;\n");
            sb.Append("using EvalCSCode;\n");

            sb.Append("namespace CSCodeEvaler{ \n");
            sb.Append("public class CSCodeEvaler{ \n");
            sb.Append("public object EvalCode(){\n");
            sb.Append("object o = new object(); \n");
            sb.Append(sCSCode + " \n");
            sb.Append("return o; \n");
            sb.Append("} \n");
            sb.Append("} \n");
            sb.Append("}\n");
            //Debug.WriteLine(sb.ToString())// ' look at this to debug your eval string

            CompilerResults cr = c.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
            {
                throw new ArgumentException(cr.Errors[0].ErrorText, "CSCode");
            }

            System.Reflection.Assembly a = cr.CompiledAssembly;
            object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");
            Type t = o.GetType();
            MethodInfo mi = t.GetMethod("EvalCode");
            object s = mi.Invoke(o, null);
            return s;
        }
    }
}
