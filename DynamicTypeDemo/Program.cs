using DynamicTypeDemo.Entities;
using DynamicTypeDemo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace DynamicTypeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = "(Id==1 and Name==\"dsfdsf\")or(Id>=100 and Name!=\"\\\"sdfsdfs\")and IsDelete==false";
            Console.WriteLine(s);
            Console.WriteLine("-=解析=-");
            var c = DynamicCondition.Parse(s);
            var exp = c.GenerateExperssion<TableTemplate>();
            Console.WriteLine(c);
            Console.WriteLine("-=Linq=-");
            Console.WriteLine(exp);
            Console.ReadKey();
            /*for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine(i);
                DoSomething();
            }*/
            return;
            var sql = typeof(IT_GeneralTable).SQLGenerateCreateTable("dddddddddddd");
            /*var db = new Model2();
            var template = db.TableTemplates.FirstOrDefault(p=>p.Id == 3005);
            var sql = template.SQLGenerateCreateTable(template.Name);*/
            Console.Write(sql);
            //Console.ReadKey();

            /*
            var template = DynamicEntityService.GetTemplate(3005);
            var type = template.CreateType("T_Metadata", new Type[] { typeof(IT_GeneralTable) });
            var service1 = new DynamicEntityService( type, "T_Metadata");

            //var service = new DynamicEntityService(typeof(TableTemplate), "TableTemplate");
            var list = service1.GetEntities();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            Console.WriteLine(json);
            
            service1.PostEntity(Activator.CreateInstance(type));
            list = service1.GetEntities();
            json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            */
            //Console.ReadKey();
        }

        static void DoSomething()
        {
            var cond = new DynamicCondition("ND", "==", "rtyrty\\\"1999").And("SYS_ID","==", 1);
            cond = cond.And(new DynamicCondition("FLH","==","XZ11").Or("FLH", "==", "\\\"XZ12"));
            Console.WriteLine(cond);
            parsecondition(cond.ToString());
            var template = DynamicEntityService.GetTemplate(3005);
            var type = template.CreateType("T_Metadata", new Type[] { typeof(IT_GeneralTable) });
            var db = type.GetDynamicEntityModel();
            //var service1 = new DynamicEntityService(type, "T_Metadata");

            //var service = new DynamicEntityService(typeof(TableTemplate), "TableTemplate");
            var list = db.GetEntities();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            //Console.WriteLine(json);

        }

        static DynamicCondition parsecondition(string condition)
        {
            try
            {
                int index = 1;
                Stack<string> strings = new Stack<string>();
                Dictionary<string, DynamicCondition> exps = new Dictionary<string, DynamicCondition>();
                var r = new Regex("\"(.*?[^\\\\])\"");
                condition = r.Replace(condition, p =>
                {
                    strings.Push(p.Value.Replace("\\\"", "\""));
                    return "$strings";
                });
                strings = new Stack<string>(strings);
                r = new Regex("([_a-zA-Z]\\w*?)\\s*([=!<>]{1,2})\\s*([\\w\\.$]+)");
                var f = new Regex("\\d+\\.\\d+");
                var i = new Regex("\\d+");
                condition = r.Replace(condition, p =>
                {
                    //strings.Push(p.Value);
                    var field = p.Groups[1].Value;
                    var opertion = p.Groups[2].Value;
                    object value = p.Groups[3].Value;
                    if ((string)value  == "$strings")
                    {
                        value = strings.Pop();
                    }
                    else if((string)value =="true" || (string)value =="false")
                    {
                        value = bool.Parse((string)value);
                    }else if (f.IsMatch((string)value))
                    {
                        value = float.Parse((string)value);
                    }else if (i.IsMatch((string)value))
                    {
                        value = int.Parse((string)value);
                    }
                    else
                    {
                        throw new Exception("无效的数值格式。");
                    }
                    exps.Add("$exp" + index, new DynamicCondition(field, opertion, value));
                    return "$exp" + index++;
                });

                if (strings.Count > 0)
                {
                    throw new Exception("格式错误：" + strings.Pop());
                }

                r = new Regex("\\s*\\(\\s*(\\$exp\\d+)\\s*\\)\\s*");
                var and = new Regex("(\\$exp\\d+)\\s*and\\s*(\\$exp\\d+)", RegexOptions.IgnoreCase);
                var or = new Regex("(\\$exp\\d+)\\s*or\\s*(\\$exp\\d+)", RegexOptions.IgnoreCase);
                do
                {

                    do
                        condition = and.Replace(condition, p =>
                        {
                        exps.Add("$exp" + index, exps[p.Groups[1].Value] * exps[p.Groups[2].Value]);
                            return string.Format(" $exp{0} ", index++);
                        });
                    while (and.IsMatch(condition));
                    do
                        condition = or.Replace(condition, p =>
                        {
                        exps.Add("$exp" + index, exps[p.Groups[1].Value] + exps[p.Groups[2].Value]);
                            return string.Format(" $exp{0} ", index++);
                        });
                    while (or.IsMatch(condition));
                    condition = r.Replace(condition, p =>
                    {
                    return " " + p.Groups[1].Value + " ";

                    });
                } while (r.IsMatch(condition) || and.IsMatch(condition) || or.IsMatch(condition));
                return exps[condition.Trim()];
            }
            catch
            {
                throw;
            }

        }

        void test()
        {
            /*var t = TypeCreator.Creator("Qwert", 10);
            object obj = Activator.CreateInstance(t);
            var ps = t.GetProperties();
            ps.Where(p=>p.PropertyType == typeof(string)).ToList().ForEach(p =>
            {
                p.SetValue(obj,"ddddddddd");
            });*/


            /*
            SqlServerMigrationSqlGenerator gen = new SqlServerMigrationSqlGenerator();

            
            
            var operations = new List<MigrationOperation>();
            var table1 = new CreateTableOperation("fdgdg");
            var id = new ColumnModel(System.Data.Entity.Core.Metadata.Edm.PrimitiveTypeKind.Int32) { Name = "id", IsIdentity = true };
            table1.Columns.Add(id);
            table1.PrimaryKey = new AddPrimaryKeyOperation();
            table1.PrimaryKey.Columns.Add("id");
            operations.Add(table1);
            var sql = gen.Generate( operations , "2008");*/


            /*var db = new Model1();
            var list = db.getdata();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            Type listtype = typeof(List<>).MakeGenericType(db.type);
            var obj = JsonConvert.DeserializeObject(json, listtype);*/
            //list.Where(new )

            TableTemplate template = new TableTemplate()
            {
                Name = "TEMPLATE_100"
            };
            template.Fields.Add(new TableTemplateField() { Name = "ND", Type = TableTemplateFieldType.String, Length = 20 });
            template.Fields.Add(new TableTemplateField() { Name = "FLH", Type = TableTemplateFieldType.String, Length = 20 });
            template.Fields.Add(new TableTemplateField() { Name = "AJH", Type = TableTemplateFieldType.Int });
            template.Fields.Add(new TableTemplateField() { Name = "DH", Type = TableTemplateFieldType.String, Length = 50 });
            template.Fields.Add(new TableTemplateField() { Name = "AJTM", Type = TableTemplateFieldType.String, Length = 200 });
            template.Fields.Add(new TableTemplateField() { Name = "BGQX", Type = TableTemplateFieldType.String, Length = 20 });
            template.Fields.Add(new TableTemplateField() { Name = "DW", Type = TableTemplateFieldType.String, Length = 100 });

            var sql = template.SQLGenerateCreateTable("T_100");
            var type = template.CreateType("T_100", "T_100");
            using (var db = new Model1(new List<Type>() { type }))
            {
                /*//var tran = db.Database.BeginTransaction();
                db.Database.ExecuteSqlCommand(sql);
                var list = db.getdata(type,"sys_id", 1);
                tran.Rollback();*/
                //db.Database.Connection.
                //var ss = System.Data.Entity.Core.Common.DbProviderServices.
                var list = db.getdata(type, "sys_id", 1);

            }
            
            var sqla = template.SQLGenerateAlterTable("dddddddd");
        }
    }
}
