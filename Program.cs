using CIS.CodeDom.Csharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Oid.DNA.Start
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var s = (new Program().Make1() as ICsCode).GenerateSourceCode();                   //  теперь получаем исходный код из построенной AST-модели

            Console.WriteLine(s);
            Console.ReadLine();
        }

        public CsCompileUnit Make1()
        {
            var ccu = new CsCompileUnit();                                            //  установить единицу компиляции
            var ns = new CsNamespace(new CsIdentifier("System"));                       //  объявить пространство имен, которые будут включены в приложение
            var u1 = new CsUsing(ns);                                                       //  установить директиву использования пространства имен
            ccu.Usings.Usings.Add(u1);                                                          //  добавить его в модуль компиляции
            var cn = new CsNamespace(new CsIdentifier(ECsNames.Namespace));             //  объявить главное пространство имен
            ccu.UnitMembers.Members.Add(cn);                                                    //  привязать пространство имен к единице компиляции
            var cl = new CsClass();                                                         //  оздать новый класс
            ((ICsClass)cl).ClassName = new CsIdentifier(nameof(Program));                             //  установить его имя как предопределенное для главного класса
                                                                                                      //  it is in principle optional, see. System.CodeDom-entry point
            cn.NamespaceMembers.Members.Add(cl);                                                //  внедрить созданный класс в пространство имен программы
            var m1 = new CsMethod(new CsIdentifier(nameof(Program.Main)));                               //  создать основной метод - точку входа в основной класс программы
            (m1 as ICsModifiersProvider).Modifiers = new CsModifiers(ECsModifiers.Static);        //  указать, что он static
            ((ICsMethod)m1).TypeResult = new CsTypeReference(new CsVoid());                    //  задать возвращаемый тип
            cl.Body.Members.Add(m1);                                                            //  добавить метод в тело класса (1)
            var m2 = new CsMethod(new CsIdentifier("WriteLine"));                          //  добавить внешний метод (он указан по своему имени)
                                                                                           //  здесь предполагаем, что разбор внешних сборок выполнен
                                                                                           //  на которые указана ссылка в атрибутах компиляции
            var cm1 = new CsExpressionCall(new CsMethodReference(m2));             //  декларация вызова метода
            var l1 = new CsExpressionLiteral(new CsLiteralString("message"));   //  декларация значения аргумента вызова
            var a1 = new CsArgument(null, l1);                                           //  расположение аргумента в списке аргументов
            ((ICsArgument)a1).ArgumentInitialisationType = ECsInitialisationType.Expression;    //  указывают на то, что аргумент содержит простое выражение, и это его значение
            cm1.Arguments.Arguments.Add(a1);                                                    //  в коде вызова добавить этот аргумент
            var n1 = new CsExpressionName("Console");                              //  указать имя вызываемого внешнего метода
            var e1 = new CsExpressionQualifiedExpression(n1, cm1);  //  указание необходимости создать квалифицированное имя
            (e1 as ICsExpressionQualifiedExpression).Access = ECsObjectAccess.Reference;          //  фрагменты квалифицированного имени сочленяются через "."
            var s1 = new CsStatementExpression(e1);                           //  создаем предложение кода
            m1.Body.Statements.Statements.Add(s1);                                              //  и помещаем его в тело метода


            return ccu;
        }
    }
}

//~
//  OUTPUT

/*
using System;
namespace Namespace9
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine
            ("message");
        }
    }
}
*/
